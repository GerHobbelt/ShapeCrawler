﻿using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;

namespace ShapeCrawler.Extensions;

internal static class OpenXmlElementExtensions
{
    internal static A.RunProperties AddRunProperties(this OpenXmlElement openXmlElement)
    {
        var aRunPr = new A.RunProperties();
        openXmlElement.InsertAt(aRunPr, 0);

        return aRunPr;
    }
}