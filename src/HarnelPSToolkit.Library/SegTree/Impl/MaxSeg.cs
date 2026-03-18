using System;

namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class MaxSeg : MonoidSegTree<long, MaxSeg.Operator>
{
    public MaxSeg(int size) : base(size)
    {
    }

    public struct Operator : IMonoidSegTreeOperator<long>
    {
        public bool IsCommutative() => true;
        public long Identity() => Int64.MinValue;
        public long Merge(long l, long r) => Math.Max(l, r);
    }
}
