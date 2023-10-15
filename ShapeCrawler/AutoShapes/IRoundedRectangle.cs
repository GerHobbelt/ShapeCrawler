﻿using ShapeCrawler.Collections;
using ShapeCrawler.Shapes;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.AutoShapes;

/// <summary>
///     Represents a Rounded Rectangle shape.
/// </summary>
public interface IRoundedRectangle : IAutoShape
{
}

internal sealed class SCRoundedRectangle : SCAutoShape, IRoundedRectangle
{
    public SCRoundedRectangle(AutoShapeCollection autoShapeCollection, P.Shape pShape, SCGroupShape? groupShape) 
        : base(autoShapeCollection.ParentShapeCollection.ParentSlideObject, pShape, groupShape)
    {
    }
}