using System;
using System.Collections.Generic;
using System.Numerics;

namespace HarnelPSToolkit.Library.SegTree;

[IncludeIfReferenced]
public abstract class AbelianGroupSegTree<TElement, TUpdate, TDiff, TOperator>
    where TOperator : struct, IAbelianGroupSegTreeOperator<TElement, TUpdate, TDiff>
{
    protected TElement[] _tree;
    protected int _leafMask;

    public int Size { get; }

    protected AbelianGroupSegTree(int size)
    {
        _leafMask = (int)BitOperations.RoundUpToPowerOf2((uint)size);
        var treeSize = _leafMask << 1;

        _tree = new TElement[treeSize];
        Size = size;

        Array.Fill(_tree, default(TOperator).Identity());
    }

    public TElement AllRange => _tree[1];
    public TElement ElementAt(int idx) => _tree[_leafMask | idx];

    public void Init(TElement element)
    {
        for (var idx = 0; idx < Size; idx++)
            _tree[_leafMask | idx] = element;

        for (var idx = _leafMask - 1; idx > 0; idx--)
            _tree[idx] = default(TOperator).Merge(_tree[2 * idx], _tree[2 * idx + 1]);
    }

    public void Init(IList<TElement> init)
    {
        if (init.Count > Size)
            throw new ArgumentException("Init list is too large.");

        for (var idx = 0; idx < init.Count; idx++)
            _tree[_leafMask | idx] = init[idx];

        for (var idx = init.Count; idx < Size; idx++)
            _tree[_leafMask | idx] = default(TOperator).Identity();

        for (var idx = _leafMask - 1; idx > 0; idx--)
            _tree[idx] = default(TOperator).Merge(_tree[2 * idx], _tree[2 * idx + 1]);
    }

    public void UpdateValue(int index, TUpdate val)
    {
        var curr = _leafMask | index;
        var diff = default(TOperator).CreateDiff(_tree[curr], val);

        UpdateDiff(index, diff);
    }

    public void UpdateDiff(int index, TDiff diff)
    {
        var curr = _leafMask | index;

        while (curr != 0)
        {
            _tree[curr] = default(TOperator).ApplyDiff(_tree[curr], diff);
            curr >>= 1;
        }
    }

    public TElement Range(int stIncl, int edExcl)
    {
        if (stIncl >= _leafMask || edExcl > _leafMask)
            throw new ArgumentOutOfRangeException();

        var leftNode = _leafMask | stIncl;
        var rightNode = _leafMask | (edExcl - 1);

        var aggregated = default(TOperator).Identity();

        while (leftNode <= rightNode)
        {
            if ((leftNode & 1) == 1)
                aggregated = default(TOperator).Merge(aggregated, _tree[leftNode++]);
            if ((rightNode & 1) == 0)
                aggregated = default(TOperator).Merge(aggregated, _tree[rightNode--]);

            leftNode >>= 1;
            rightNode >>= 1;
        }

        return aggregated;
    }
}