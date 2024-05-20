using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ShapeCrawler.Shapes;
using ShapeCrawler.Tests.Shared;
using ShapeCrawler.Tests.Unit.Helpers;
using ShapeCrawler.Tests.Unit.Helpers.Attributes;
using Xunit;

// ReSharper disable SuggestVarOrType_SimpleTypes

namespace ShapeCrawler.Tests.Unit;

public class FontTests : SCTest
{
    [Test]
    public void EastAsianName_Setter_sets_font_for_the_east_asian_characters_in_New_Presentation()
    {
        // Arrange
        var pres = new SCPresentation();
        var slide = pres.Slides[0];
        slide.Shapes.AddRectangle(10, 10, 10, 10);
        var rectangle = slide.Shapes.Last();
        rectangle.TextFrame.Paragraphs[0].Portions.AddText("test");
        var font = rectangle.TextFrame!.Paragraphs[0].Portions[0].Font;

        // Act
        font.EastAsianName = "SimSun";

        // Assert
        font.EastAsianName.Should().Be("SimSun");
        pres.Validate();
    }
    
    [Test]
    public void Size_Getter_returns_font_size_of_non_first_portion()
    {
        // Arrange
        var pptx15 = StreamOf("015.pptx");
        var pres15 = new SCPresentation(pptx15);
        var font1 = pres15.Slides[0].Shapes.GetById<IShape>(5).TextFrame!.Paragraphs[0].Portions[2].Font;
        var font2 = new SCPresentation(StreamOf("009_table.pptx")).Slides[2].Shapes.GetById<IShape>(2).TextFrame!.Paragraphs[0].Portions[1].Font;

        // Act
        var fontSize1 = font1.Size;
        var fontSize2 = font2.Size;
        
        // Assert
        fontSize1.Should().Be(18);
        fontSize2.Should().Be(20);
    }

    [Test]
    public void Size_Getter_returns_Font_Size_of_Non_Placeholder_Table()
    {
        // Arrange
        var table = (ITable)new SCPresentation(StreamOf("009_table.pptx")).Slides[2].Shapes.First(sp => sp.Id == 3);
        var cellPortion = table.Rows[0].Cells[0].TextFrame.Paragraphs[0].Portions[0];

        // Act-Assert
        cellPortion.Font.Size.Should().Be(18);
    }

    [Test]
    public void IsBold_GetterReturnsTrue_WhenFontOfNonPlaceholderTextIsBold()
    {
        // Arrange
        var nonPlaceholderAutoShapeCase1 =
            (IShape)new SCPresentation(StreamOf("020.pptx")).Slides[0].Shapes.First(sp => sp.Id == 3);
        ITextPortionFont fontC1 = nonPlaceholderAutoShapeCase1.TextFrame.Paragraphs[0].Portions[0].Font;

        // Act-Assert
        fontC1.IsBold.Should().BeTrue();
    }

    [Test]
    public void IsBold_GetterReturnsTrue_WhenFontOfPlaceholderTextIsBold()
    {
        // Arrange
        var pres = new SCPresentation(StreamOf("020.pptx"));
        var placeholderAutoShape = pres.Slides[1].Shapes.GetById<IShape>(6);
        var portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        var isBold = portion.Font.IsBold;

        // Assert
        isBold.Should().BeTrue();
    }

    [Test]
    public void IsBold_GetterReturnsFalse_WhenFontOfNonPlaceholderTextIsNotBold()
    {
        // Arrange
        IShape nonPlaceholderAutoShape = (IShape)new SCPresentation(StreamOf("020.pptx")).Slides[0].Shapes.First(sp => sp.Id == 2);
        IParagraphPortion portion = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        bool isBold = portion.Font.IsBold;

        // Assert
        isBold.Should().BeFalse();
    }

    [Test]
    public void IsBold_GetterReturnsFalse_WhenFontOfPlaceholderTextIsNotBold()
    {
        // Arrange
        var placeholderAutoShape = (IShape)new SCPresentation(StreamOf("020.pptx")).Slides[2].Shapes.First(sp => sp.Id == 7);
        var portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        bool isBold = portion.Font.IsBold;

        // Assert
        isBold.Should().BeFalse();
    }

