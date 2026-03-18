namespace HarnelPSToolkit.Library.NumberTheory;

[IncludeIfReferenced]
public class FastPow
{
    public static long Eval(long b, long p, long mod)
    {
        var rv = 1L;
        while (p != 0)
        {
            if ((p & 1) == 1)
                rv = rv * b % mod;

            b = b * b % mod;
            p >>= 1;
        }

        return rv;
    }
}
