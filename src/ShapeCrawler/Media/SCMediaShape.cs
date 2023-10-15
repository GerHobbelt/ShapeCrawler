﻿using System.Linq;
using DocumentFormat.OpenXml;
using OneOf;
using ShapeCrawler.Extensions;
using ShapeCrawler.Shapes;
using ShapeCrawler.SlideMasters;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Media;

internal abstract class SCMediaShape : SCShape
{
    protected SCMediaShape(
        OpenXmlCompositeElement pShapeTreeChild,
        OneOf<SCSlide, SCSlideLayout, SCSlideMaster> parentSlideObject,
        OneOf<ShapeCollection, SCGroupShape> parentShapeCollection) 
        : base(pShapeTreeChild, parentSlideObject, parentShapeCollection)
    {
    }

    public byte[] BinaryData => this.GetBinaryData();

    public string MIME => this.GetMime();

    private byte[] GetBinaryData()
    {
        var pPic = (P.Picture)this.PShapeTreesChild;
        var p14Media = pPic.NonVisualPictureProperties!.ApplicationNonVisualDrawingProperties!.Descendants<DocumentFormat.OpenXml.Office2010.PowerPoint.Media>().Single();
        var relationship = this.SlideBase.TypedOpenXmlPart.DataPartReferenceRelationships.First(r => r.Id == p14Media.Embed!.Value);
        var stream = relationship.DataPart.GetStream();
        var bytes = stream.ToArray();
        stream.Close();

        return bytes;
    }

    private string GetMime()
    {
        var pPic = (P.Picture)this.PShapeTreesChild;
        var p14Media = pPic.NonVisualPictureProperties!.ApplicationNonVisualDrawingProperties!.Descendants<DocumentFormat.OpenXml.Office2010.PowerPoint.Media>().Single();
        var relationship = this.SlideBase.TypedOpenXmlPart.DataPartReferenceRelationships.First(r => r.Id == p14Media.Embed!.Value);

        return relationship.DataPart.ContentType;
    }
}