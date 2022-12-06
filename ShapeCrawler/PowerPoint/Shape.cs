﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using OneOf;
using ShapeCrawler.Constants;
using ShapeCrawler.Exceptions;
using ShapeCrawler.Extensions;
using ShapeCrawler.Placeholders;
using ShapeCrawler.Shapes;
using ShapeCrawler.SlideMasters;
using ShapeCrawler.Statics;
using SkiaSharp;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler;

internal abstract class Shape : IShape
{
    protected Shape(
        OpenXmlCompositeElement pShapeTreeChild, 
        OneOf<SCSlide, SCSlideLayout, SCSlideMaster> slideObject,
        Shape? groupShape)
        : this(pShapeTreeChild, slideObject)
    {
        this.GroupShape = groupShape;
        this.SlideObject = slideObject.Match(slide => slide as SlideObject, layout => layout, master => master);
    }

    protected Shape(OpenXmlCompositeElement pShapeTreeChild, OneOf<SCSlide, SCSlideLayout, SCSlideMaster> slideObject)
    {
        this.PShapeTreesChild = pShapeTreeChild;
        this.SlideObject = slideObject.Match(slide => slide as SlideObject, layout => layout, master => master);
        this.SlideBase = slideObject.Match(slide => slide as SlideObject, layout => layout, master => master);
    }

    public int Id => (int)this.PShapeTreesChild.GetNonVisualDrawingProperties().Id!.Value;

    public string Name => this.PShapeTreesChild.GetNonVisualDrawingProperties().Name!;

    public bool Hidden =>
        this.DefineHidden(); // TODO: the Shape is inherited by LayoutShape, hence do we need this property?

