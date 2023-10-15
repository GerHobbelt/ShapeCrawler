﻿using System;
using DocumentFormat.OpenXml;
using ShapeCrawler.Constants;
using ShapeCrawler.Extensions;
using ShapeCrawler.Factories;
using ShapeCrawler.Placeholders;
using ShapeCrawler.Services;
using ShapeCrawler.Shared;
using A = DocumentFormat.OpenXml.Drawing;

namespace ShapeCrawler.AutoShapes;

internal class SCFont : IFont
{
    private readonly A.Text aText;
    private readonly A.FontScheme aFontScheme;
    private readonly Lazy<ColorFormat> colorFormat;
    private readonly ResettableLazy<A.LatinFont> latinFont;
    private readonly ResettableLazy<int> size;

    internal SCFont(A.Text aText, SCPortion portion)
    {
        this.aText = aText;
        this.size = new ResettableLazy<int>(this.GetSize);
        this.latinFont = new ResettableLazy<A.LatinFont>(this.GetALatinFont);
        this.colorFormat = new Lazy<ColorFormat>(() => new ColorFormat(this));
        this.ParentPortion = portion;
        var parentTextBoxContainer = portion.ParentParagraph.ParentTextFrame.TextFrameContainer;
        Shape shape;
        if (parentTextBoxContainer is SCCell cell)
        {
            shape = cell.Shape;
        }
        else
        {
            shape = (Shape)portion.ParentParagraph.ParentTextFrame.TextFrameContainer;
        }

        this.aFontScheme = shape.SlideMasterInternal.ThemePart.Theme.ThemeElements!.FontScheme!;
    }

    public string Name
    {
        get => this.GetName();
        set => this.SetName(value);
    }

    public int Size
    {
        get => this.size.Value;
        set => this.SetFontSize(value);
    }

    public bool IsBold
    {
        get => this.GetBoldFlag();
        set => this.SetBoldFlag(value);
    }

    public bool IsItalic
    {
        get => this.GetItalicFlag();
        set => this.SetItalicFlag(value);
    }

    public IColorFormat ColorFormat => this.colorFormat.Value;

    public DocumentFormat.OpenXml.Drawing.TextUnderlineValues Underline
    {
        get
        {
            var aRunPr = this.aText.Parent!.GetFirstChild<A.RunProperties>();
            return aRunPr?.Underline?.Value ?? A.TextUnderlineValues.None;
        }

        set
        {
            var aRunPr = this.aText.Parent!.GetFirstChild<A.RunProperties>();
            if (aRunPr != null)
            {
                aRunPr.Underline = new EnumValue<A.TextUnderlineValues>(value);
            }
            else
            {
                var aEndParaRPr = this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
                if (aEndParaRPr != null)
                {
                    aEndParaRPr.Underline = new EnumValue<A.TextUnderlineValues>(value);
                }
                else
                {
                    var runProp = this.aText.Parent.AddRunProperties();
                    runProp.Underline = new EnumValue<A.TextUnderlineValues>(value);
                }
            }
        }
    }

    public int OffsetEffect
    {
        get => this.GetOffsetEffect();
        set => this.SetOffset(value);
    }

    internal SCPortion ParentPortion { get; }

    public bool CanChange()
    {
        var placeholder = this.ParentPortion.ParentParagraph.ParentTextFrame.TextFrameContainer.Shape.Placeholder;

        return placeholder is null or { Type: SCPlaceholderType.Text };
    }

    private void SetOffset(int value)
    {
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        Int32Value int32Value = value * 1000;
        if (aRunProperties is not null)
        {
            aRunProperties.Baseline = int32Value;
        }
        else
        {
            var aEndParaRPr = this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
            if (aEndParaRPr != null)
            {
                aEndParaRPr.Baseline = int32Value;
            }
            else
            {
                aRunProperties = new A.RunProperties { Baseline = int32Value };
                this.aText.Parent.InsertAt(aRunProperties, 0); // append to <a:r>
            }
        }
    }

    private int GetOffsetEffect()
    {
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        if (aRunProperties is not null &&
            aRunProperties.Baseline is not null)
        {
            return aRunProperties.Baseline.Value / 1000;
        }

        var aEndParaRPr = this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
        if (aEndParaRPr is not null)
        {
            return aEndParaRPr.Baseline! / 1000;
        }

        return 0;
    }

    private string GetName()
    {
        const string majorLatinFont = "+mj-lt";
        if (this.latinFont.Value.Typeface == majorLatinFont)
        {
            return this.aFontScheme.MajorFont!.LatinFont!.Typeface!;
        }

        return this.latinFont.Value.Typeface!;
    }

    private A.LatinFont GetALatinFont()
    {
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        var aLatinFont = aRunProperties?.GetFirstChild<A.LatinFont>();

        if (aLatinFont != null)
        {
            return aLatinFont;
        }

        var phFontData = FontDataParser.FromPlaceholder(this.ParentPortion.ParentParagraph);
        return phFontData.ALatinFont ?? this.aFontScheme.MinorFont!.LatinFont!;
    }

