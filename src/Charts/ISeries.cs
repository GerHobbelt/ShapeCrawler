﻿using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Charts;
using C = DocumentFormat.OpenXml.Drawing.Charts;

#pragma warning disable IDE0130
namespace ShapeCrawler;
#pragma warning restore IDE0130

/// <summary>
///     Represents a chart series.
/// </summary>
public interface ISeries
{
    /// <summary>
    ///     Gets series name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets chart type.
    /// </summary>
    ChartType Type { get; }

    /// <summary>
    ///     Gets the collection of chart points.
    /// </summary>
    IReadOnlyList<IChartPoint> Points { get; }

    /// <summary>
    ///     Gets a value indicating whether chart has name.
    /// </summary>
    bool HasName { get; }
}

internal sealed class Series : ISeries
{
    private readonly ChartPart chartPart;
    private readonly OpenXmlElement cSer;

    internal Series(ChartPart sdkChartPart, OpenXmlElement cSer, ChartType type)
    {
        this.chartPart = sdkChartPart;
        this.cSer = cSer;
        this.Type = type;
        this.Points = new ChartPoints(this.chartPart, this.cSer);
    }

    public ChartType Type { get; }

    public IReadOnlyList<IChartPoint> Points { get; }

    public bool HasName => this.cSer.GetFirstChild<C.SeriesText>()?.StringReference != null;

    public string Name => this.ParseName();

    private string ParseName()
    {
        var cStrRef = this.cSer.GetFirstChild<C.SeriesText>()?.StringReference ?? throw new SCException($"Series does not have name. Use {nameof(this.HasName)} property to check if series has name.");
        var fromCache = cStrRef.StringCache?.GetFirstChild<C.StringPoint>() !.Single().InnerText;

        return fromCache ?? new Workbook(this.chartPart.EmbeddedPackagePart!).FormulaValues(cStrRef.Formula!.Text)[0].ToString();
    }
}