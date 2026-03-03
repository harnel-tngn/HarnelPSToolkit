using System;
using System.Collections.Generic;

namespace HarnelPSToolkit.Library.NumberTheory;

[IncludeIfReferenced]
public class Sieve
{
    private int _n;
    private bool[] _isPrime;

    public Sieve(int n)
    {
        _n = n;

        // 2k+1 maps to k
        _isPrime = new bool[1 + (n - 1) / 2];

        for (var x = 3; x <= n; x += 2)
            _isPrime[(x - 1) / 2] = true;

        var sqn = 1 + (int)Math.Sqrt(n);
        for (var x = 3; x <= sqn; x += 2)
            if (_isPrime[(x - 1) / 2])
            {
                for (var y = x * x; y <= n; y += 2 * x)
                    _isPrime[(y - 1) / 2] = false;
            }
    }

    public bool IsPrime(int x)
    {
        if (x % 2 == 0)
            return x == 2;

        return _isPrime[(x - 1) / 2];
    }

    public List<int> GetPrimeList()
    {
        var l = new List<int>();
        l.Add(2);

        for (var x = 3; x <= _n; x += 2)
            if (IsPrime(x))
                l.Add(x);

        return l;
    }
}
