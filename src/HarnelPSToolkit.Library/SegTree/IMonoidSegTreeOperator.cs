namespace HarnelPSToolkit.Library.SegTree;

[IncludeIfReferenced]
public interface IMonoidSegTreeOperator<TElement, in TUpdate>
{
    bool IsCommutative();

    TElement Identity();
    TElement UpdateElement(TElement elem, TUpdate update);
    TElement Merge(TElement l, TElement r);
}