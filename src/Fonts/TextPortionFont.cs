﻿using System;
using System.Linq;
using DocumentFormat.OpenXml;
using ShapeCrawler.Colors;
using ShapeCrawler.Extensions;
using ShapeCrawler.Shapes;
using ShapeCrawler.Texts;
using A = DocumentFormat.OpenXml.Drawing;

namespace ShapeCrawler.Fonts;

internal sealed class TextPortionFont : ITextPortionFont
{
    private readonly A.Text aText;
    private readonly Lazy<FontColor> fontColor;
    private readonly IFontSize size;
    private readonly ThemeFontScheme themeFontScheme;
    private readonly SCAText scAText;

    internal TextPortionFont(
        A.Text aText,
        IFontSize size)
        : this(
            aText,
            size,
            new ThemeFontScheme(aText.Ancestors<OpenXmlPartRootElement>().First().OpenXmlPart!))
    {
    }

    private TextPortionFont(A.Text aText, IFontSize size, ThemeFontScheme themeFontScheme)
    {
        this.aText = aText;
        this.fontColor = new Lazy<FontColor>(() => new FontColor(this.aText));
        this.size = size;
        this.themeFontScheme = themeFontScheme;
        this.scAText = new SCAText(aText);
    }

    #region Public APIs

    public decimal Size
    {
        get => this.size.Size;
        set => this.size.Size = value;
    }

    public string? LatinName
    {
        get
        {
            if (this.ALatinFont().Typeface == "+mj-lt")
            {
                return this.themeFontScheme.MajorLatinFont();
            }

            return this.ALatinFont().Typeface!;
        }
        set => this.UpdateLatinName(value!);
    }

    public string EastAsianName
    {
        get => this.scAText.EastAsianName();
        set => this.scAText.UpdateEastAsianName(value);
    }

    public bool IsBold
    {
        get => this.ParseBoldFlag();
        set => this.UpdateBoldFlag(value);
    }

    public bool IsItalic
    {
        get => this.GetItalicFlag();
        set => this.SetItalicFlag(value);
    }

    public IFontColor Color => this.fontColor.Value;

    public A.TextUnderlineValues Underline
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
                var aEndParaRPr =
                    this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
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

    #endregion Public APIs

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
    
    private A.LatinFont ALatinFont()
    {
        var openXmlPart = this.aText.Ancestors<OpenXmlPartRootElement>().First().OpenXmlPart!;
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        var aLatinFont = aRunProperties?.GetFirstChild<A.LatinFont>();

        if (aLatinFont != null)
        {
            return aLatinFont;
        }

        aLatinFont = new ReferencedIndentLevel(this.aText).ALatinFontOrNull();
        if (aLatinFont != null)
        {
            return aLatinFont;
        }

        return new ThemeFontScheme(openXmlPart).MinorLatinFont();
    }

    private bool ParseBoldFlag()
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

        bool? isFontBold = new ReferencedIndentLevel(this.aText).FontBoldFlagOrNull();
        if (isFontBold.HasValue)
        {
            return isFontBold.Value;
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

        IndentFont phIndentFont = new();
        if (phIndentFont.IsItalic is not null)
        {
            return phIndentFont.IsItalic.Value;
        }

        return false;
    }

    private void UpdateBoldFlag(bool value)
    {
        var aRunPr = this.aText.Parent!.GetFirstChild<A.RunProperties>();
        if (aRunPr != null)
        {
            aRunPr.Bold = new BooleanValue(value);
        }
        else
        {
            IndentFont phIndentFont = new();
            if (phIndentFont.IsBold is not null)
            {
                phIndentFont.IsBold = new BooleanValue(value);
            }
            else
            {
                var aEndParaRPr =
                    this.aText.Parent.NextSibling<A.EndParagraphRunProperties>();
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
                aRunPr = new A.RunProperties { Italic = new BooleanValue(isItalic) };
                aTextParent.InsertAt(aRunPr, 0);
            }
        }
    }

    private void UpdateLatinName(string latinFont)
    {
        var aRunProperties = this.aText.Parent!.GetFirstChild<A.RunProperties>();

        A.TextCharacterPropertiesType aCurrentProperties; 

        if (aRunProperties is not null)
        {
            aCurrentProperties = aRunProperties;
        }
        else
        {
            var aEndParaRunProperties = this.aText.Parent!.GetFirstChild<A.EndParagraphRunProperties>();

            if (aEndParaRunProperties is not null)
            {
                aCurrentProperties = aEndParaRunProperties;   
            }
            else
            {
                aCurrentProperties = this.aText.Parent!.AddRunProperties();
            }
        }

        var aLatinFont = aCurrentProperties.GetFirstChild<A.LatinFont>();

        if (aLatinFont is null)
        {
            aLatinFont = new A.LatinFont();
            aCurrentProperties.Append(aLatinFont); // to avoid ignoring color
        }
        
        aLatinFont.Typeface = latinFont;
    }
}
