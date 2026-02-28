namespace HarnelPSToolkit.Library.Geometry;

[IncludeIfReferenced]
public interface IPointOperator<TPoint>
    where TPoint : struct
{
    int AngleSortCompare(TPoint left, TPoint right);
}
