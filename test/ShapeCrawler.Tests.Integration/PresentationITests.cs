using System;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using ShapeCrawler.Logger;
using ShapeCrawler.Tests.Shared;
using ShapeCrawler.Tests.Unit.Helpers;
using Xunit;

namespace ShapeCrawler.Tests.Integration;

public class PresentationITests : SCTest
{
    [Fact]
    public void Open_doesnt_create_log_file_When_logger_is_off()
    {
        // Arrange
        var pptxStream = StreamOf("autoshape-case001.pptx");

        // Act
        SCSettings.CanCollectLogs = false;
        new SCPresentation(pptxStream);

        // Assert
        var logPath = Path.Combine(Path.GetTempPath(), "sc-log.json");
        File.Exists(logPath).Should().BeFalse();
    }
    
    [Fact(Skip = "Wait deploy statistics service")]
    public void Open_create_log_file()
    {
        // Arrange
        var logPath = Path.Combine(Path.GetTempPath(), "sc-log.json");
        var pptxStream = TestHelperShared.GetStream("autoshape-case001.pptx");

        // Act
        new SCPresentation(pptxStream);

        // Assert
        File.Exists(logPath).Should().BeTrue();
        var json = File.OpenRead(logPath);
        var log = JsonSerializer.Deserialize<SCLog>(json)!;
        var sendDate = (DateTime)log.SentDate;
        sendDate.Day.Should().Be(DateTime.UtcNow.Day);
        
        // Clean
        File.Delete(logPath);
    }

    [Fact]
    public void Open_should_not_throw_exception()
    {
        // Arrange
        var originFilePath = Path.GetTempFileName();
        var savedAsFilePath = Path.GetTempFileName();
        var pptx = StreamOf("001.pptx");
        File.WriteAllBytes(originFilePath, pptx.ToArray());
        var pres = new SCPresentation(originFilePath);
        pres.SaveAs(savedAsFilePath);

        // Act-Assert
        new SCPresentation(originFilePath);
        
        // Clean up
        File.Delete(originFilePath);
        File.Delete(savedAsFilePath);
    }
    
    [Fact]
    public void SaveAs_should_not_change_the_Original_Path_when_it_is_saved_to_New_Stream()
    {
        // Arrange
        var originalPath = GetTestPath("001.pptx");
        var pres = new SCPresentation(originalPath);
        var textBox = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var originalText = textBox!.Text;
        var newStream = new MemoryStream();
        textBox.Text = originalText + "modified";

        // Act
        pres.SaveAs(newStream);

        // Assert
        pres = new SCPresentation(originalPath);
        textBox = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var autoShapeText = textBox!.Text;
        autoShapeText.Should().BeEquivalentTo(originalText);
            
        // Clean
        File.Delete(originalPath);
    }
           
    [Fact]
    public void SaveAs_should_not_change_the_Original_Stream_when_it_is_saved_to_New_Path()
    {
        // Arrange
        var originalFile = GetTestPath("001.pptx");
        var pres = new SCPresentation(originalFile);
        var textBox = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var originalText = textBox!.Text;
        var newPath = Path.GetTempFileName();
        textBox.Text = originalText + "modified";

        // Act
        pres.SaveAs(newPath);

        // Assert
        pres = new SCPresentation(originalFile);
        textBox = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var autoShapeText = textBox!.Text;
        autoShapeText.Should().BeEquivalentTo(originalText);
            
        // Clean
        File.Delete(newPath);
    }
    
    [Test]
    public void SaveAs_should_not_change_the_Original_Path_when_it_is_saved_to_New_Path()
    {
        // Arrange
        var originalPath = GetTestPath("001.pptx");
        var pres = new SCPresentation(originalPath);
        var textFrame = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var originalText = textFrame!.Text;
        var newPath = Path.GetTempFileName();
        textFrame.Text = originalText + "modified";

        // Act
        pres.SaveAs(newPath);

        // Assert
        pres = new SCPresentation(originalPath);
        textFrame = pres.Slides[0].Shapes.GetByName<IShape>("TextBox 3").TextFrame;
        var autoShapeText = textFrame!.Text; 
        autoShapeText.Should().BeEquivalentTo(originalText);
            
        // Clean
        File.Delete(originalPath);
        File.Delete(newPath);
    }
}