namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class MultSeg : MonoidSegTree<long, MultSeg.Operator>
{
    public MultSeg(int size) : base(size)
    {
    }

    public struct Operator : IMonoidSegTreeOperator<long>
    {
        public bool IsCommutative() => true;
        public long Identity() => 1;
        public long Merge(long l, long r) => l * r;
    }
}
