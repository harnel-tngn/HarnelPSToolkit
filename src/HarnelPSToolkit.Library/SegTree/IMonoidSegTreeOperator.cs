namespace HarnelPSToolkit.Library.SegTree;

[IncludeIfReferenced]
public interface IMonoidSegTreeOperator<TElement>
{
    bool IsCommutative();
    TElement Identity();
    TElement Merge(TElement l, TElement r);
}