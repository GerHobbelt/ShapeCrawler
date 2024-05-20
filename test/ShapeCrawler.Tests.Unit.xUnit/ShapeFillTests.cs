﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using ShapeCrawler.Shapes;
using ShapeCrawler.Tests.Shared;
using ShapeCrawler.Tests.Unit.Helpers;
using ShapeCrawler.Tests.Unit.Helpers.Attributes;
using Xunit;

// ReSharper disable TooManyDeclarations
// ReSharper disable InconsistentNaming
// ReSharper disable TooManyChainedReferences

namespace ShapeCrawler.Tests.Unit;

public class ShapeFillTests : SCTest
{
    [Theory]
    [SlideShapeData("008.pptx", slideNumber: 1, shapeName: "AutoShape 1")]
    [SlideShapeData("autoshape-case009.pptx", slideNumber: 1, shapeName: "AutoShape 1")]
    [LayoutShapeData("autoshape-case003.pptx", slideNumber: 1, shapeName: "AutoShape 1")]
    [MasterShapeData("autoshape-case003.pptx", shapeName: "AutoShape 1")]
    public void SetPicture_updates_fill_with_specified_picture_image_When_shape_is_Not_filled(IShape shape)
    {
        // Arrange
        var autoShape = (IShape)shape;
        var fill = autoShape.Fill;
        var imageStream = StreamOf("test-image-1.png");

        // Act
        fill.SetPicture(imageStream);

        // Assert
        var pictureBytes = fill.Picture!.AsByteArray();
        var imageBytes = imageStream.ToArray();
        pictureBytes.SequenceEqual(imageBytes).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TestCasesFillType))]
    public void Type_returns_fill_type(IShape shape, FillType expectedFill)
    {
        // Act
        var fillType = shape.Fill.Type;

        // Assert
        fillType.Should().Be(expectedFill);
    }

    public static IEnumerable<object[]> TestCasesFillType()
    {
        var pptxStream = StreamOf("009_table.pptx");
        var pres = new Presentation(pptxStream);
        
        var withNoFill = pres.Slides[1].Shapes.GetById<IShape>(6);
        yield return new object[] { withNoFill, FillType.NoFill };
        
        var withSolid = pres.Slides[1].Shapes.GetById<IShape>(2);
        yield return new object[] { withSolid, FillType.Solid };
        
        var withGradient = pres.Slides[1].Shapes.GetByName<IShape>("AutoShape 1");
        yield return new object[] { withGradient, FillType.Gradient };
        
        var withPicture = pres.Slides[2].Shapes.GetById<IShape>(4);
        yield return new object[] { withPicture, FillType.Picture };
        
        var withPattern = pres.Slides[1].Shapes.GetByName<IShape>("AutoShape 2");
        yield return new object[] { withPattern, FillType.Pattern };
        
        pres = new Presentation(StreamOf("autoshape-case003.pptx"));
        var withSlideBg = pres.Slides[0].Shapes.GetByName<IShape>("AutoShape 1");
        yield return new object[] { withSlideBg, FillType.SlideBackground };
    }
}