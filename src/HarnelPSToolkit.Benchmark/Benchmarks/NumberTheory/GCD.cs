using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace HarnelPSToolkit.Benchmark.Benchmarks.NumberTheory;

[SimpleJob(RuntimeMoniker.Net60)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[DisassemblyDiagnoser]
[ReturnValueValidator]
public class GCD
{
    [Params(1000, 1_000_000)]
    public int Size { get; set; }

    [Params(9, 18)]
    public int LogRange { get; set; }

    private (long X, long Y)[] _data;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rd = new Random(42);
        var range = (long)BigInteger.Pow(10, LogRange);

        _data = new (long X, long Y)[Size];
        for (var idx = 0; idx < Size; idx++)
            _data[idx] = (rd.NextInt64(range), rd.NextInt64(range));
    }

    [Benchmark(Baseline = true)]
    public long RecursiveGCD()
    {
        var xored = 0L;
        foreach (var (x, y) in _data)
            xored ^= GCD(x, y);

        return xored;

        static long GCD(long x, long y)
        {
            if (x == 0 || y == 0)
                return x | y;

            return GCD(y, x % y);
        }
    }

    [Benchmark]
    public long NonRecursiveGCD()
    {
        var xored = 0L;
        foreach (var (x, y) in _data)
            xored ^= GCD(x, y);

        return xored;

        static long GCD(long x, long y)
        {
            while (true)
            {
                if (x == 0 || y == 0)
                    return x | y;

                (x, y) = (y, x % y);
            }
        }
    }

    [Benchmark]
    public long BinaryGCD()
    {
        var xored = 0L;
        foreach (var (x, y) in _data)
            xored ^= (long)GCD((ulong)x, (ulong)y);

        return xored;

        // Credits to https://en.wikipedia.org/wiki/Binary_GCD_algorithm
        static ulong GCD(ulong u, ulong v)
        {
            var uv = u | v;
            if (u == 0 || v == 0)
                return uv;

            u >>= BitOperations.TrailingZeroCount(u) + 1;
            var tz = BitOperations.TrailingZeroCount(v);
            while (true)
            {
                v >>= tz + 1;
                v -= u;
                if (v == 0)
                    break;

                tz = BitOperations.TrailingZeroCount(v);
                var mask = (ulong)((long)v >> 63);
                u += v & mask;
                v ^= mask;
            }

            return (2 * u + 1) << BitOperations.TrailingZeroCount(uv);
        }
    }
}