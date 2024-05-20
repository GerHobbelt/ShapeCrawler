﻿using FluentAssertions;
using ShapeCrawler.Tests.Unit.Helpers;
using NUnit.Framework;
using ShapeCrawler.Placeholders;
using ShapeCrawler.Tests.Unit.Helpers.Attributes;

namespace ShapeCrawler.Tests.Unit;

[TestFixture]
public class ShapeTests : SCTest
{
    [Test]
    public void AudioShape_BinaryData_returns_audio_bytes()
    {
        // Arrange
        var pptx = StreamOf("audio-case001.pptx");
        var pres = new Presentation(pptx);
        var audioShape = pres.Slides[0].Shapes.GetByName<IMediaShape>("Audio 1");

        // Act
        var bytes = audioShape.AsByteArray();

        // Assert
        bytes.Should().NotBeEmpty();
    }

    [Test]
    public void AudioShape_MIME_returns_MIME_type_of_audio_content()
    {
        // Arrange
        var pptxStream = StreamOf("audio-case001.pptx");
        var pres = new Presentation(pptxStream);
        var audioShape = pres.Slides[0].Shapes.GetByName<IMediaShape>("Audio 1");

        // Act
        var mime = audioShape.MIME;

        // Assert
        mime.Should().Be("audio/mpeg");
    }

    [Test]
    public void VideoShape_BinaryData_returns_video_bytes()
    {
        // Arrange
        var pptxStream = StreamOf("video-case001.pptx");
        var pres = new Presentation(pptxStream);
        var videoShape = pres.Slides[0].Shapes.GetByName<IMediaShape>("Video 1");

        // Act
        var bytes = videoShape.AsByteArray();

        // Assert
        bytes.Should().NotBeEmpty();
    }

    [Test]
    public void AudioShape_MIME_returns_MIME_type_of_video_content()
    {
        // Arrange
        var pptxStream = StreamOf("video-case001.pptx");
        var pres = new Presentation(pptxStream);
        var videoShape = pres.Slides[0].Shapes.GetByName<IMediaShape>("Video 1");

        // Act
        var mime = videoShape.MIME;

        // Assert
        mime.Should().Be("video/mp4");
    }

    [Test]
    public void PictureSetImage_ShouldNotImpactOtherPictureImage_WhenItsOriginImageIsShared()
    {
        // Arrange
        var pptx = StreamOf("009_table.pptx");
        var image = StreamOf("test-image-2.png");
        IPresentation presentation = new Presentation(pptx);
        IPicture picture5 = (IPicture)presentation.Slides[3].Shapes.First(sp => sp.Id == 5);
        IPicture picture6 = (IPicture)presentation.Slides[3].Shapes.First(sp => sp.Id == 6);
        int pic6LengthBefore = picture6.Image.AsByteArray().Length;
        MemoryStream modifiedPresentation = new();

        // Act
        picture5.Image.Update(image);

        // Assert
        int pic6LengthAfter = picture6.Image.AsByteArray().Length;
        pic6LengthAfter.Should().Be(pic6LengthBefore);

        presentation.SaveAs(modifiedPresentation);
        presentation = new Presentation(modifiedPresentation);
        picture6 = (IPicture)presentation.Slides[3].Shapes.First(sp => sp.Id == 6);
        pic6LengthBefore = picture6.Image.AsByteArray().Length;
        pic6LengthAfter.Should().Be(pic6LengthBefore);
    }

    [Test]
    public void Y_Getter_returns_y_coordinate_in_pixels()
    {
        // Arrange
        IShape shapeCase1 = new Presentation(StreamOf("006_1 slides.pptx")).Slides[0].Shapes.First(sp => sp.Id == 2);
        IShape shapeCase2 = new Presentation(StreamOf("018.pptx")).Slides[0].Shapes.First(sp => sp.Id == 7);
        IShape shapeCase3 = new Presentation(StreamOf("009_table.pptx")).Slides[1].Shapes.First(sp => sp.Id == 9);
        float verticalResoulution = Helpers.TestHelper.VerticalResolution;

        // Act
        int yCoordinate1 = shapeCase1.Y;
        int yCoordinate2 = shapeCase2.Y;
        int yCoordinate3 = shapeCase3.Y;

        // Assert
        yCoordinate1.Should().Be((int)(1122363 * verticalResoulution / 914400));
        yCoordinate2.Should().Be((int)(4 * verticalResoulution / 914400));
        yCoordinate3.Should().Be((int)(3463288 * verticalResoulution / 914400));
    }

