﻿using DocumentFormat.OpenXml;
using OneOf;
using ShapeCrawler.Shared;
using ShapeCrawler.SlideMasters;
using SkiaSharp;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

// ReSharper disable PossibleMultipleEnumeration
namespace ShapeCrawler.Shapes;

internal sealed class SCGroupShape : SCShape, IGroupShape
{
    private readonly P.GroupShape pGroupShape;
    private readonly OneOf<SCSlide, SCSlideLayout, SCSlideMaster> parentSlideObject;
    
    internal SCGroupShape(
        P.GroupShape pGroupShape, 
        OneOf<SCSlide, SCSlideLayout, SCSlideMaster> parentSlideObject,
        OneOf<ShapeCollection, SCGroupShape> parentShapeCollection)
        : base(pGroupShape, parentSlideObject, parentShapeCollection)
    {
        this.pGroupShape = pGroupShape;
        this.parentSlideObject = parentSlideObject;
    }

    public IGroupedShapeCollection Shapes => GroupedShapeCollection.Create(this.pGroupShape, this.parentSlideObject, this);

    public override SCShapeType ShapeType => SCShapeType.GroupShape;

    internal A.TransformGroup ATransformGroup => this.pGroupShape.GroupShapeProperties!.TransformGroup!;

    internal override void Draw(SKCanvas canvas)
    {
        throw new System.NotImplementedException();
    }

    internal void OnGroupedShapeXChanged(object sender, int xGroupedShape)
    {
        var offset = this.ATransformGroup.Offset!;
        var extents = this.ATransformGroup.Extents!;
        var childOffset = this.ATransformGroup.ChildOffset!;
        var childExtents = this.ATransformGroup.ChildExtents!;
        
        if (xGroupedShape < this.X)
        {
            var groupedXEmu = UnitConverter.HorizontalPixelToEmu(xGroupedShape); 
            var diff = this.ATransformGroup.Offset!.X! - groupedXEmu;
            
            offset.X = new Int64Value(offset.X! - diff);
            extents.Cx = new Int64Value(extents.Cx! + diff);
            childOffset.X = new Int64Value(childOffset.X! - diff);
            childExtents.Cx = new Int64Value(childExtents.Cx! + diff);
            
            return;
        }

        var groupedShape = (SCShape)sender;
        var parentGroupRight = this.X + this.Width; 
        var groupedShapeRight = groupedShape.X + groupedShape.Width;
        if (groupedShapeRight > parentGroupRight)
        {
            
            var diff = groupedShapeRight - parentGroupRight;
            var diffEmu = UnitConverter.HorizontalPixelToEmu(diff);
            extents.Cx = new Int64Value(extents.Cx! + diffEmu);
            childExtents.Cx = new Int64Value(childExtents.Cx! + diffEmu);
        }
    }
    
    internal void OnGroupedShapeYChanged(object sender, int yGroupedShape)
    {
        var offset = this.ATransformGroup.Offset!;
        var extents = this.ATransformGroup.Extents!;
        var childOffset = this.ATransformGroup.ChildOffset!;
        var childExtents = this.ATransformGroup.ChildExtents!;
        
        if(yGroupedShape < this.Y)
        {
            var groupedYEmu = UnitConverter.VerticalPixelToEmu(yGroupedShape); 
            var diff = this.ATransformGroup.Offset!.Y! - groupedYEmu;
            
            offset.Y = new Int64Value(offset.Y! - diff);
            extents.Cy = new Int64Value(extents.Cy! + diff);
            childOffset.Y = new Int64Value(childOffset.Y! - diff);
            childExtents.Cy = new Int64Value(childExtents.Cy! + diff);
            
            return;
        }
    }

    protected override int GetXCoordinate()
    {
        var aXfrm = ((P.GroupShape)this.PShapeTreesChild).GroupShapeProperties!.TransformGroup!;

        return UnitConverter.HorizontalEmuToPixel(aXfrm.Offset!.X!);
    }

    protected override void SetXCoordinate(int xPx)
    {
        var pGrpSpPr = this.PShapeTreesChild.GetFirstChild<P.GroupShapeProperties>() !;
        var aXfrm = pGrpSpPr.TransformGroup!;
        aXfrm.Offset!.X = UnitConverter.HorizontalPixelToEmu(xPx);
        aXfrm.ChildOffset!.X = UnitConverter.HorizontalPixelToEmu(xPx);
    }
    
    protected override void SetYCoordinate(int yPx)
    {
        var pGrpSpPr = this.PShapeTreesChild.GetFirstChild<P.GroupShapeProperties>() !;
        var aXfrm = pGrpSpPr.TransformGroup!;
        aXfrm.Offset!.Y = UnitConverter.VerticalPixelToEmu(yPx);
    }
    
    protected override void SetWidth(int wPx)
    {
        var pGrpSpPr = this.PShapeTreesChild.GetFirstChild<P.GroupShapeProperties>() !;
        var aXfrm = pGrpSpPr.TransformGroup!;
        aXfrm.Extents!.Cx = UnitConverter.HorizontalPixelToEmu(wPx);
    }
    
    protected override void SetHeight(int hPx)
    {
        var pGrpSpPr = this.PShapeTreesChild.GetFirstChild<P.GroupShapeProperties>() !;
        var aXfrm = pGrpSpPr.TransformGroup!;
        aXfrm.Extents!.Cy = UnitConverter.VerticalPixelToEmu(hPx);
    }
}