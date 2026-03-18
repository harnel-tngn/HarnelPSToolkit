using System;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
internal class RefableList<T>
{
    private T[] _arr;
    public int Count { get; private set; }

    public RefableList() : this(16)
    {
    }
    public RefableList(int capacity)
    {
        _arr = new T[capacity];
    }

    public void Add(T elem)
    {
        if (Count == _arr.Length)
        {
            var newarr = new T[_arr.Length * 2];
            Array.Copy(_arr, newarr, _arr.Length);
            _arr = newarr;
        }

        _arr[Count++] = elem;
    }

    public ref T this[int idx] => ref _arr[idx];
}
