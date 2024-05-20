﻿using ShapeCrawler.Texts;
using A = DocumentFormat.OpenXml.Drawing;

// ReSharper disable CheckNamespace
namespace ShapeCrawler;

/// <summary>
///     Represents a slide number font.
/// </summary>
public interface ISlideNumberFont : IFont
{
    /// <summary>
    ///     Gets or sets color.
    /// </summary>
    Color Color { get; set; }
}

internal sealed class SlideNumberFont : ISlideNumberFont
{
    private readonly A.DefaultRunProperties aDefaultRunProperties;
    private readonly MasterSlideNumberSize masterSlideNumberSize;

    internal SlideNumberFont(A.DefaultRunProperties aDefaultRunProperties)
    {
        this.aDefaultRunProperties = aDefaultRunProperties;
        this.masterSlideNumberSize = new MasterSlideNumberSize(aDefaultRunProperties);
    }

    public Color Color
    {
        get => this.ParseColor();
        set => this.UpdateColor(value);
    }

    public int Size
    {
        get => this.masterSlideNumberSize.Size();
        set => this.masterSlideNumberSize.Update(value);
    }

    private void UpdateColor(Color color)
    {
        var solidFill = this.aDefaultRunProperties.GetFirstChild<A.SolidFill>();
        solidFill?.Remove();

        var rgbColorModelHex = new A.RgbColorModelHex { Val = color.ToString() };
        solidFill = new A.SolidFill(rgbColorModelHex);
        
        this.aDefaultRunProperties.Append(solidFill);
    }

    private Color ParseColor()
    {
        var hex = this.aDefaultRunProperties.GetFirstChild<A.SolidFill>() !.RgbColorModelHex!.Val!.Value!;

        return Color.FromHex(hex);
    }
}