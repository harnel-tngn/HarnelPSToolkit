namespace HarnelPSToolkit.Library.SegTree;

[IncludeIfReferenced]
public interface IAbelianGroupSegTreeOperator<TElement, in TUpdate, TDiff>
{
    TElement Identity();
    TDiff CreateDiff(TElement element, TUpdate val);
    TElement ApplyDiff(TElement element, TDiff diff);
    TElement Merge(TElement l, TElement r);
}
