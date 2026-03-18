using System;

namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class MinSeg : MonoidSegTree<long, MinSeg.Operator>
{
    public MinSeg(int size) : base(size)
    {
    }

    public struct Operator : IMonoidSegTreeOperator<long>
    {
        public bool IsCommutative() => true;
        public long Identity() => Int64.MaxValue;
        public long Merge(long l, long r) => Math.Min(l, r);
    }
}
