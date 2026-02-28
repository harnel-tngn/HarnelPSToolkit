using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HarnelPSToolkit.Library.Geometry;

public static class AngleSort
{
    private record struct AngleSortComparable<TElem, TPoint, TPointOperator>(TPoint Point) : IComparable<AngleSortComparable<TElem, TPoint, TPointOperator>>
        where TElem : struct, IComparable<TElem>, IEquatable<TElem>
        where TPoint : struct, IPoint2D<TElem, TPoint, TPointOperator>
        where TPointOperator : struct, IPointOperator<TPoint>
    {
        public int CompareTo(AngleSortComparable<TElem, TPoint, TPointOperator> other)
            => default(TPointOperator).AngleSortCompare(this.Point, other.Point);
    }

    public static void InplaceSort(this List<LongPoint2D> points)
        => InplaceSort<long, LongPoint2D, LongPoint2D.PointOperator>(CollectionsMarshal.AsSpan(points));

    public static void InplaceSort(this LongPoint2D[] points)
        => InplaceSort<long, LongPoint2D, LongPoint2D.PointOperator>(points.AsSpan());

    public static void InplaceSort<TElem, TPoint, TPointOperator>(this Span<TPoint> points)
        where TElem : struct, IComparable<TElem>, IEquatable<TElem>
        where TPoint : struct, IPoint2D<TElem, TPoint, TPointOperator>
        where TPointOperator : struct, IPointOperator<TPoint>
    {
        var castedSpan = MemoryMarshal.Cast<TPoint, AngleSortComparable<TElem, TPoint, TPointOperator>>(points);
        castedSpan.Sort();
    }
}
