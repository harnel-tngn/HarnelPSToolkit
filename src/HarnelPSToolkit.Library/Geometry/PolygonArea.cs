using System;

namespace HarnelPSToolkit.Library.Geometry;

[IncludeIfReferenced]
public static class PolygonArea
{
    public static long TriangleAreaDouble(LongPoint2D a, LongPoint2D b, LongPoint2D c)
    {
        return (a.X * b.Y - a.Y * b.X
            + b.X * c.Y - b.Y * c.X
            + c.X * a.Y - c.Y * a.X);
    }

    public static long TriangleAreaDoubleAbs(LongPoint2D a, LongPoint2D b, LongPoint2D c)
        => Math.Abs(TriangleAreaDouble(a, b, c));
}
