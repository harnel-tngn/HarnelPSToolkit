using System;
using System.Numerics;

namespace HarnelPSToolkit.Library.NumberTheory;

[IncludeIfReferenced]
public class GCD
{
    public static long Eval(long u, long v)
    {
        return (long)Eval((ulong)Math.Abs(u), (ulong)Math.Abs(v));
    }

    // Credits to https://en.wikipedia.org/wiki/Binary_GCD_algorithm
    public static ulong Eval(ulong u, ulong v)
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