using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System;

namespace HarnelPSToolkit.Benchmark.Benchmarks.Primitives;

[SimpleJob(RuntimeMoniker.Net60)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[DisassemblyDiagnoser]
[ReturnValueValidator]
public class MultConstMod
{
    [Params(99_999, 100_000, 1_000_000, 10_000_000)]
    public int N { get; set; }

    private const int Mod = 1_000_000_007;

    [Benchmark(Baseline = true)]
    public long Naive()
    {
        var fac = 1L;
        for (var v = 1; v <= N; v++)
            fac = fac * v % Mod;

        return fac;
    }

    [Benchmark]
    public long Barrett()
    {
        const long m = (long)(UInt64.MaxValue / Mod);

        var fac = 1L;
        for (var v = 1; v <= N; v++)
        {
            fac *= v;
            var hi = Math.BigMul(fac, m, out _);
            fac -= hi * Mod;
        }

        return fac;
    }

    [Benchmark]
    public long BarrettUnrollOutOrOrder8()
    {
        const long m = (long)(UInt64.MaxValue / Mod);

        var fac1 = 1L;
        var fac2 = 1L;
        var fac3 = 1L;
        var fac4 = 1L;
        var fac5 = 1L;
        var fac6 = 1L;
        var fac7 = 1L;
        var fac8 = 1L;

        int v;
        for (v = 1; v <= N - 8;)
        {
            fac1 *= v++;
            fac2 *= v++;
            fac3 *= v++;
            fac4 *= v++;
            fac5 *= v++;
            fac6 *= v++;
            fac7 *= v++;
            fac8 *= v++;

            fac1 -= Math.BigMul(fac1, m, out _) * Mod;
            fac2 -= Math.BigMul(fac2, m, out _) * Mod;
            fac3 -= Math.BigMul(fac3, m, out _) * Mod;
            fac4 -= Math.BigMul(fac4, m, out _) * Mod;
            fac5 -= Math.BigMul(fac5, m, out _) * Mod;
            fac6 -= Math.BigMul(fac6, m, out _) * Mod;
            fac7 -= Math.BigMul(fac7, m, out _) * Mod;
            fac8 -= Math.BigMul(fac8, m, out _) * Mod;
        }

        var fac = 1L;
        for (; v <= N; v++)
            fac = fac * v % Mod;

        fac = fac * fac1 % Mod;
        fac = fac * fac2 % Mod;
        fac = fac * fac3 % Mod;
        fac = fac * fac4 % Mod;
        fac = fac * fac5 % Mod;
        fac = fac * fac6 % Mod;
        fac = fac * fac7 % Mod;
        fac = fac * fac8 % Mod;

        return fac;
    }
}