    [Test]
    public void IsBold_Setter_AddsBoldForNonPlaceholderTextFont()
    {
        // Arrange
        var mStream = new MemoryStream();
        var pres20 = new SCPresentation(StreamOf("020.pptx"));
        IPresentation presentation = pres20;
        IShape nonPlaceholderAutoShape = (IShape)presentation.Slides[0].Shapes.First(sp => sp.Id == 2);
        IParagraphPortion portion = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        portion.Font.IsBold = true;

        // Assert
        portion.Font.IsBold.Should().BeTrue();
        presentation.SaveAs(mStream);
        presentation = new SCPresentation(mStream);
        nonPlaceholderAutoShape = (IShape)presentation.Slides[0].Shapes.First(sp => sp.Id == 2);
        portion = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0];
        portion.Font.IsBold.Should().BeTrue();
    }

    [Test]
    public void IsItalic_GetterReturnsTrue_WhenFontOfNonPlaceholderTextIsItalic()
    {
        // Arrange
        IShape nonPlaceholderAutoShape = (IShape)new SCPresentation(StreamOf("020.pptx")).Slides[0].Shapes.First(sp => sp.Id == 3);
        ITextPortionFont font = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0].Font;

        // Act
        bool isItalicFont = font.IsItalic;

        // Assert
        isItalicFont.Should().BeTrue();
    }

    [Test]
    public void IsItalic_GetterReturnsTrue_WhenFontOfPlaceholderTextIsItalic()
    {
        // Arrange
        IShape placeholderAutoShape = (IShape)new SCPresentation(StreamOf("020.pptx")).Slides[2].Shapes.First(sp => sp.Id == 7);
        IParagraphPortion portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act-Assert
        portion.Font.IsItalic.Should().BeTrue();
    }

    [Test]
    public void IsItalic_Setter_SetsItalicFontForForNonPlaceholderText()
    {
        // Arrange
        var mStream = new MemoryStream();
        IPresentation presentation = new SCPresentation(StreamOf("020.pptx"));
        IShape nonPlaceholderAutoShape = (IShape)presentation.Slides[0].Shapes.First(sp => sp.Id == 2);
        IParagraphPortion portion = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        portion.Font.IsItalic = true;

        // Assert
        portion.Font.IsItalic.Should().BeTrue();
        presentation.SaveAs(mStream);
        presentation = new SCPresentation(mStream);
        nonPlaceholderAutoShape = (IShape)presentation.Slides[0].Shapes.First(sp => sp.Id == 2);
        portion = nonPlaceholderAutoShape.TextFrame.Paragraphs[0].Portions[0];
        portion.Font.IsItalic.Should().BeTrue();
    }

    [Test]
    public void IsItalic_SetterSetsNonItalicFontForPlaceholderText_WhenFalseValueIsPassed()
    {
        // Arrange
        var mStream = new MemoryStream();
        IPresentation presentation = new SCPresentation(StreamOf("020.pptx"));
        IShape placeholderAutoShape = (IShape)presentation.Slides[2].Shapes.First(sp => sp.Id == 7);
        IParagraphPortion portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        portion.Font.IsItalic = false;

        // Assert
        portion.Font.IsItalic.Should().BeFalse();
        presentation.SaveAs(mStream);

        presentation = new SCPresentation(mStream);
        placeholderAutoShape = (IShape)presentation.Slides[2].Shapes.First(sp => sp.Id == 7);
        portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];
        portion.Font.IsItalic.Should().BeFalse();
    }

    [Test]
    public void Underline_SetUnderlineFont_WhenValueEqualsSetPassed()
    {
        // Arrange
        var mStream = new MemoryStream();
        IPresentation presentation = new SCPresentation(StreamOf("020.pptx"));
        IShape placeholderAutoShape = (IShape)presentation.Slides[2].Shapes.First(sp => sp.Id == 7);
        IParagraphPortion portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];

        // Act
        portion.Font.Underline = DocumentFormat.OpenXml.Drawing.TextUnderlineValues.Single;

        // Assert
        portion.Font.Underline.Should().Be(DocumentFormat.OpenXml.Drawing.TextUnderlineValues.Single);
        presentation.SaveAs(mStream);

        presentation = new SCPresentation(mStream);
        placeholderAutoShape = (IShape)presentation.Slides[2].Shapes.First(sp => sp.Id == 7);
        portion = placeholderAutoShape.TextFrame.Paragraphs[0].Portions[0];
        portion.Font.Underline.Should().Be(DocumentFormat.OpenXml.Drawing.TextUnderlineValues.Single);
    }
    
    [Test]
    [TestCase("001.pptx", 1, "TextBox 3")]
    public void EastAsianName_Setter_sets_font_for_the_east_asian_characters(string file, int slideNumber, string shapeName)
    {
        // Arrange
        var pres = new SCPresentation(StreamOf(file));
        var shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);
        var font = shape.TextFrame.Paragraphs[0].Portions[0].Font;

        // Act
        font.EastAsianName = "SimSun";

        // Assert
        font.EastAsianName.Should().Be("SimSun");
        pres.Validate();
    }
}