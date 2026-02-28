using System;

namespace HarnelPSToolkit.Library.Geometry;

[IncludeIfReferenced]
public record struct LongPoint2D : IPoint2D<long, LongPoint2D, LongPoint2D.PointOperator>
{
    public long X { get; }
    public long Y { get; }

    public LongPoint2D(long x, long y)
    {
        X = x;
        Y = y;
    }

    public long MagnitudeSquare => X * X + Y * Y;

    public override string ToString() => $"{X}, {Y}";

    public static implicit operator LongPoint2D((long X, long Y) p)
       => new LongPoint2D(p.X, p.Y);

    public void Deconstruct(out long x, out long y)
        => (x, y) = (X, Y);

    public struct PointOperator : IPointOperator<LongPoint2D>
    {
        // y
        // ^ 4 3 2
        // | 5 0 1
        // | 6 7 8
        // +-----> x
        private static int[] _signToIndex = new int[]
        {
            6, 7, 8,
            5, 0, 1,
            4, 3, 2,
        };

        private static int CoordinateToIndex(long x, long y)
        {
            var sx = Math.Sign(x) + 1;
            var sy = Math.Sign(y) + 1;
            return _signToIndex[sy * 3 + sx];
        }

        public int AngleSortCompare(LongPoint2D left, LongPoint2D right)
        {
            var uidx = CoordinateToIndex(left.X, left.Y);
            var vidx = CoordinateToIndex(right.X, right.Y);

            if (uidx != vidx)
                return uidx.CompareTo(vidx);

            var ccw = left.Y * right.X - right.Y * left.X;
            if (ccw == 0)
            {
                var lxy = left.X * left.X + left.Y * left.Y;
                var rxy = right.X * right.X + right.Y * right.Y;

                return lxy.CompareTo(rxy);
            }
            else
            {
                return ccw.CompareTo(0);
            }
        }
    }
}
