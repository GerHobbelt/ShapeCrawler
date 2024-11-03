﻿namespace ShapeCrawler.Units;

internal readonly ref struct Points
{
    private readonly decimal points;

    internal Points(float points)
    {
        this.points = (decimal)points;
    }

    internal Points(decimal points)
    {
        this.points = points;
    }

    internal long AsEmus() => (long)this.points * 12700;

    internal float AsPixels() => (float)this.points * 96 / 72;
}