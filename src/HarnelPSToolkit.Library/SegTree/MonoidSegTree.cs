using System;
using System.Collections.Generic;
using System.Numerics;

namespace HarnelPSToolkit.Library.SegTree;

[IncludeIfReferenced]
public abstract class MonoidSegTree<TElement, TOperator>
    where TOperator : struct, IMonoidSegTreeOperator<TElement>
{
    protected TElement[] _tree;
    protected int _leafMask;

    public int Size { get; private set; }

    private readonly List<int> _lefts = new();
    private readonly List<int> _rights = new();

    protected MonoidSegTree(int size)
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

    public void Update(int index, TElement val)
    {
        var curr = _leafMask | index;
        _tree[curr] = val;
        curr >>= 1;

        while (curr != 0)
        {
            _tree[curr] = default(TOperator).Merge(_tree[2 * curr], _tree[2 * curr + 1]);
            curr >>= 1;
        }
    }

    public TElement Range(int stIncl, int edExcl)
    {
        if (stIncl >= _leafMask || edExcl > _leafMask)
            throw new ArgumentOutOfRangeException();

        var leftNode = _leafMask | stIncl;
        var rightNode = _leafMask | (edExcl - 1);

        if (default(TOperator).IsCommutative())
        {
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
        else
        {
            _lefts.Clear();
            _rights.Clear();

            while (leftNode <= rightNode)
            {
                if ((leftNode & 1) == 1)
                    _lefts.Add(leftNode++);
                if ((rightNode & 1) == 0)
                    _rights.Add(rightNode--);

                leftNode >>= 1;
                rightNode >>= 1;
            }

            var aggregated = default(TOperator).Identity();

            for (var idx = 0; idx < _lefts.Count; idx++)
                aggregated = default(TOperator).Merge(aggregated, _tree[_lefts[idx]]);

            for (var idx = _rights.Count - 1; idx >= 0; idx--)
                aggregated = default(TOperator).Merge(aggregated, _tree[_rights[idx]]);

            return aggregated;
        }
    }
}