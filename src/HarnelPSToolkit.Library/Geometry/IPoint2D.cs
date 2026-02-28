using System;

namespace HarnelPSToolkit.Library.Geometry;

public interface IPoint2D<TElem, TSelf, TOperator>
    where TElem : struct, IComparable<TElem>, IEquatable<TElem>
{
    TElem X { get; }
    TElem Y { get; }
}