    public string? CustomData
    {
        get => this.GetCustomData();
        set => this.SetCustomData(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public abstract SCShapeType ShapeType { get; }

    public ISlideObject SlideObject { get; }

    public abstract IPlaceholder? Placeholder { get; }

    public virtual SCGeometry GeometryType => this.GetGeometryType();

    public int X
    {
        get => this.GetXCoordinate();
        set => this.SetXCoordinate(value);
    }

    public int Y
    {
        get => this.GetYCoordinate();
        set => this.SetYCoordinate(value);
    }

    public int Height
    {
        get => this.GetHeightPixels();
        set => this.SetHeight(value);
    }

    public int Width
    {
        get => this.GetWidthPixels();
        set => this.SetWidth(value);
    }

    internal SCSlideMaster SlideMasterInternal
    {
        get
        {
            if (this.SlideBase is SCSlide slide)
            {
                return slide.SlideLayoutInternal.SlideMasterInternal;
            }

            if (this.SlideBase is SCSlideLayout layout)
            {
                return layout.SlideMasterInternal;
            }

            var master = (SCSlideMaster)this.SlideBase;
            return master;
        }
    }

    internal OpenXmlCompositeElement PShapeTreesChild { get; }

    internal SlideObject SlideBase { get; }

    internal P.ShapeProperties PShapeProperties => this.PShapeTreesChild.GetFirstChild<P.ShapeProperties>() !;

    private Shape? GroupShape { get; }

    internal abstract void Draw(SKCanvas canvas);

    private void SetCustomData(string value)
    {
        string customDataElement =
            $@"<{SCConstants.CustomDataElementName}>{value}</{SCConstants.CustomDataElementName}>";
        this.PShapeTreesChild.InnerXml += customDataElement;
    }

    private string? GetCustomData()
    {
        var pattern = @$"<{SCConstants.CustomDataElementName}>(.*)<\/{SCConstants.CustomDataElementName}>";
        var regex = new Regex(pattern);
        var elementText = regex.Match(this.PShapeTreesChild.InnerXml).Groups[1];
        if (elementText.Value.Length == 0)
        {
            return null;
        }

        return elementText.Value;
    }

    private bool DefineHidden()
    {
        var parsedHiddenValue = this.PShapeTreesChild.GetNonVisualDrawingProperties().Hidden?.Value;
        return parsedHiddenValue is true;
    }

    private void SetXCoordinate(int newXPixels)
    {
        if (this.GroupShape is not null)
        {
            throw new RuntimeDefinedPropertyException("X coordinate of grouped shape cannot be changed.");
        }
        
        var pSpPr = this.PShapeTreesChild.GetFirstChild<P.ShapeProperties>() !;
        var aXfrm = pSpPr.Transform2D;
        if (aXfrm is null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            var referencedShape = placeholder.ReferencedShape.Value;
            var xEmu = UnitConverter.HorizontalPixelToEmu(newXPixels);
            var yEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.Y);
            var wEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Width);
            var hEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Height);
            pSpPr.AddAXfrm(xEmu, yEmu, wEmu, hEmu);
        }
        else
        {
            aXfrm.Offset!.X = UnitConverter.HorizontalPixelToEmu(newXPixels);
        }
    }
    
    private void SetYCoordinate(int newYPixels)
    {
        if (this.GroupShape is not null)
        {
            throw new RuntimeDefinedPropertyException("Y coordinate of grouped shape cannot be changed.");
        }
        
        var pSpPr = this.PShapeTreesChild.GetFirstChild<P.ShapeProperties>() !;
        var aXfrm = pSpPr.Transform2D;
        if (aXfrm is null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            var referencedShape = placeholder.ReferencedShape.Value;
            var xEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.X);
            var yEmu = UnitConverter.HorizontalPixelToEmu(newYPixels);
            var wEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Width);
            var hEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Height);
            pSpPr.AddAXfrm(xEmu, yEmu, wEmu, hEmu);
        }
        else
        {
            aXfrm.Offset!.Y = UnitConverter.HorizontalPixelToEmu(newYPixels);
        }
    }
    
    private void SetHeight(int newHPixels)
    {
        if (this.GroupShape is not null)
        {
            throw new RuntimeDefinedPropertyException("Height coordinate of grouped shape cannot be changed.");
        }
        
        var pSpPr = this.PShapeTreesChild.GetFirstChild<P.ShapeProperties>() !;
        var aXfrm = pSpPr.Transform2D;
        if (aXfrm is null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            var referencedShape = placeholder.ReferencedShape.Value;
            var xEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.X);
            var yEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.X);
            var wEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Width);
            var hEmu = UnitConverter.VerticalPixelToEmu(newHPixels);
            pSpPr.AddAXfrm(xEmu, yEmu, wEmu, hEmu);
        }
        else
        {
            aXfrm.Extents!.Cy = UnitConverter.HorizontalPixelToEmu(newHPixels);
        }
    }
    
    private void SetWidth(int newWPixels)
    {
        if (this.GroupShape is not null)
        {
            throw new RuntimeDefinedPropertyException("Width coordinate of grouped shape cannot be changed.");
        }
        
        var pSpPr = this.PShapeTreesChild.GetFirstChild<P.ShapeProperties>() !;
        var aXfrm = pSpPr.Transform2D;
        if (aXfrm is null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            var referencedShape = placeholder.ReferencedShape.Value;
            var xEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.X);
            var yEmu = UnitConverter.HorizontalPixelToEmu(referencedShape.X);
            var wEmu = UnitConverter.VerticalPixelToEmu(newWPixels);
            var hEmu = UnitConverter.VerticalPixelToEmu(referencedShape.Height);
            pSpPr.AddAXfrm(xEmu, yEmu, wEmu, hEmu);
        }
        else
        {
            aXfrm.Extents!.Cx = UnitConverter.HorizontalPixelToEmu(newWPixels);
        }
    }

    private int GetXCoordinate()
    {
        var aOffset = this.PShapeTreesChild.Descendants<A.Offset>().FirstOrDefault();
        if (aOffset == null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            var referencedShape = placeholder.ReferencedShape.Value; 
            return referencedShape.X;
        }

        long xEmu = aOffset.X!;

        if (this.GroupShape is not null)
        {
            var aTransformGroup = ((P.GroupShape)this.GroupShape.PShapeTreesChild).GroupShapeProperties!.TransformGroup;
            xEmu = xEmu - aTransformGroup!.ChildOffset!.X! + aTransformGroup!.Offset!.X!;
        }

        return UnitConverter.HorizontalEmuToPixel(xEmu);
    }

    private int GetYCoordinate()
    {
        var aOffset = this.PShapeTreesChild.Descendants<A.Offset>().FirstOrDefault();
        if (aOffset == null)
        {
            return ((Placeholder)this.Placeholder!).ReferencedShape.Value.Y;
        }

        var yEmu = aOffset.Y!;

        if (this.GroupShape is not null)
        {
            var aTransformGroup =
                ((P.GroupShape)this.GroupShape.PShapeTreesChild).GroupShapeProperties!.TransformGroup!;
            yEmu = yEmu - aTransformGroup.ChildOffset!.Y! + aTransformGroup!.Offset!.Y!;
        }

        return UnitConverter.VerticalEmuToPixel(yEmu);
    }

    private int GetWidthPixels()
    {
        var aExtents = this.PShapeTreesChild.Descendants<A.Extents>().FirstOrDefault();
        if (aExtents == null)
        {
            var placeholder = (Placeholder)this.Placeholder!;
            return placeholder.ReferencedShape.Value.Width;
        }

        return UnitConverter.HorizontalEmuToPixel(aExtents.Cx!);
    }

    private int GetHeightPixels()
    {
        var aExtents = this.PShapeTreesChild.Descendants<A.Extents>().FirstOrDefault();
        if (aExtents == null)
        {
            return ((Placeholder)this.Placeholder!).ReferencedShape.Value.Height;
        }

        return UnitConverter.VerticalEmuToPixel(aExtents!.Cy!);
    }

    private SCGeometry GetGeometryType()
    {
        var spPr = this.PShapeTreesChild.Descendants<P.ShapeProperties>().First(); // TODO: optimize
        var aTransform2D = spPr.Transform2D;
        if (aTransform2D != null)
        {
            var aPresetGeometry = spPr.GetFirstChild<A.PresetGeometry>();

            // Placeholder can have transform on the slide, without having geometry
            if (aPresetGeometry == null)
            {
                if (spPr.OfType<A.CustomGeometry>().Any())
                {
                    return SCGeometry.Custom;
                }
            }
            else
            {
                var name = aPresetGeometry.Preset!.Value.ToString();
                Enum.TryParse(name, true, out SCGeometry geometryType);
                return geometryType;
            }
        }

        var placeholder = this.Placeholder as Placeholder;
        if (placeholder?.ReferencedShape.Value != null)
        {
            return placeholder.ReferencedShape.Value.GeometryType;
        }

        return SCGeometry.Rectangle; // return default
    }
}