namespace HarnelPSToolkit.Library.Geometry;

public interface IPointOperator<TPoint>
    where TPoint : struct
{
    int AngleSortCompare(TPoint left, TPoint right);
}
