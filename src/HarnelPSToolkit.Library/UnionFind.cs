using System.Collections.Generic;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public class UnionFind
{
    private int[] _set;
    private int[] _rank;
    private List<int> _buffer;

    public UnionFind(int n)
    {
        _set = new int[n];
        _rank = new int[n];
        _buffer = new List<int>(n);

        Clear();
    }

    private void Clear()
    {
        for (var idx = 0; idx < _set.Length; idx++)
        {
            _set[idx] = idx;
            _rank[idx] = 1;
        }
    }

    public int Find(int v)
    {
        if (_set[v] == _set[_set[v]])
            return _set[v];

        var root = v;

        _buffer.Clear();
        do
        {
            _buffer.Add(root);
            root = _set[root];
        }
        while (root != _set[root]);

        foreach (var val in _buffer)
            _set[val] = root;

        return root;
    }

    public int Rank(int v)
    {
        return _rank[Find(v)];
    }

    public bool TryUnion(int l, int r)
    {
        var leftRoot = Find(l);
        var rightRoot = Find(r);

        if (leftRoot == rightRoot)
            return false;

        if (_rank[leftRoot] < _rank[rightRoot])
        {
            _set[leftRoot] = rightRoot;
            _rank[rightRoot] += _rank[leftRoot];
        }
        else
        {
            _set[rightRoot] = leftRoot;
            _rank[leftRoot] += _rank[rightRoot];
        }

        return true;
    }
}