namespace HarnelPSToolkit.Library.SegTree.Impl;

[IncludeIfReferenced]
public sealed class CountingSeg : AbelianGroupSegTree<int, int, int, CountingSeg.Operator>
{
    public CountingSeg(int size) : base(size)
    {
    }

    /// <summary>
    /// k is 0-based
    /// </summary>
    public bool TryGetKthElement(int k, out int index)
    {
        index = 0;
        if (k < 0 || Range(0, Size) <= k)
            return false;

        index = 1;
        k++;
        while (index < _leafMask)
        {
            var l = _tree[2 * index];
            if (l < k)
            {
                k -= l;
                index = 2 * index + 1;
            }
            else
            {
                index = 2 * index;
            }
        }

        index ^= _leafMask;
        return true;
    }

    public struct Operator : IAbelianGroupSegTreeOperator<int, int, int>
    {
        public int Identity() => 0;
        public int CreateDiff(int element, int val) => val - element;
        public int ApplyDiff(int element, int diff) => element + diff;
        public int Merge(int l, int r) => l + r;
    }
}
