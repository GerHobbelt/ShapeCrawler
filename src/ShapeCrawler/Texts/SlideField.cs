﻿using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Drawing;
using ShapeCrawler.Extensions;
using ShapeCrawler.Fonts;
using ShapeCrawler.Services;
using ShapeCrawler.Shared;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Texts;

internal sealed class SlideField : IParagraphPortion
{
    private readonly ResetableLazy<ITextPortionFont> font;
    private readonly A.Field aField;
    private readonly PortionText portionText;
    private readonly A.Text? aText;
    private readonly SlidePart sdkSlidePart;

    internal SlideField(SlidePart sdkSlidePart, A.Field aField)
    {
        this.sdkSlidePart = sdkSlidePart;
        this.aText = aField.GetFirstChild<A.Text>();
        this.aField = aField;

        this.font = new ResetableLazy<ITextPortionFont>(() =>
        {
            var textPortionSize = new PortionFontSize(this.aText!);
            return new SlideTextPortionFont(sdkSlidePart, this.aText!, textPortionSize);
        });

        this.portionText = new PortionText(this.aField);
    }

    internal event Action? Removed;

    /// <inheritdoc/>
    public string? Text
    {
        get => this.portionText.Text();
        set => this.portionText.Update(value!);
    }

    /// <inheritdoc/>
    public ITextPortionFont Font => this.font.Value;

    public string? Hyperlink
    {
        get => this.GetHyperlink();
        set => this.SetHyperlink(value);
    }

    public SCColor? TextHighlightColor
    {
        get => this.ParseTextHighlight();
        set => this.UpdateTextHighlight(value);
    }

    internal bool IsRemoved { get; set; }

    public void Remove()
    {
        this.aField.Remove();
        this.Removed?.Invoke();
    }

    private SCColor ParseTextHighlight()
    {
        var arPr = this.aText!.PreviousSibling<A.RunProperties>();

        // Ensure RgbColorModelHex exists and his value is not null.
        if (arPr?.GetFirstChild<A.Highlight>()?.RgbColorModelHex is not A.RgbColorModelHex aSrgbClr
            || aSrgbClr.Val is null)
        {
            return SCColor.Transparent;
        }

        // Gets node value.
        // TODO: Check if DocumentFormat.OpenXml.StringValue is necessary.
        var hex = aSrgbClr.Val.ToString() !;

        // Check if color value is valid, we are expecting values as "000000".
        var color = SCColor.FromHex(hex);

        // Calculate alpha value if is defined in highlight node.
        var aAlphaValue = aSrgbClr.GetFirstChild<A.Alpha>()?.Val ?? 100000;
        color.Alpha = SCColor.OPACITY / (100000 / aAlphaValue);

        return color;
    }

    private void UpdateTextHighlight(SCColor? color)
    {
        var arPr = this.aText!.PreviousSibling<A.RunProperties>() ?? this.aText.Parent!.AddRunProperties();

        arPr.AddAHighlight((SCColor)color);
    }

    private string? GetHyperlink()
    {
        var runProperties = this.aText!.PreviousSibling<A.RunProperties>();
        if (runProperties == null)
        {
            return null;
        }

        var hyperlink = runProperties.GetFirstChild<A.HyperlinkOnClick>();
        if (hyperlink == null)
        {
            return null;
        }

        var hyperlinkRelationship = (HyperlinkRelationship)this.sdkSlidePart.GetReferenceRelationship(hyperlink.Id!);

        return hyperlinkRelationship.Uri.ToString();
    }

    private void SetHyperlink(string? url)
    {
        var runProperties = this.aText!.PreviousSibling<A.RunProperties>();
        if (runProperties == null)
        {
            runProperties = new A.RunProperties();
        }

        var hyperlink = runProperties.GetFirstChild<A.HyperlinkOnClick>();
        if (hyperlink == null)
        {
            hyperlink = new A.HyperlinkOnClick();
            runProperties.Append(hyperlink);
        }

        var uri = new Uri(url!, UriKind.RelativeOrAbsolute);
        var addedHyperlinkRelationship = sdkSlidePart.AddHyperlinkRelationship(uri, true);

        hyperlink.Id = addedHyperlinkRelationship.Id;
    }
}