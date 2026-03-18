using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HarnelPSToolkit.Library.LazySegTree;

[IncludeIfReferenced]
public abstract class GenericLazySeg<TElement, TUpdate, TLazy>
    where TElement : struct
    where TUpdate : struct
    where TLazy : struct
{
    protected TElement[] _tree;
    protected TLazy?[] _lazy;

    protected int _leafMask;

    private List<int> _path;
    private List<(int st, int idx)> _mergeIndices;

    public GenericLazySeg(int size)
    {
        _leafMask = (int)BitOperations.RoundUpToPowerOf2((uint)size);
        var treeSize = _leafMask << 1;

        _tree = new TElement[treeSize];
        _lazy = new TLazy?[treeSize];

        _path = new List<int>();
        _mergeIndices = new List<(int st, int idx)>();
    }

    public void Init(IList<TElement> init)
    {
        for (var idx = 0; idx < init.Count; idx++)
            _tree[_leafMask | idx] = init[idx];

        for (var idx = _leafMask - 1; idx > 0; idx--)
            _tree[idx] = MergeElement(_tree[2 * idx], _tree[2 * idx + 1]);
    }

    public void RangeUpdate(int stIncl, int edExcl, TUpdate val)
    {
        RangeUpdate(1, 0, _leafMask, stIncl, edExcl, val);
    }
    private void RangeUpdate(int idx, int lIncl, int rExcl, int stIncl, int edExcl, TUpdate val)
    {
        if (rExcl <= stIncl || edExcl <= lIncl)
            return;

        if (stIncl <= lIncl && rExcl <= edExcl)
        {
            var newLazy = CreateLazy(lIncl, rExcl, val);
            var prev = _lazy[idx];

            if (prev.HasValue)
                _lazy[idx] = MergeLazy(prev.Value, newLazy);
            else
                _lazy[idx] = newLazy;

            return;
        }

        ApplyAndPropagate(idx, lIncl, rExcl);

        var mid = (lIncl + rExcl) / 2;
        RangeUpdate(2 * idx, lIncl, mid, stIncl, edExcl, val);
        RangeUpdate(2 * idx + 1, mid, rExcl, stIncl, edExcl, val);

        if ((idx & _leafMask) == 0)
        {
            ApplyAndPropagate(2 * idx, lIncl, mid);
            ApplyAndPropagate(2 * idx + 1, mid, rExcl);
            _tree[idx] = MergeElement(_tree[2 * idx], _tree[2 * idx + 1]);
        }
    }

    public TElement RangeQuery(int stIncl, int edExcl)
    {
        var q = new Queue<(int idx, int lIncl, int rExcl)>();
        q.Enqueue((1, 0, _leafMask));

        _mergeIndices.Clear();

        while (q.TryDequeue(out var state))
        {
            var (idx, lIncl, rExcl) = state;
            if (rExcl <= stIncl || edExcl <= lIncl)
                continue;

            ApplyAndPropagate(idx, lIncl, rExcl);
            if (stIncl <= lIncl && rExcl <= edExcl)
            {
                _mergeIndices.Add((lIncl, idx));
                continue;
            }

            var mid = (lIncl + rExcl) / 2;
            q.Enqueue((2 * idx, lIncl, mid));
            q.Enqueue((2 * idx + 1, mid, rExcl));
        }

        _mergeIndices.Sort();
        var agg = _tree[_mergeIndices[0].idx];
        foreach (var (_, idx) in _mergeIndices.Skip(1))
            agg = MergeElement(agg, _tree[idx]);

        return agg;
    }

    private void ApplyAndPropagate(int idx, int stIncl, int edExcl)
    {
        var t = _tree[idx];
        var l = _lazy[idx];

        if (l == null)
            return;

        _tree[idx] = ApplyLazy(stIncl, edExcl, t, l.Value);

        if ((idx & _leafMask) == 0)
        {
            var (left, right) = SplitLazy(l.Value, stIncl, (stIncl + edExcl) / 2, edExcl);
            var (lprev, rprev) = (_lazy[2 * idx], _lazy[2 * idx + 1]);

            if (lprev == null)
                _lazy[2 * idx] = left;
            else
                _lazy[2 * idx] = MergeLazy(lprev.Value, left);

            if (rprev == null)
                _lazy[2 * idx + 1] = right;
            else
                _lazy[2 * idx + 1] = MergeLazy(rprev.Value, right);
        }

        _lazy[idx] = null;
    }

    protected abstract TLazy CreateLazy(int lIncl, int rExcl, TUpdate val);
    protected abstract TLazy MergeLazy(TLazy l, TLazy r);
    protected abstract TElement ApplyLazy(int lIncl, int rExcl, TElement element, TLazy lazy);
    protected abstract (TLazy left, TLazy right) SplitLazy(TLazy lazy, int stIncl, int mid, int edExcl);

    protected abstract TElement MergeElement(TElement l, TElement r);
}
