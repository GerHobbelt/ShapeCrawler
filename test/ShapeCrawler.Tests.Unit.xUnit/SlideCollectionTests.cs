﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ShapeCrawler.Tests.Shared;
using ShapeCrawler.Tests.Unit.Helpers;
using Xunit;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Tests.Unit;

[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class SlideCollectionTests : SCTest
{
    [Fact]
    public void Count_returns_one_When_presentation_contains_one_slide()
    {
        // Act
        var pptx17 = StreamOf("017.pptx");
        var pres17 = new SCPresentation(pptx17);        
        var pptx16 = StreamOf("016.pptx");
        var pres16 = new SCPresentation(pptx16);
        var numberSlidesCase1 = pres17.Slides.Count;
        var numberSlidesCase2 = pres16.Slides.Count;

        // Assert
        numberSlidesCase1.Should().Be(1);
        numberSlidesCase2.Should().Be(1);
    }

    [Fact]
    public void Add_adds_external_slide()
    {
        // Arrange
        var sourceSlide = new SCPresentation(StreamOf("001.pptx")).Slides[0];
        var pptx = StreamOf("002.pptx");
        var destPre = new SCPresentation(pptx);
        var originSlidesCount = destPre.Slides.Count;
        var expectedSlidesCount = ++originSlidesCount;
        MemoryStream savedPre = new ();

        // Act
        destPre.Slides.Add(sourceSlide);

        // Assert
        destPre.Slides.Count.Should().Be(expectedSlidesCount, "because the new slide has been added");

        destPre.SaveAs(savedPre);
        destPre = new SCPresentation(savedPre);
        destPre.Slides.Count.Should().Be(expectedSlidesCount, "because the new slide has been added");
    }
    
    [Fact]
    public void Add_adds_slide_from_the_Same_presentation()
    {
        // Arrange
        var pptxStream = StreamOf("charts-case003.pptx");
        var pres = new SCPresentation(pptxStream);
        var expectedSlidesCount = pres.Slides.Count + 1;
        var slideCollection = pres.Slides;
        var addingSlide = slideCollection[0];

        // Act
        pres.Slides.Add(addingSlide);

        // Assert
        pres.Slides.Count.Should().Be(expectedSlidesCount);
    }
    
    [Fact]
    public void Add_adds_slide_After_updating_chart_series()
    {
        // Arrange
        var pptx = TestHelper.GetStream("charts_bar-chart.pptx");
        var pres = new SCPresentation(pptx);
        var chart = pres.Slides[0].Shapes.GetByName<IChart>("Bar Chart 1");
        var expectedSlidesCount = pres.Slides.Count + 1;

        // Act
        chart.SeriesList[0].Points[0].Value = 1;
        pres.Slides.Add(pres.Slides[0]);
        
        // Assert
        pres.Slides.Count.Should().Be(expectedSlidesCount);
    }

    [Fact]
    public void Add_add_adds_New_slide()
    {
        // Arrange
        var pptx = StreamOf("autoshape-grouping.pptx");
        var pres = new SCPresentation(pptx);
        var layout = pres.SlideMasters[0].SlideLayouts[0]; 
        var slides = pres.Slides;

        // Act
        slides.AddEmptySlide(layout);

        // Assert
        var addedSlide = slides.Last();
        addedSlide.Should().NotBeNull();
        pres.Validate();
    }

    [Fact]
    public void AddEmptySlide_adds_slide_from_layout()
    {
        // Arrange
        var pres = new SCPresentation(StreamOf("017.pptx"));
        var titleAndContentLayout = pres.SlideMasters[0].SlideLayouts[0];

        // Act
        pres.Slides.AddEmptySlide(SCSlideLayoutType.Title);

        // Assert
        var addedSlide = pres.Slides.Last();
        titleAndContentLayout.Type.Should().Be(SCSlideLayoutType.Title);
        addedSlide.Should().NotBeNull();
        titleAndContentLayout.Shapes.Select(s => s.Name).Should().BeSubsetOf(addedSlide.Shapes.Select(s => s.Name));
    }

    [Fact]
    public void Slides_Insert_inserts_slide_at_the_specified_position()
    {
        // Arrange
        var pptx = StreamOf("001.pptx");
        var sourceSlide = new SCPresentation(pptx).Slides[0];
        var sourceSlideId = Guid.NewGuid().ToString();
        sourceSlide.CustomData = sourceSlideId;
        pptx = StreamOf("002.pptx");
        var destPre = new SCPresentation(pptx);

        // Act
        destPre.Slides.Insert(2, sourceSlide);

        // Assert
        destPre.Slides[1].CustomData.Should().Be(sourceSlideId);
    }

    [Xunit.Theory]
    [MemberData(nameof(TestCasesSlidesRemove))]
    public void Slides_Remove_removes_slide(string file, int expectedSlidesCount)
    {
        // Arrange
        var pptx = StreamOf(file);
        var pres = new SCPresentation(pptx);
        var removingSlide = pres.Slides[0];
        var mStream = new MemoryStream();

        // Act
        pres.Slides.Remove(removingSlide);

        // Assert
        pres.Slides.Should().HaveCount(expectedSlidesCount);

        pres.SaveAs(mStream);
        pres = new SCPresentation(mStream);
        pres.Slides.Should().HaveCount(expectedSlidesCount);
    }
        
    public static IEnumerable<object[]> TestCasesSlidesRemove()
    {
        yield return new object[] {"007_2 slides.pptx", 1};
        yield return new object[] {"006_1 slides.pptx", 0};
    }
        
    [Fact]
    public void Slides_Remove_removes_slide_from_section()
    {
        // Arrange
        var pptxStream = StreamOf("autoshape-case017_slide-number.pptx");
        var pres = new SCPresentation(pptxStream);
        var sectionSlides = pres.Sections[0].Slides;
        var removingSlide = sectionSlides[0];
        var mStream = new MemoryStream();

        // Act
        pres.Slides.Remove(removingSlide);

        // Assert
        sectionSlides.Count.Should().Be(0);

        pres.SaveAs(mStream);
        pres = new SCPresentation(mStream);
        sectionSlides = pres.Sections[0].Slides;
        sectionSlides.Count.Should().Be(0);
    }
}