﻿using AngleSharp.Html.Dom;
using OneOf;
using ShapeCrawler.Shapes;
using P = DocumentFormat.OpenXml.Presentation;

// ReSharper disable CheckNamespace
namespace ShapeCrawler;

/// <summary>
///     Represents a Rectangle shape.
/// </summary>
public interface IRectangle : IAutoShape
{
}

internal sealed class SCRectangle : SCAutoShape, IRectangle
{
    internal SCRectangle(
        P.Shape pShape,
        OneOf<SCSlide, SCSlideLayout, SCSlideMaster> slideOf,
        OneOf<ShapeCollection, SCGroupShape> shapeCollectionOf)
        : base(pShape, slideOf, shapeCollectionOf)
    {
    }

    internal override IHtmlElement ToHtmlElement()
    {
        throw new System.NotImplementedException();
    }
}