    [Test]
    public void Id_returns_id()
    {
        // Arrange
        var pres = new Presentation(StreamOf("010.pptx"));
        var shape = pres.SlideMasters[0].Shapes.GetByName<IShape>("Date Placeholder 3");

        // Act
        var id = shape.Id;

        // Assert
        id.Should().Be(9);
    }

    [Test]
    public void Y_Setter_moves_the_Up_hand_grouped_shape_to_Up()
    {
        // Arrange
        var pptx = StreamOf("autoshape-grouping.pptx");
        var pres = new Presentation(pptx);
        var parentGroupShape = pres.Slides[0].Shapes.GetByName<IGroupShape>("Group 2");
        var groupedShape = parentGroupShape.Shapes.GetByName<IShape>("Shape 1");

        // Act
        groupedShape.Y = 359;

        // Assert
        groupedShape.Y.Should().Be(359);
        parentGroupShape.Y.Should().Be(359, "because the moved grouped shape was on the up-hand side");
        parentGroupShape.Height.Should().Be(172);
    }

    [Test]
    public void Y_Setter_moves_the_Down_hand_grouped_shape_to_Down()
    {
        // Arrange
        var pres = new Presentation(StreamOf("autoshape-grouping.pptx"));
        var groupShape = pres.Slides[0].Shapes.GetByName<IGroupShape>("Group 2");
        var groupedShape = groupShape.Shapes.GetByName<IShape>("Shape 2");

        // Act
        groupedShape.Y = 555;

        // Assert
        groupedShape.Y.Should().Be(555);
        groupShape.Height.Should().Be(178, "because it was 108 and the down-hand grouped shape got down on 71 pixels");
    }

    [Test]
    public void X_Setter_moves_the_Left_hand_grouped_shape_to_Left()
    {
        // Arrange
        var pres = new Presentation(StreamOf("autoshape-grouping.pptx"));
        var groupShape = pres.Slides[0].Shapes.GetByName<IGroupShape>("Group 2");
        var groupedShape = groupShape.Shapes.GetByName<IShape>("Shape 1");

        // Act
        groupedShape.X = 67;

        // Assert
        groupedShape.X.Should().Be(67);
        groupShape.X.Should().Be(67, "because the moved grouped shape was on the left-hand side");
        groupShape.Width.Should().Be(117);
    }

    [Test]
    public void X_Setter_moves_the_Right_hand_grouped_shape_to_Right()
    {
        // Arrange
        var pres = new Presentation(StreamOf("autoshape-grouping.pptx"));
        var groupShape = pres.Slides[0].Shapes.GetByName<IGroupShape>("Group 2");
        var groupedShape = groupShape.Shapes.GetByName<IShape>("Shape 1");

        // Act
        groupedShape.X = 91;

        // Assert
        groupedShape.X.Should().Be(91);
        groupShape.X.Should().Be(79,
            "because the X-coordinate of parent group shouldn't be changed when a grouped shape is moved to the right side");
        groupShape.Width.Should().Be(115);
    }

    [Test]
    public void Width_returns_shape_width_in_pixels()
    {
        // Arrange
        IShape shapeCase1 = new Presentation(StreamOf("006_1 slides.pptx")).Slides[0].Shapes.First(sp => sp.Id == 2);
        IGroupShape groupShape = (IGroupShape)new Presentation(StreamOf("009_table.pptx")).Slides[1].Shapes.First(sp => sp.Id == 7);
        IShape shapeCase2 = groupShape.Shapes.First(sp => sp.Id == 5);
        IShape shapeCase3 = new Presentation(StreamOf("009_table.pptx")).Slides[1].Shapes.First(sp => sp.Id == 9);

        // Act
        int width1 = shapeCase1.Width;
        int width2 = shapeCase2.Width;
        int width3 = shapeCase3.Width;

        // Assert
        (width1 * 914400 / TestHelper.HorizontalResolution).Should().Be(9144000);
        (width2 * 914400 / TestHelper.HorizontalResolution).Should().Be(1181100);
        (width3 * 914400 / TestHelper.HorizontalResolution).Should().Be(485775);
    }

