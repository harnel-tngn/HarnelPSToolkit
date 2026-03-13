using System.Collections.Generic;

namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class RangeUpdatePointQuery
{
    private SumSeg _seg;

    public RangeUpdatePointQuery(int size)
    {
        _seg = new SumSeg(size);
    }


    public void Init(IList<long> init)
    {
        _seg.Init(init);
    }

    public void UpdateElement(int index, long value)
    {
        var diff = value - ElementAt(index);
        UpdateDiff(index, index + 1, diff);
    }

    public void UpdateDiff(int stIncl, int edExcl, long diff)
    {
        _seg.UpdateDiff(stIncl, diff);

        if (edExcl != _seg.Size)
            _seg.UpdateDiff(edExcl, -diff);
    }

    public long ElementAt(int index)
    {
        return _seg.Range(0, index + 1);
    }
}
