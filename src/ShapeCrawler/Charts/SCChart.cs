using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using OneOf;
using ShapeCrawler.Collections;
using ShapeCrawler.Exceptions;
using ShapeCrawler.Shapes;
using ShapeCrawler.Shared;
using SkiaSharp;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Charts;

internal class SCChart : SCShape, IChart
{
    private readonly ResettableLazy<ICategoryCollection?> categories;
    private readonly Lazy<SCChartType> chartType;
    private readonly Lazy<OpenXmlElement?> firstSeries;
    private readonly P.GraphicFrame pGraphicFrame;
    private readonly Lazy<SCSeriesCollection> series;
    private readonly Lazy<SCLibraryCollection<double>?> xValues;

    // Contains chart elements, e.g. <c:pieChart>, <c:barChart>, <c:lineChart> etc. If the chart type is not a combination,
    // then collection contains only single item.
    private readonly IEnumerable<OpenXmlElement> cXCharts;

    private string? chartTitle;

    internal SCChart(
        P.GraphicFrame pGraphicFrame, 
        OneOf<SCSlide, SCSlideLayout, SCSlideMaster> parentSlideObject, 
        OneOf<ShapeCollection, SCGroupShape> parentShapeCollection)
        : base(pGraphicFrame, parentSlideObject, parentShapeCollection)
    {
        this.pGraphicFrame = pGraphicFrame;
        this.firstSeries = new Lazy<OpenXmlElement?>(this.GetFirstSeries);
        this.xValues = new Lazy<SCLibraryCollection<double>?>(this.GetXValues);
        this.series = new Lazy<SCSeriesCollection>(this.GetSeries);
        this.categories = new ResettableLazy<ICategoryCollection?>(this.GetCategories);
        this.chartType = new Lazy<SCChartType>(this.GetChartType);

        var cChartReference = this.pGraphicFrame.GetFirstChild<A.Graphic>() !.GetFirstChild<A.GraphicData>() !
            .GetFirstChild<C.ChartReference>() !;

        var slideStructureCore = (SlideStructure)this.SlideStructure; 

        this.ChartPart = (ChartPart)slideStructureCore.TypedOpenXmlPart.GetPartById(cChartReference.Id!);

        var cPlotArea = this.ChartPart.ChartSpace.GetFirstChild<C.Chart>() !.PlotArea;
        this.cXCharts = cPlotArea!.Where(e => e.LocalName.EndsWith("Chart", StringComparison.Ordinal));

        this.ChartWorkbook = this.ChartPart.EmbeddedPackagePart != null ? new ChartWorkbook(this, this.ChartPart.EmbeddedPackagePart) : null;
    }

    public SCChartType Type => this.chartType.Value;

    public override SCShapeType ShapeType => SCShapeType.Chart;

    public string? Title
    {
        get
        {
            this.chartTitle = this.GetTitleOrDefault();
            return this.chartTitle;
        }
    }

    public bool HasTitle
    {
        get
        {
            this.chartTitle ??= this.GetTitleOrDefault();
            return this.chartTitle != null;
        }
    }

    public bool HasCategories => this.categories.Value != null;

    public ISeriesCollection SeriesCollection => this.series.Value;

    public ICategoryCollection? Categories => this.categories.Value;

    public bool HasXValues => this.xValues.Value != null;

    public SCLibraryCollection<double> XValues
    {
        get
        {
            if (this.xValues.Value == null)
            {
                throw new NotSupportedException(ExceptionMessages.NotXValues);
            }

            return this.xValues.Value;
        }
    }

    public override SCGeometry GeometryType => SCGeometry.Rectangle;

    public byte[] WorkbookByteArray => this.ChartWorkbook!.BinaryData;

    public SpreadsheetDocument SDKSpreadsheetDocument => this.ChartWorkbook!.SpreadsheetDocument.Value;

    internal ChartWorkbook? ChartWorkbook { get; set; }

    internal ChartPart ChartPart { get; private set; }

    internal override void Draw(SKCanvas canvas)
    {
        throw new NotImplementedException();
    }

    private SCChartType GetChartType()
    {
        if (this.cXCharts.Count() > 1)
        {
            return SCChartType.Combination;
        }

        var chartName = this.cXCharts.Single().LocalName;
        Enum.TryParse(chartName, true, out SCChartType enumChartType);

        return enumChartType;
    }

    private ICategoryCollection? GetCategories()
    {
        return CategoryCollection.Create(this, this.firstSeries.Value, this.Type);
    }

    private SCSeriesCollection GetSeries()
    {
        return SCSeriesCollection.Create(this, this.cXCharts);
    }

    private string? GetTitleOrDefault()
    {
        var cTitle = this.ChartPart.ChartSpace.GetFirstChild<C.Chart>() !.Title;
        if (cTitle == null)
        {
            // chart has not title
            return null;
        }

        var cChartText = cTitle.ChartText;
        bool staticAvailable = this.TryGetStaticTitle(cChartText!, out var staticTitle);
        if (staticAvailable)
        {
            return staticTitle;
        }

        // Dynamic title
        if (cChartText != null)
        {
            return cChartText.Descendants<C.StringPoint>().Single().InnerText;
        }

        // PieChart uses only one series for view.
        // However, it can have store multiple series data in the spreadsheet.
        if (this.Type == SCChartType.PieChart)
        {
            return ((SCSeriesCollection)this.SeriesCollection).First().Name;
        }

        return null;
    }

    private bool TryGetStaticTitle(C.ChartText chartText, out string? staticTitle)
    {
        staticTitle = null;
        if (this.Type == SCChartType.Combination)
        {
            staticTitle = chartText.RichText!.Descendants<A.Text>().Select(t => t.Text)
                .Aggregate((t1, t2) => t1 + t2);
            return true;
        }

        var rRich = chartText?.RichText;
        if (rRich != null)
        {
            staticTitle = rRich.Descendants<A.Text>().Select(t => t.Text).Aggregate((t1, t2) => t1 + t2);
            return true;
        }

        return false;
    }

    private SCLibraryCollection<double>? GetXValues()
    {
        var sdkXValues = this.firstSeries.Value?.GetFirstChild<C.XValues>();
        if (sdkXValues?.NumberReference == null)
        {
            return null;
        }

        IEnumerable<double> points =
            ChartReferencesParser.GetNumbersFromCacheOrWorkbook(sdkXValues.NumberReference, this);

        return new SCLibraryCollection<double>(points);
    }

    private OpenXmlElement? GetFirstSeries()
    {
        return this.cXCharts.First().ChildElements
            .FirstOrDefault(e => e.LocalName.Equals("ser", StringComparison.Ordinal));
    }
}