    [Test]
    public void Height_returns_Grouped_Shape_height_in_pixels()
    {
        // Arrange
        var pptx = StreamOf("009_table.pptx");
        var pres = new Presentation(pptx);
        var groupShape = pres.Slides[1].Shapes.GetByName<IGroupShape>("Group 1");
        var groupedShape = groupShape.Shapes.GetByName<IShape>("Shape 2");

        // Act
        var height = groupedShape.Height;

        // Assert
        height.Should().Be(68);
    }

    [TestCase(2, Geometry.Rectangle)]
    [TestCase(3, Geometry.Ellipse)]
    public void GeometryType_returns_shape_geometry_type(int shapeId, Geometry expectedGeometryType)
    {
        // Arrange
        var presentation = new Presentation(StreamOf("021.pptx"));

        // Act
        var shape = presentation.Slides[3].Shapes.First(sp => sp.Id == shapeId);

        // Assert
        shape.GeometryType.Should().Be(expectedGeometryType);
    }
    
    [Test]
    public void Shape_IsNotGroupShape()
    {
        // Arrange
        IShape shape = new Presentation(StreamOf("006_1 slides.pptx")).Slides[0].Shapes.First(x => x.Id == 3);

        // Act-Assert
        shape.Should().NotBeOfType<IGroupShape>();
    }
    
    [Test]
    public void CustomData_ReturnsNull_WhenShapeHasNotCustomData()
    {
        // Arrange
        var shape = new Presentation(StreamOf("009_table.pptx")).Slides.First().Shapes.First();

        // Act
        var shapeCustomData = shape.CustomData;

        // Assert
        shapeCustomData.Should().BeNull();
    }

    [Test]
    public void CustomData_ReturnsCustomDataOfTheShape_WhenShapeWasAssignedSomeCustomData()
    {
        // Arrange
        const string customDataString = "Test custom data";
        var savedPreStream = new MemoryStream();
        var presentation = new Presentation(StreamOf("009_table.pptx"));
        var shape = presentation.Slides.First().Shapes.First();

        // Act
        shape.CustomData = customDataString;
        presentation.SaveAs(savedPreStream);

        // Assert
        presentation = new Presentation(savedPreStream);
        shape = presentation.Slides.First().Shapes.First();
        shape.CustomData.Should().Be(customDataString);
    }

    [Test]
    public void Name_ReturnsShapeNameString()
    {
        // Arrange
        IShape shape = new Presentation(StreamOf("009_table.pptx")).Slides[1].Shapes.First(sp => sp.Id == 8);

        // Act
        string shapeName = shape.Name;

        // Assert
        shapeName.Should().BeEquivalentTo("Object 2");
    }

    [TestCase(0, true)]
    [TestCase(1, false)]
    public void Hidden_ReturnsValueIndicatingWhetherShapeIsHiddenFromTheSlide(int shapeIndex, bool expectedHidden)
    {
        // Arrange
        var pptx = StreamOf("004.pptx");
        var pres = new Presentation(pptx);
        var shape = pres.Slides[0].Shapes[shapeIndex];

        // Act-Assert
        shape.Hidden.Should().Be(expectedHidden);
    }

    [TestCase("autoshape-case018_rotation.pptx", 1, "RotationTextBox", 325)]
    [TestCase("autoshape-case018_rotation.pptx", 2, "VerticalTextPH", 282)]
    [TestCase("autoshape-case018_rotation.pptx", 2, "NoRotationGroup", 0)]
    [TestCase("autoshape-case018_rotation.pptx", 2, "RotationGroup", 56)]
    public void Rotation_returns_shape_rotation_in_degrees(string presentationName, int slideNumber, string shapeName, double expectedAngle)
    {
        // Arrange
        var pres = new Presentation(StreamOf(presentationName));
        var shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);

