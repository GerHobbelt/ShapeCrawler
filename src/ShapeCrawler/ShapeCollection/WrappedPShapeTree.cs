﻿using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using ShapeCrawler.Extensions;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.ShapeCollection;

internal readonly ref struct WrappedPShapeTree
{
    private readonly P.ShapeTree pShapeTree;

    internal WrappedPShapeTree(P.ShapeTree pShapeTree)
    {
        this.pShapeTree = pShapeTree;
    }

    internal void Add(OpenXmlElement sdkOpenXmlElement)
    {
        var id = this.pShapeTree.Descendants<P.NonVisualDrawingProperties>().Select(s => s.Id!.Value).Max() + 1;
        var existingShapeNames = this.pShapeTree.Descendants<P.NonVisualDrawingProperties>().Select(s => s.Name!.Value!);
        var pShapeCopy = sdkOpenXmlElement.CloneNode(true);
        pShapeCopy.NonVisualDrawingProperties().Id = new UInt32Value(id);
        this.pShapeTree.AppendChild(pShapeCopy);
        var copyName = pShapeCopy.NonVisualDrawingProperties().Name!.Value!;
        if (existingShapeNames.Any(existingShapeName => existingShapeName == copyName))
        {
            var currentShapeCollectionSuffixes = existingShapeNames
                .Where(c => c.StartsWith(copyName, StringComparison.InvariantCulture))
                .Select(c => c.Substring(copyName.Length))
                .ToArray();

            // We will try to check numeric suffixes only.
            var numericSuffixes = new List<int>();

            foreach (var currentSuffix in currentShapeCollectionSuffixes)
            {
                if (int.TryParse(currentSuffix, out var numericSuffix))
                {
                    numericSuffixes.Add(numericSuffix);
                }
            }

            numericSuffixes.Sort();
            var lastSuffix = numericSuffixes.LastOrDefault() + 1;
            pShapeCopy.NonVisualDrawingProperties().Name = copyName + " " + lastSuffix;
        }
    }

    internal P.Shape? ReferencedPShapeOrNull(P.PlaceholderShape pPlaceholder)
    {
        var pShapes = this.pShapeTree.Elements<P.Shape>();
        foreach (var layoutPShape in pShapes)
        {
            var layoutPPlaceholder = layoutPShape.NonVisualShapeProperties!.ApplicationNonVisualDrawingProperties!
                .GetFirstChild<P.PlaceholderShape>();
            if (layoutPPlaceholder == null)
            {
                continue;
            }

            if (pPlaceholder.Index is not null
                && layoutPPlaceholder.Index is not null
                && pPlaceholder.Index == layoutPPlaceholder.Index)
            {
                return layoutPShape;
            }

            if (pPlaceholder.Type == null || layoutPPlaceholder.Type == null)
            {
                return layoutPShape;
            }

            if (pPlaceholder.Type == P.PlaceholderValues.Body
                && pPlaceholder.Index is not null
                && layoutPPlaceholder.Index is not null)
            {
                if (pPlaceholder.Index == layoutPPlaceholder.Index)
                {
                    return layoutPShape;
                }
            }

            if (pPlaceholder.Type == P.PlaceholderValues.Title && layoutPPlaceholder.Type == P.PlaceholderValues.Title)
            {
                return layoutPShape;
            }

            if (pPlaceholder.Type == P.PlaceholderValues.CenteredTitle && layoutPPlaceholder.Type == P.PlaceholderValues.CenteredTitle)
            {
                return layoutPShape;
            }
        }

        var byType = pShapes.FirstOrDefault(layoutPShape =>
            layoutPShape.NonVisualShapeProperties!.ApplicationNonVisualDrawingProperties!
                .GetFirstChild<P.PlaceholderShape>()?.Type?.Value == pPlaceholder.Type?.Value);
        if (byType != null)
        {
            return byType;
        }

        return null;
    }
}