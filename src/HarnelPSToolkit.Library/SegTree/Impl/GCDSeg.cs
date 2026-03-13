using HarnelPSToolkit.Library.NumberTheory;

namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class GCDSeg : MonoidSegTree<long, GCDSeg.Operator>
{
    public GCDSeg(int size) : base(size)
    {
    }

    public struct Operator : IMonoidSegTreeOperator<long>
    {
        public bool IsCommutative() => true;
        public long Identity() => 0;
        public long Merge(long l, long r) => GCD.Eval(l, r);
    }
}
