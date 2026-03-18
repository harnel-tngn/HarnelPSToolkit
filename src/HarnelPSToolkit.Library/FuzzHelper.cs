using System;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public static class FuzzHelper
{
    private static Random _rd;

    static FuzzHelper()
    {
        var seed = Random.Shared.Next();
        _rd = new Random(seed);
        Console.WriteLine($"Fuzz seed {seed}");
    }

    public static void SetSeed(int seed)
    {
        _rd = new Random(seed);
        Console.WriteLine($"Fuzz seed override: {seed}");
    }

    public static int RandInt(int minIncl, int maxExcl)
    {
        var val = _rd.Next(minIncl, maxExcl);
        Console.WriteLine(val);
        return val;
    }

    public static int RandInt(int maxExcl)
    {
        var val = _rd.Next(maxExcl);
        Console.WriteLine(val);
        return val;
    }

    public static string RandString(int maxLengthIncl, char[] chars)
    {
        var arr = new char[_rd.Next(1, maxLengthIncl + 1)];
        for (var idx = 0; idx < arr.Length; idx++)
            arr[idx] = chars[_rd.Next(chars.Length)];

        var str = new string(arr);
        Console.WriteLine(str);
        return str;
    }
}
