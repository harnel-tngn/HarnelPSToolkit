using System;

namespace HarnelPSToolkit.Library.SegTree.Impl;

/// <summary>
/// Non-overlapping intervals
/// </summary>
[IncludeIfReferenced]
public class IntervalTree
{
    private IndexedMaxSeg<int> _seg;

    public IntervalTree(int size)
    {
        _seg = new IndexedMaxSeg<int>(size);
        _seg.Init(Int32.MinValue);
    }

    public void AddInterval(int stIncl, int edExcl)
    {
        _seg.Update(stIncl, edExcl);
    }

    public void RemoveInterval(int stIncl)
    {
        _seg.Update(stIncl, Int32.MinValue);
    }

    public bool TryGetInterval(int point, out int stIncl, out int edExcl)
    {
        (stIncl, edExcl) = _seg.Range(0, point + 1);
        return point < edExcl;
    }
}
