using System;
using System.Collections.Generic;

namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class IndexedMaxSeg<T> : MonoidSegTree<(int index, T value), IndexedMaxSeg<T>.Operator>
    where T : IComparable<T>
{
    public IndexedMaxSeg(int size) : base(size)
    {
        for (var idx = 0; idx < Size; idx++)
            _tree[_leafMask | idx].index = idx;
    }

    public void Init(T element)
    {
        for (var idx = 0; idx < _leafMask; idx++)
            _tree[_leafMask | idx].value = element;

        for (var idx = _leafMask - 1; idx > 0; idx--)
            _tree[idx] = default(Operator).Merge(_tree[2 * idx], _tree[2 * idx + 1]);
    }

    public void Init(IList<T> init)
    {
        if (init.Count > Size)
            throw new ArgumentException("Init list is too large.");

        for (var idx = 0; idx < init.Count; idx++)
            _tree[_leafMask | idx].value = init[idx];

        for (var idx = init.Count; idx < _leafMask; idx++)
            _tree[_leafMask | idx] = default(Operator).Identity();

        for (var idx = _leafMask - 1; idx > 0; idx--)
            _tree[idx] = default(Operator).Merge(_tree[2 * idx], _tree[2 * idx + 1]);
    }

    public void Update(int index, T value) => Update(index, (index, value));

    public struct Operator : IMonoidSegTreeOperator<(int index, T value)>
    {
        public bool IsCommutative() => true;

        public (int index, T value) Identity()
        {
            if (typeof(T) == typeof(int))
                return (-1, (T)(object)Int32.MinValue);
            if (typeof(T) == typeof(long))
                return (-1, (T)(object)Int64.MinValue);
            if (typeof(T) == typeof(float))
                return (-1, (T)(object)Single.MinValue);
            if (typeof(T) == typeof(double))
                return (-1, (T)(object)Double.MinValue);

            throw new NotImplementedException();
        }

        public (int index, T value) Merge((int index, T value) l, (int index, T value) r) => l.value.CompareTo(r.value) > 0 ? l : r;
    }
}
