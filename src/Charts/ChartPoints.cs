﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace ShapeCrawler.Charts;

internal sealed class ChartPoints : IReadOnlyList<IChartPoint>
{
    private readonly ChartPart chartPart;
    private readonly List<ChartPoint> chartPoints;

    internal ChartPoints(ChartPart chartPart, OpenXmlElement cSerXmlElement)
    {
        this.chartPart = chartPart;
        
        var cVal = cSerXmlElement.GetFirstChild<Values>();
        var cNumberReference =
            cVal != null ? cVal.NumberReference! : cSerXmlElement.GetFirstChild<YValues>() !.NumberReference!;

        // Get addresses
        var cFormula = cNumberReference.Formula!;
        
        var normalizedFormula = cFormula.Text.Replace("$", string.Empty).Replace("'", string.Empty);
        var sheetName = Regex.Match(normalizedFormula, @"(?<=\(*)[\p{L} 0-9]+?(?=!)", RegexOptions.None, TimeSpan.FromMilliseconds(1000)).Value; // eg: Sheet1!A2:A5 -> Sheet1
        var addressMatches = Regex.Matches(normalizedFormula, @"[A-Z]\d+(:[A-Z]\d+)*", RegexOptions.None, TimeSpan.FromMilliseconds(1000)); // eg: Sheet1!A2:A5 -> A2:A5
        var addresses = new List<string>();
        foreach (Match match in addressMatches)
        {
            if (match.Value.Contains(":"))
            {
                var rangePointAddresses = new CellsRange(match.Value).Addresses();
                addresses.AddRange(rangePointAddresses);
            }
            else
            {
                addresses.Add(match.Value);
            }
        }

        // Get cached values
        List<C.NumericValue>? cNumericValues = null;
        if (cNumberReference.NumberingCache != null)
        {
            cNumericValues = [.. cNumberReference.NumberingCache.Descendants<C.NumericValue>()];
        }

        // Generate points
        this.chartPoints = new List<ChartPoint>(addresses.Count);

        if (addresses.Count == 1 && cNumericValues?.Count > 1)
        {
            foreach (var cNumericValue in cNumericValues)
            {
                this.chartPoints.Add(new ChartPoint(this.chartPart, cNumericValue, sheetName, addresses[0]));
            }
        }
        else
        {
            // Empty cells of range don't have the corresponding C.NumericValue.
            var quPoints = System.Math.Min(addresses.Count, cNumericValues?.Count ?? 0);
            for (int i = 0; i < quPoints; i++)
            {
                this.chartPoints.Add(new ChartPoint(this.chartPart, cNumericValues?[i]!, sheetName, addresses[i]));
            }
        }
    }

    public int Count => this.chartPoints.Count;

    public IChartPoint this[int index] => this.chartPoints[index];

    public IEnumerator<IChartPoint> GetEnumerator() => this.chartPoints.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}