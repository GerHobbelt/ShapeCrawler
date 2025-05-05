﻿#pragma warning disable IDE0130
namespace ShapeCrawler;
#pragma warning restore IDE0130

/// <summary>
///     Represents location of a shape.
/// </summary>
public interface IPosition
{
    /// <summary>
    ///     Gets or sets x-coordinate of the shape upper-left corner in points.
    /// </summary>
    decimal X { get; set; }

    /// <summary>
    ///     Gets or sets y-coordinate of the shape upper-left corner in points.
    /// </summary>
    decimal Y { get; set; }
}