        // Act
        var rotation = shape.Rotation;

        // Assert
        rotation.Should().BeApproximately(expectedAngle, 1);
    }
    
    [Test]
    [TestCase("001.pptx", 1, "TextBox 3")]
    [TestCase("001.pptx", 1, "Head 1")]
    [TestCase("autoshape-grouping.pptx", 1, "Group 1")]
    public void Y_Setter_sets_y_coordinate(string file, int slideNumber, string shapeName)
    {
        // Act
        var pres = new Presentation(StreamOf(file));
        var shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);
        shape.Y = 100;

        // Assert
        shape.Y.Should().Be(100);
        pres.Validate();
    }

    [Test]
    [TestCase("006_1 slides.pptx", 1, "Shape 1")]
    [TestCase("001.pptx", 1, "Head 1")]
    [TestCase("autoshape-grouping.pptx", 1, "Group 1")]
    [TestCase("table-case001.pptx", 1, "Table 1")]
    public void X_Setter_sets_x_coordinate(string file, int slideNumber, string shapeName)
    {
        // Arrange
        var pptx = StreamOf(file);
        var pres = new Presentation(pptx);
        var shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);
        var stream = new MemoryStream();

        // Act
        shape.X = 400;

        // Assert
        pres.SaveAs(stream);
        pres = new Presentation(stream);
        shape = pres.Slides[slideNumber-1].Shapes.GetByName<IShape>(shapeName);
        shape.X.Should().Be(400);
        pres.Validate();
    }
    
    [Test]
    [TestCase("006_1 slides.pptx", 1, "Shape 1")]
    public void Width_Setter_sets_width(string file, int slideNumber, string shapeName)
    {
        // Arrange
        var pptx = StreamOf(file);
        var pres = new Presentation(pptx);
        var stream = new MemoryStream();
        var shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);

        // Act
        shape.Width = 600;

        // Assert
        pres.SaveAs(stream);
        pres = new Presentation(stream);
        shape = pres.Slides[slideNumber - 1].Shapes.GetByName(shapeName);
        shape.Width.Should().Be(600);
        pres.Validate();
    }
    
    [Test]
    public void Remove_removes_shape()
    {
        // Arrange
        var pres = new Presentation(StreamOf("autoshape-grouping.pptx"));
        var shape = pres.Slides[0].Shapes.GetByName("TextBox 3");

        // Act
        shape.Remove();

        // Assert
        var act = () => pres.Slides[0].Shapes.GetByName("TextBox 3");
        act.Should().Throw<Exception>();
        pres.Validate();
    }
    
    [Test]
    [SlideShape("021.pptx", slideNumber: 4, shapeId: 2, expectedResult: PlaceholderType.Footer)]
    [SlideShape("008.pptx", 1, 3, PlaceholderType.DateAndTime)]
    [SlideShape("019.pptx", 1, 2, PlaceholderType.SlideNumber)]
    [SlideShape("013.pptx", 1, 281, PlaceholderType.Content)]
    [SlideShape("autoshape-case016.pptx", 1, "Content Placeholder 1", PlaceholderType.Content)]
    [SlideShape("autoshape-case016.pptx", 1, "Text Placeholder 1", PlaceholderType.Text)]
    [SlideShape("autoshape-case016.pptx", 1, "Picture Placeholder 1", PlaceholderType.Picture)]
    [SlideShape("autoshape-case016.pptx", 1, "Table Placeholder 1", PlaceholderType.Table)]
    [SlideShape("autoshape-case016.pptx", 1, "SmartArt Placeholder 1", PlaceholderType.SmartArt)]
    [SlideShape("autoshape-case016.pptx", 1, "Media Placeholder 1", PlaceholderType.Media)]
    [SlideShape("autoshape-case016.pptx", 1, "Online Image Placeholder 1", PlaceholderType.OnlineImage)]
    public void PlaceholderType_returns_placeholder_type(IShape shape, PlaceholderType expectedType)
    {
        // Act
        var placeholderType = shape.PlaceholderType;

        // Assert
        placeholderType.Should().Be(expectedType);
    }
}