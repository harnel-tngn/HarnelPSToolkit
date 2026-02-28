using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HarnelPSToolkit.Benchmark.Benchmarks.Primitives;

[SimpleJob(RuntimeMoniker.Net60)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[DisassemblyDiagnoser]
[ReturnValueValidator]
public class CustomSortBenchnmark
{
    [Params(1000, 1_000_000)]
    public int Size { get; set; }

    private (int X, int Y)[] _data;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rd = new Random(42);

        _data = new (int X, int Y)[Size];
        for (var idx = 0; idx < Size; idx++)
            _data[idx] = (rd.Next(), rd.Next());
    }

    [Benchmark(Baseline = true)]
    public int LazyLinqSort()
    {
        var result = 0;
        foreach (var (x, y) in _data.OrderBy(v => v.X).ThenBy(v => v.Y))
            result = result * x + y;

        return result;
    }

    [Benchmark]
    public int EagerLinqSort()
    {
        var cloned = _data.OrderBy(v => v.X).ThenBy(v => v.Y).ToArray();

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }

    [Benchmark]
    public int LambdaComparerSort()
    {
        var cloned = _data.ToArray();
        Array.Sort(cloned, static (l, r) =>
        {
            var xcomp = l.X.CompareTo(r.X);

            if (xcomp == 0)
                return l.Y.CompareTo(r.Y);
            else
                return xcomp;
        });

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }

    private struct StructComparer : IComparer<(int X, int Y)>
    {
        public int Compare((int X, int Y) l, (int X, int Y) r)
        {
            var xcomp = l.X.CompareTo(r.X);

            if (xcomp == 0)
                return l.Y.CompareTo(r.Y);
            else
                return xcomp;
        }
    }

    [Benchmark]
    public int StructComparerArraySort()
    {
        var cloned = _data.ToArray();
        Array.Sort(cloned, new StructComparer());

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }

    [Benchmark]
    public int StructComparerSpanSort()
    {
        var cloned = _data.ToArray();
        cloned.AsSpan().Sort(new StructComparer());

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }

    private class ClassComparer : IComparer<(int X, int Y)>
    {
        public int Compare((int X, int Y) l, (int X, int Y) r)
        {
            var xcomp = l.X.CompareTo(r.X);

            if (xcomp == 0)
                return l.Y.CompareTo(r.Y);
            else
                return xcomp;
        }
    }

    [Benchmark]
    public int ClassComparerSort()
    {
        var cloned = _data.ToArray();
        Array.Sort(cloned, new ClassComparer());

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }

    private record struct XYPair(int X, int Y) : IComparable<XYPair>
    {
        public int CompareTo(XYPair other)
        {
            var xcomp = X.CompareTo(other.X);

            if (xcomp == 0)
                return Y.CompareTo(other.Y);
            else
                return xcomp;
        }
    }

    [Benchmark]
    public int ComparableSpanSort()
    {
        var cloned = _data.ToArray();
        var casted = MemoryMarshal.Cast<(int X, int Y), XYPair>(cloned.AsSpan());
        casted.Sort();

        var result = 0;
        foreach (var (x, y) in cloned)
            result = result * x + y;

        return result;
    }
}
