﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using FluentAssertions;
using NUnit.Framework;
using ShapeCrawler.Tests.Shared;
using ShapeCrawler.Tests.Unit.Helpers;
using Xunit;
using Assert = Xunit.Assert;

// ReSharper disable TooManyDeclarations
// ReSharper disable InconsistentNaming
// ReSharper disable TooManyChainedReferences

namespace ShapeCrawler.Tests.Unit;

[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class ChartTests : SCTest
{
    [Test]
    public void XValues_ReturnsParticularXAxisValue_ViaItsCollectionIndexer()
    {
        // Arrange
        var pptx = StreamOf("024_chart.pptx");
        var pres = new Presentation(pptx);
        IChart chart = pres.Slides[1].Shapes.First(sp => sp.Id == 5) as IChart;

        // Act
        double xValue = chart.XValues[0];

        // Assert
        xValue.Should().Be(10);
        chart.HasXValues.Should().BeTrue();
    }

    [Test]
    public void HasXValues()
    {
        // Arrange
        var pptx = StreamOf("025_chart.pptx");
        var pres = new Presentation(pptx);
        ISlide slide1 = pres.Slides[0];
        ISlide slide2 = pres.Slides[1];
        IChart chart8 = slide1.Shapes.First(x => x.Id == 8) as IChart;
        IChart chart11 = slide2.Shapes.First(x => x.Id == 11) as IChart;

        // Act
        var chart8HasXValues = chart8.HasXValues;
        var chart11HasXValues = chart11.HasXValues;

        // Assert
        Assert.False(chart8HasXValues);
        Assert.False(chart11HasXValues);
    }

    [Test]
    public void HasCategories_ReturnsFalse_WhenAChartHasNotCategories()
    {
        // Arrange
        IChart chart = (IChart)new Presentation(StreamOf("021.pptx")).Slides[2].Shapes.First(sp => sp.Id == 4);

        // Act
        bool hasChartCategories = chart.HasCategories;

        // Assert
        hasChartCategories.Should().BeFalse();
    }

    [Test]
    public void TitleAndHasTitle_ReturnChartTitleStringAndFlagIndicatingWhetherChartHasATitle()
    {
        // Arrange
        var pres13 = new Presentation(StreamOf("013.pptx"));
        var pres19 = new Presentation(StreamOf("019.pptx"));
        IChart chartCase1 = (IChart)new Presentation(StreamOf("018.pptx")).Slides[0].Shapes.First(sp => sp.Id == 6);
        IChart chartCase2 = (IChart)new Presentation(StreamOf("025_chart.pptx")).Slides[0].Shapes.First(sp => sp.Id == 7);
        IChart chartCase3 = (IChart)pres13.Slides[0].Shapes.First(sp => sp.Id == 5);
        IChart chartCase4 = (IChart)pres13.Slides[0].Shapes.First(sp => sp.Id == 4);
        IChart chartCase5 = (IChart)pres19.Slides[0].Shapes.First(sp => sp.Id == 4);
        IChart chartCase6 = (IChart)pres13.Slides[0].Shapes.First(sp => sp.Id == 6);
        IChart chartCase7 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[2].Shapes.First(sp => sp.Id == 7);
        IChart chartCase8 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[2].Shapes.First(sp => sp.Id == 6);
        IChart chartCase9 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[4].Shapes.First(sp => sp.Id == 6);
        IChart chartCase10 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[4].Shapes.First(sp => sp.Id == 3);
        IChart chartCase11 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[4].Shapes.First(sp => sp.Id == 5);
            
        // Act
        string charTitleCase1 = chartCase1.Title;
        string charTitleCase2 = chartCase2.Title;
        string charTitleCase3 = chartCase3.Title;
        string charTitleCase5 = chartCase5.Title;
        string charTitleCase7 = chartCase7.Title;
        string charTitleCase8 = chartCase8.Title;
        string charTitleCase9 = chartCase9.Title;
        string charTitleCase10 = chartCase10.Title;
        string charTitleCase11 = chartCase11.Title;
        bool hasTitleCase4 = chartCase4.HasTitle;
        bool hasTitleCase6 = chartCase6.HasTitle;

        // Assert
        charTitleCase1.Should().BeEquivalentTo("Test title");
        charTitleCase2.Should().BeEquivalentTo("Series 1_id7");
        charTitleCase3.Should().BeEquivalentTo("Title text");
        charTitleCase5.Should().BeEquivalentTo("Test title");
        charTitleCase7.Should().BeEquivalentTo("Sales");
        charTitleCase8.Should().BeEquivalentTo("Sales2");
        charTitleCase9.Should().BeEquivalentTo("Sales3");
        charTitleCase10.Should().BeEquivalentTo("Sales4");
        charTitleCase11.Should().BeEquivalentTo("Sales5");
        hasTitleCase4.Should().BeFalse();
        hasTitleCase6.Should().BeFalse();
    }
        
    [Test]
    public void SeriesCollection_Series_Points_returns_chart_point_collection()
    {
        // Arrange
        var pptxStream = StreamOf("charts-case001.pptx");
        var presentation = new Presentation(pptxStream);
        var chart = (IChart) presentation.Slides[0].Shapes.First(shape => shape.Name == "chart");
        var series = chart.SeriesList[0]; 
            
        // Act
        var chartPoints = series.Points;
            
        // Assert
        chartPoints.Should().NotBeEmpty();
    }
    
    [TestCase("charts_bar-chart.pptx", "Bar Chart 1")]
    [TestCase("019.pptx", "Pie Chart 1")]
    public void SeriesCollection_RemoveAt_removes_series_by_index(string pptxFile, string chartName)
    {
        // Arrange
        var pptxStream = StreamOf(pptxFile);
        var pres = new Presentation(pptxStream);
        var chart = pres.Slides[0].Shapes.GetByName<IChart>(chartName);
        var expectedSeriesCount = chart.SeriesList.Count - 1; 
            
        // Act
        chart.SeriesList.RemoveAt(0);

        // Assert
        chart.SeriesList.Count.Should().Be(expectedSeriesCount);
    }
    
    [Test]
    public void CategoryName_GetterReturnsChartCategoryName()
    {
        // Arrange
        IChart chartCase1 = (IChart)new Presentation(StreamOf("025_chart.pptx")).Slides[0].Shapes.First(sp => sp.Id == 4);
        IChart chartCase3 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[2].Shapes.First(sp => sp.Id == 7);

        // Act-Assert
        chartCase1.Categories[0].Name.Should().BeEquivalentTo("Dresses");
        chartCase3.Categories[0].Name.Should().BeEquivalentTo("Q1");
        chartCase3.Categories[1].Name.Should().BeEquivalentTo("Q2");
        chartCase3.Categories[2].Name.Should().BeEquivalentTo("Q3");
        chartCase3.Categories[3].Name.Should().BeEquivalentTo("Q4");
    }
        
    [Test]
    public void Category_Name_Getter_returns_category_name_for_chart_from_collection_of_Combination_chart()
    {
        // Arrange
        var comboChart = (IChart)new Presentation(StreamOf("021.pptx")).Slides[0].Shapes.First(sp => sp.Id == 4);

        // Act-Assert
        comboChart.Categories[0].Name.Should().BeEquivalentTo("2015");
    }

    [Test]
    public void CategoryName_GetterReturnsChartCategoryName_OfMultiCategoryChart()
    {
        // Arrange
        var chartCase1 = (IChart)new Presentation(StreamOf("025_chart.pptx")).Slides[0].Shapes.First(sp => sp.Id == 4);

        // Act-Assert
        chartCase1.Categories[0].MainCategory.Name.Should().BeEquivalentTo("Clothing");
    }

    [Test]
    public void Category_Name_Setter_updates_category_name_in_non_multi_category_pie_chart()
    {
        // Arrange
        var pres = new Presentation(StreamOf("025_chart.pptx"));
        var mStream = new MemoryStream();
        var pieChart = pres.Slides[0].Shapes.GetById<IChart>(7);

        // Act
        pieChart.Categories[0].Name = "Category 1_new";

        // Assert
        pieChart.Categories[0].Name.Should().Be("Category 1_new");
        pres.SaveAs(mStream);
        pres = new Presentation(mStream);
        pieChart = pres.Slides[0].Shapes.GetById<IChart>(7);
        pieChart.Categories[0].Name.Should().Be("Category 1_new");
    }

    [Test, Ignore("ClosedXML dependency must be removed")]
    public void Category_Name_Setter_updates_value_of_Excel_cell()
    {
        // Arrange
        var pres = new Presentation(StreamOf("025_chart.pptx"));
        var lineChart = pres.Slides[3].Shapes.GetById<IChart>(13);
        var category = lineChart.Categories[0]; 

        // Act
        category.Name = "Category 1_new";

        // Assert
        var mStream = new MemoryStream(lineChart.BookByteArray());
        var workbook = new XLWorkbook(mStream);
        var cellValue = workbook.Worksheets.First().Cell("A2").Value.ToString();
        cellValue.Should().BeEquivalentTo("Category 1_new");
    }

    [Test, Ignore("On Hold")]
    public void CategoryName_SetterChangeName_OfSecondaryCategoryInMultiCategoryBarChart()
    {
        // Arrange
        var pptxStream = StreamOf("025_chart.pptx");
        var pres = new Presentation(pptxStream);
        var barChart = (IChart)pres.Slides[0].Shapes.First(sp => sp.Id == 4);
        const string newCategoryName = "Clothing_new";

        // Act
        barChart.Categories[0].Name = newCategoryName;

        // Assert
        barChart.Categories[0].Name.Should().Be(newCategoryName);

        pres.Save();
        pres = new Presentation(pptxStream);
        barChart = (IChart)pres.Slides[0].Shapes.First(sp => sp.Id == 4);
        barChart.Categories[0].Name.Should().Be(newCategoryName);
    }

    [Test]
    public void SeriesType_ReturnsChartTypeOfTheSeries()
    {
        // Arrange
        IChart chart = (IChart)new Presentation(StreamOf("021.pptx")).Slides[0].Shapes.First(sp => sp.Id == 3);
        ISeries series2 = chart.SeriesList[1];
        ISeries series3 = chart.SeriesList[2];

        // Act
        ChartType seriesChartType2 = series2.Type;
        ChartType seriesChartType3 = series3.Type;

        // Assert
        seriesChartType2.Should().Be(ChartType.BarChart);
        seriesChartType3.Should().Be(ChartType.ScatterChart);
    }

    [Test]
    public void Series_Name_returns_chart_series_name()
    {
        // Arrange
        IChart chart = (IChart)new Presentation(StreamOf("025_chart.pptx")).Slides[0].Shapes.First(sp => sp.Id == 5);

        // Act
        string seriesNameCase1 = chart.SeriesList[0].Name;
        string seriesNameCase2 = chart.SeriesList[2].Name;

        // Assert
        seriesNameCase1.Should().BeEquivalentTo("Ряд 1");
        seriesNameCase2.Should().BeEquivalentTo("Ряд 3");
    }

    [Test]
    public void Type_ReturnsChartType()
    {
        // Arrange
        var pres13 = new Presentation(StreamOf("013.pptx"));
        IChart chartCase1 = (IChart)new Presentation(StreamOf("021.pptx")).Slides[1].Shapes.First(sp => sp.Id == 3);
        IChart chartCase2 = (IChart)new Presentation(StreamOf("021.pptx")).Slides[2].Shapes.First(sp => sp.Id == 4);
        IChart chartCase3 = (IChart)pres13.Slides[0].Shapes.First(sp => sp.Id == 5);
        IChart chartCase4 = (IChart)new Presentation(StreamOf("009_table.pptx")).Slides[2].Shapes.First(sp => sp.Id == 7);

        // Act
        ChartType chartTypeCase1 = chartCase1.Type;
        ChartType chartTypeCase2 = chartCase2.Type;
        ChartType chartTypeCase3 = chartCase3.Type;
        ChartType chartTypeCase4 = chartCase4.Type;

        // Assert
        chartTypeCase1.Should().Be(ChartType.BubbleChart);
        chartTypeCase2.Should().Be(ChartType.ScatterChart);
        chartTypeCase3.Should().Be(ChartType.Combination);
        chartTypeCase4.Should().Be(ChartType.PieChart);
    }

    [Test]
    public void GeometryType_Getter_returns_rectangle()
    {
        // Arrange
        IChart chart = (IChart)new Presentation(StreamOf("018.pptx")).Slides[0].Shapes.First(sp => sp.Id == 6);

        // Act-Assert
        chart.GeometryType.Should().Be(Geometry.Rectangle);
    }

    [Test]
    public void Axes_ValueAxis_Minimum_Getter()
    {
        // Arrange
        var pptx = TestHelper.GetStream("charts_bar-chart.pptx");
        var pres = new Presentation(pptx);
        var barChart = pres.Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        
        // Act
        var minimum = barChart.Axes.ValueAxis.Minimum;
        
        // Assert
        minimum.Should().Be(0);
    }
    
    [Test]
    public void Axes_ValueAxis_Minimum_Setter()
    {
        // Arrange
        var pptx = TestHelper.GetStream("charts_bar-chart.pptx");
        var pres = new Presentation(pptx);
        var barChart = pres.Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        var mStream = new MemoryStream();
        
        // Act
        barChart.Axes.ValueAxis.Minimum = 1;

        // Assert
        pres.SaveAs(mStream);
        barChart = new Presentation(mStream).Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        barChart.Axes.ValueAxis.Minimum.Should().Be(1);
        pres.Validate();
    }
    
    [Test]
    public void Axes_ValueAxis_Maximum_Setter()
    {
        // Arrange
        var pptx = TestHelper.GetStream("charts_bar-chart.pptx");
        var pres = new Presentation(pptx);
        var barChart = pres.Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        
        // Act
        barChart.Axes.ValueAxis!.Maximum = 7;

        // Assert
        barChart.Axes.ValueAxis.Maximum.Should().Be(7);
        pres.Validate();
    }
    
    [Test]
    public void Axes_ValueAxis_Maximum_Getter_returns_default_6()
    {
        // Arrange
        var pptx = TestHelper.GetStream("charts_bar-chart.pptx");
        var pres = new Presentation(pptx);
        var barChart = pres.Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        
        // Act
        var maximum = barChart.Axes.ValueAxis.Maximum;
        
        // Assert
        maximum.Should().Be(6);
    }
    
    [Test]
    [SlideShape("013.pptx", slideNumber:1, shapeId: 5, expectedResult: 3)]
    [SlideShape("009_table.pptx", slideNumber:3, shapeId: 7, expectedResult: 1)]
    public void SeriesCollection_Count_returns_number_of_series(IShape shape, int expectedSeriesCount)
    {
        // Act
        var chart = (IChart)shape;
        int seriesCount = chart.SeriesList.Count;

        // Assert
        seriesCount.Should().Be(expectedSeriesCount);
    }
}