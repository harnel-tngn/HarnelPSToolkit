using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public class FFTHelper
{
    private long _mod;
    private long _mod2;
    private long _primitive;

    private long[] root;
    private long[] rootinv;
    private long[] rate2;
    private long[] rate2inv;
    private long[] rate3;
    private long[] rate3inv;

    public FFTHelper(long mod = 998244353, long primitive = 3)
    {
        _mod = mod;
        _mod2 = mod * mod;
        _primitive = primitive;

        var rank2 = 0;
        var modcopy = mod - 1;
        while (modcopy % 2 == 0)
        {
            rank2++;
            modcopy /= 2;
        }

        root = new long[rank2 + 1];
        rootinv = new long[rank2 + 1];

        root[rank2] = FastPow(_primitive, (_mod - 1) >> rank2);
        for (var idx = rank2 - 1; idx >= 0; idx--)
            root[idx] = root[idx + 1] * root[idx + 1] % _mod;

        for (var idx = 0; idx < root.Length; idx++)
            rootinv[idx] = FastPow(root[idx], _mod - 2);

        var rate2prod = 1L;
        rate2 = new long[rank2 - 1];
        rate2inv = new long[rank2 - 1];
        for (var idx = 0; idx <= rank2 - 2; idx++)
        {
            rate2[idx] = root[idx + 2] * rate2prod % _mod;
            rate2inv[idx] = FastPow(rate2[idx], _mod - 2);

            rate2prod = rate2prod * rootinv[idx + 2] % _mod;
        }

        var rate3prod = 1L;
        rate3 = new long[rank2 - 2];
        rate3inv = new long[rank2 - 2];
        for (var idx = 0; idx <= rank2 - 3; idx++)
        {
            rate3[idx] = root[idx + 3] * rate3prod % _mod;
            rate3inv[idx] = FastPow(rate3[idx], _mod - 2);

            rate3prod = rate3prod * rootinv[idx + 3] % _mod;
        }
    }

    private long FastPow(long a, long p)
    {
        var rv = 1L;
        while (p > 0)
        {
            if (p % 2 == 1)
                rv = rv * a % _mod;

            a = a * a % _mod;
            p /= 2;
        }

        return rv;
    }

    public List<long> Square(List<long> a)
    {
        var asize = a.Count;

        if (asize == 0)
            return a;

        if (asize <= 60)
            return ConvNaive(a, a);

        var logconvsize = 1 + BitOperations.Log2((uint)(2 * asize - 2));
        var convsize = 1 << logconvsize;

        while (a.Count != convsize)
            a.Add(0);

        Butterfly(a);

        for (var idx = 0; idx < convsize; idx++)
            a[idx] = a[idx] * a[idx] % _mod;

        ButterflyInv(a);

        var convsizeInv = FastPow(convsize, _mod - 2);
        for (var idx = 0; idx < convsize; idx++)
            a[idx] = (a[idx] * convsizeInv) % _mod;

        return a.Take(asize + asize - 1).ToList();
    }

    public List<long> Conv(List<long> a, List<long> b)
    {
        var asize = a.Count;
        var bsize = b.Count;

        if (asize == 0)
            return a;
        if (bsize == 0)
            return b;

        if (asize <= 60 && bsize <= 60)
            return ConvNaive(a, b);

        var logconvsize = 1 + BitOperations.Log2((uint)(asize + bsize - 2));
        var convsize = 1 << logconvsize;

        while (a.Count != convsize)
            a.Add(0);
        while (b.Count != convsize)
            b.Add(0);

        Butterfly(a);
        Butterfly(b);

        for (var idx = 0; idx < convsize; idx++)
            a[idx] = a[idx] * b[idx] % _mod;

        ButterflyInv(a);

        var convsizeInv = FastPow(convsize, _mod - 2);
        for (var idx = 0; idx < convsize; idx++)
            a[idx] = (a[idx] * convsizeInv) % _mod;

        return a.Take(asize + bsize - 1).ToList();
    }

    private List<long> ConvNaive(List<long> a, List<long> b)
    {
        if (a.Count < b.Count)
            return ConvNaive(b, a);

        var ans = new List<long>(a.Count + b.Count - 1);
        for (var idx = 0; idx < a.Count + b.Count - 1; idx++)
            ans.Add(0);

        for (var idx = 0; idx < a.Count; idx++)
            for (var jdx = 0; jdx < b.Count; jdx++)
                ans[idx + jdx] = (ans[idx + jdx] + a[idx] * b[jdx]) % _mod;

        return ans;
    }

    private void Butterfly(List<long> a)
    {
        var h = BitOperations.TrailingZeroCount((uint)a.Count);
        var len = 0;

        while (len < h)
        {
            if (h - len == 1)
            {
                var p = 1 << (h - len - 1);
                var rot = 1L;

                for (var s = 0; s < (1 << len); s++)
                {
                    var offset = s << (h - len);

                    for (var i = 0; i < p; i++)
                    {
                        var l = a[i + offset];
                        var r = a[i + offset + p] * rot % _mod;

                        a[i + offset] = (l + r) % _mod;
                        a[i + offset + p] = (_mod + l - r) % _mod;
                    }

                    if (s + 1 != (1 << len))
                        rot = rot * rate2[BitOperations.TrailingZeroCount(~s)] % _mod;
                }

                len++;
            }
            else
            {
                var p = 1 << (h - len - 2);
                var rot = 1L;
                var imag = root[2];

                for (var s = 0; s < (1 << len); s++)
                {
                    var rot2 = rot * rot % _mod;
                    var rot3 = rot * rot2 % _mod;
                    var offset = s << (h - len);

                    for (var idx = 0; idx < p; idx++)
                    {
                        var a0 = a[idx + offset];
                        var a1 = a[idx + offset + p] * rot % _mod;
                        var a2 = a[idx + offset + 2 * p] * rot2 % _mod;
                        var a3 = a[idx + offset + 3 * p] * rot3 % _mod;

                        // imag*(a1-a3)
                        var imaga1minusa3 = (_mod2 + a1 - a3) % _mod;
                        imaga1minusa3 *= imag;

                        // -a2
                        var a0minusa2 = a0 + _mod2 - a2;

                        a[idx + offset] = (a0 + a1 + a2 + a3) % _mod;
                        a[idx + offset + p] = (a0 + a2 + 2 * _mod2 - a1 - a3) % _mod;
                        a[idx + offset + 2 * p] = (a0minusa2 + imaga1minusa3) % _mod;
                        a[idx + offset + 3 * p] = (a0minusa2 + _mod2 - imaga1minusa3) % _mod;
                    }

                    if (s + 1 != (1 << len))
                        rot = rot * rate3[BitOperations.TrailingZeroCount(~s)] % _mod;
                }

                len += 2;
            }
        }
    }
    private void ButterflyInv(List<long> a)
    {
        var h = BitOperations.Log2((uint)a.Count);
        var len = h;

        while (len > 0)
        {
            if (len == 1)
            {
                var p = 1 << (h - len);
                var irot = 1L;

                for (var s = 0; s < (1 << (len - 1)); s++)
                {
                    var offset = s << (h - len + 1);

                    for (var idx = 0; idx < p; idx++)
                    {
                        var l = a[idx + offset];
                        var r = a[idx + offset + p];

                        a[idx + offset] = (l + r) % _mod;
                        a[idx + offset + p] = (_mod + l - r) * irot % _mod;
                    }

                    if (s + 1 != (1 << (len - 1)))
                        irot = irot * rate2inv[BitOperations.TrailingZeroCount(~s)] % _mod;
                }

                len--;
            }
            else
            {
                var p = 1 << (h - len);
                var irot = 1L;
                var iimag = rootinv[2];

                for (var s = 0; s < (1 << (len - 2)); s++)
                {
                    var irot2 = irot * irot % _mod;
                    var irot3 = irot * irot2 % _mod;
                    var offset = s << (h - len + 2);

                    for (var idx = 0; idx < p; idx++)
                    {
                        var a0 = a[idx + offset];
                        var a1 = a[idx + offset + p];
                        var a2 = a[idx + offset + 2 * p];
                        var a3 = a[idx + offset + 3 * p];

                        var v = (_mod + a2 - a3) * iimag % _mod;

                        a[idx + offset] = (a0 + a1 + a2 + a3) % _mod;
                        a[idx + offset + p] = (a0 + _mod - a1 + v) * irot % _mod;
                        a[idx + offset + 2 * p] = (a0 + a1 + 2 * _mod - a2 - a3) * irot2 % _mod;
                        a[idx + offset + 3 * p] = (a0 + 2 * _mod - a1 - v) * irot3 % _mod;
                    }

                    if (s + 1 != (1 << (len - 2)))
                        irot = irot * rate3inv[BitOperations.TrailingZeroCount(~s)] % _mod;
                }

                len -= 2;
            }
        }
    }
}
