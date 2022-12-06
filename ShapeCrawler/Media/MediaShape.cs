﻿using System.Linq;
using DocumentFormat.OpenXml;
using ShapeCrawler.Extensions;
using ShapeCrawler.SlideMasters;
using P = DocumentFormat.OpenXml.Presentation;
using OneOf;

namespace ShapeCrawler.Media;

internal abstract class MediaShape : SlideShape
{
    protected MediaShape(OpenXmlCompositeElement pShapeTreeChild, OneOf<SCSlide, SCSlideLayout, SCSlideMaster> oneOfSlide, Shape? groupShape)
        : base(pShapeTreeChild, oneOfSlide, groupShape)
    {
    }

    public byte[] BinaryData => this.GetBinaryData();

    public string MIME => this.GetMime();

    private byte[] GetBinaryData()
    {
        var pPic = (P.Picture)this.PShapeTreesChild;
        var p14Media = pPic.NonVisualPictureProperties!.ApplicationNonVisualDrawingProperties!.Descendants<DocumentFormat.OpenXml.Office2010.PowerPoint.Media>().Single();
        var relationship = this.Slide.TypedOpenXmlPart.DataPartReferenceRelationships.First(r => r.Id == p14Media.Embed!.Value);
        var stream = relationship.DataPart.GetStream();
        var bytes = stream.ToArray();
        stream.Close();

        return bytes;
    }

    private string GetMime()
    {
        var pPic = (P.Picture)this.PShapeTreesChild;
        var p14Media = pPic.NonVisualPictureProperties!.ApplicationNonVisualDrawingProperties!.Descendants<DocumentFormat.OpenXml.Office2010.PowerPoint.Media>().Single();
        var relationship = this.Slide.TypedOpenXmlPart.DataPartReferenceRelationships.First(r => r.Id == p14Media.Embed!.Value);

        return relationship.DataPart.ContentType;
    }
}