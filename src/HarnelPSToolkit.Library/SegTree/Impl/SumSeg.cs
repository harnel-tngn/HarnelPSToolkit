namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class SumSeg : AbelianGroupSegTree<long, long, long, SumSeg.Operator>
{
    public SumSeg(int size) : base(size)
    {
    }

    public struct Operator : IAbelianGroupSegTreeOperator<long, long, long>
    {
        public long Identity() => 0;
        public long CreateDiff(long element, long val) => val - element;
        public long ApplyDiff(long element, long diff) => element + diff;
        public long Merge(long l, long r) => l + r;
    }
}