    private int GetSize()
    {
        var fontSize = this.ParentPortion.AText.Parent!.GetFirstChild<A.RunProperties>()?.FontSize?.Value;
        if (fontSize != null)
        {
            return fontSize.Value / 100;
        }

        var paragraph = this.ParentPortion.ParentParagraph;
        var textFrameContainer = paragraph.ParentTextFrame.TextFrameContainer;
        var paraLevel = paragraph.Level;

        if (textFrameContainer is Shape { Placeholder: { } } shape)
        {
            if (TryFromPlaceholder(shape, paraLevel, out var sizeFromPlaceholder))
            {
                return sizeFromPlaceholder;
            }
        }

        var presentation = textFrameContainer.Shape.SlideBase.PresentationInternal;
        if (presentation.ParaLvlToFontData.TryGetValue(paraLevel, out var fontData))
        {
            if (fontData.FontSize is not null)
            {
                return fontData.FontSize / 100;
            }
        }

        return SCConstants.DefaultFontSize;
    }

    private static bool TryFromPlaceholder(Shape shape, int paraLevel, out int i)
    {
        i = -1;
        var placeholder = shape.Placeholder as Placeholder;
        var referencedShape = placeholder?.ReferencedShape.Value as AutoShape;
        var fontDataPlaceholder = new FontData();
        if (referencedShape != null)
        {
            referencedShape.FillFontData(paraLevel, ref fontDataPlaceholder);
            if (fontDataPlaceholder.FontSize is not null)
            {
                {
                    i = fontDataPlaceholder.FontSize / 100;
                    return true;
                }
            }
        }

        var slideMaster = shape.SlideMasterInternal;
        if (placeholder?.Type == SCPlaceholderType.Title)
        {
            var pTextStyles = slideMaster.PSlideMaster.TextStyles!;
            var titleFontSize = pTextStyles.TitleStyle!.Level1ParagraphProperties!
                .GetFirstChild<A.DefaultRunProperties>() !.FontSize!.Value;
            i = titleFontSize / 100;
            return true;
        }

        if (slideMaster.TryGetFontSizeFromBody(paraLevel, out var fontSizeBody))
        {
            {
                i = fontSizeBody / 100;
                return true;
            }
        }

        if (slideMaster.TryGetFontSizeFromOther(paraLevel, out var fontSizeOther))
        {
            {
                i = fontSizeOther / 100;
                return true;
            }
        }

        return false;
    }

    private bool GetBoldFlag()
    {
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        if (aRunProperties == null)
        {
            return false;
        }

        if (aRunProperties.Bold is not null && aRunProperties.Bold == true)
        {
            return true;
        }

        FontData phFontData = new ();
        FontDataParser.GetFontDataFromPlaceholder(ref phFontData, this.ParentPortion.ParentParagraph);
        if (phFontData.IsBold is not null)
        {
            return phFontData.IsBold.Value;
        }

        return false;
    }

    private bool GetItalicFlag()
    {
        var aRunPr = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        if (aRunPr == null)
        {
            return false;
        }

        if (aRunPr.Italic is not null && aRunPr.Italic == true)
        {
            return true;
        }

        FontData phFontData = new ();
        FontDataParser.GetFontDataFromPlaceholder(ref phFontData, this.ParentPortion.ParentParagraph);
        if (phFontData.IsItalic is not null)
        {
            return phFontData.IsItalic.Value;
        }

        return false;
    }

    private void SetBoldFlag(bool value)
    {
        var aRunPr = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        if (aRunPr != null)
        {
            aRunPr.Bold = new BooleanValue(value);
        }
        else
        {
            FontData phFontData = new ();
            FontDataParser.GetFontDataFromPlaceholder(ref phFontData, this.ParentPortion.ParentParagraph);
            if (phFontData.IsBold is not null)
            {
                phFontData.IsBold = new BooleanValue(value);
            }
            else
            {
                var aEndParaRPr = this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
                if (aEndParaRPr != null)
                {
                    aEndParaRPr.Bold = new BooleanValue(value);
                }
                else
                {
                    aRunPr = new A.RunProperties { Bold = new BooleanValue(value) };
                    this.aText.Parent.InsertAt(aRunPr, 0); // append to <a:r>
                }
            }
        }
    }

    private void SetItalicFlag(bool isItalic)
    {
        var aTextParent = this.aText.Parent!;
        var aRunPr = aTextParent.GetFirstChild<A.RunProperties>();
        if (aRunPr != null)
        {
            aRunPr.Italic = new BooleanValue(isItalic);
        }
        else
        {
            var aEndParaRPr = aTextParent.NextSibling<A.EndParagraphRunProperties>();
            if (aEndParaRPr != null)
            {
                aEndParaRPr.Italic = new BooleanValue(isItalic);
            }
            else
            {
                aTextParent.AddRunProperties(isItalic);
            }
        }
    }

    private void SetName(string fontName)
    {
        var aLatinFont = this.latinFont.Value;
        aLatinFont.Typeface = fontName;
        this.latinFont.Reset();
    }

    private void SetFontSize(int newFontSize)
    {
        var parent = this.aText.Parent!;
        var aRunPr = parent.GetFirstChild<A.RunProperties>();
        if (aRunPr == null)
        {
            var builder = new ARunPropertiesBuilder();
            aRunPr = builder.Build();
            parent.InsertAt(aRunPr, 0);
        }

        aRunPr.FontSize = newFontSize * 100;
    }
}