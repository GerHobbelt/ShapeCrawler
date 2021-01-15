﻿namespace ShapeCrawler.Models
{
    /// <summary>
    /// Represent presentation slides size.
    /// </summary>
    public class SlideSizeSc
    {
        #region Properties

        public int Width { get; }

        public int Height { get; }

        #endregion Properties

        #region Constructors

        public SlideSizeSc(int sdkW, int sdkH)
        {
            Width = sdkW;
            Height = sdkH;
        }

        #endregion Constructors
    }
}