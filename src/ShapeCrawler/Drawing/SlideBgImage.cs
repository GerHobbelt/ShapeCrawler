﻿using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Exceptions;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Drawing;

/// <summary>
///     Slide background image.
/// </summary>
internal sealed class SlideBgImage : ISlideBgImage
{
    private readonly SlidePart sdkSlidePart;
    private const string NotPresentedErrorMessage =
        $"Background image is not presented. Use {nameof(ISlideBgImage.Present)} to check.";
    
    internal SlideBgImage(SlidePart sdkSlidePart)
    {
        this.sdkSlidePart = sdkSlidePart;
    }

    public string MIME => this.ParseMIME();
    public string Name => this.ParseName();

    public void Update(Stream stream)
    {
        var aBlip = this.ABlip();
        var imageParts = this.sdkSlidePart.ImageParts;
        var sdkImagePart = this.SDKImagePartOrNull();
        var isSharedImagePart = imageParts.Count(x => x == sdkImagePart) > 1;
        if (isSharedImagePart)
        {
            var rId = $"rId-{Guid.NewGuid().ToString("N").Substring(0, 5)}";
            sdkImagePart = this.sdkSlidePart.AddNewPart<ImagePart>("image/png", rId);
            aBlip.Embed!.Value = rId;
        }

        stream.Position = 0;
        sdkImagePart!.FeedData(stream);
    }

    public void Update(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);

        this.Update(stream);
    }

    public void Update(string file)
    {
        byte[] sourceBytes = File.ReadAllBytes(file);
        this.Update(sourceBytes);
    }
    
    public byte[] AsByteArray()
    {
        var sdkImagePart = this.SDKImagePartOrNull();
        if (sdkImagePart == null)
        {
            throw new SCException(NotPresentedErrorMessage);
        }

        var stream = sdkImagePart.GetStream();
        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        stream.Close();

        return bytes;
    }

    public bool Present()
    {
        throw new NotImplementedException();
    }

    private A.Blip ABlip()
    {
        var pBg = this.sdkSlidePart.Slide.CommonSlideData!.Background!;
        if (pBg != null)
        {
            return pBg.BackgroundProperties!.Descendants<A.Blip>().First();    
        }
        
        var rId = $"rId-{Guid.NewGuid().ToString("N").Substring(0, 5)}";
        var aBlip = new A.Blip { Embed = rId };
        var pBackground = new P.Background(
            new P.BackgroundProperties(
                new A.BlipFill(aBlip)
                )
            );
        this.sdkSlidePart.Slide.CommonSlideData!.InsertAt(pBackground, 0);
        this.sdkSlidePart.AddNewPart<ImagePart>("image/png", rId);
        return aBlip;
    }

    private string ParseMIME()
    {
        var sdkImagePart = this.SDKImagePartOrNull();
        if (sdkImagePart == null)
        {
            throw new SCException(
                $"Background image is not presented. Use {nameof(ISlideBgImage.Present)} to check.");
        }

        return sdkImagePart.ContentType;
    }

    private ImagePart? SDKImagePartOrNull()
    {
        var pBg = this.sdkSlidePart.Slide.CommonSlideData!.Background;
        if (pBg == null)
        {
            return null;
        }
        
        var aBlip = pBg.BackgroundProperties!.Descendants<A.Blip>().First();

        return (ImagePart)this.sdkSlidePart.GetPartById(aBlip.Embed!.Value!);
    }

    private string ParseName()
    {
        var sdkImagePart = this.SDKImagePartOrNull();
        if (sdkImagePart == null)
        {
            throw new SCException(NotPresentedErrorMessage);
        }

        return Path.GetFileName(sdkImagePart.Uri.ToString());
    }
}