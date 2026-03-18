using System;
using System.Collections.Generic;
using System.Linq;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public static class ValueCompressor
{
    public static (Dictionary<T, int> map, List<T> revmap) Compress<T>(IEnumerable<T> list)
    {
        var map = new Dictionary<T, int>();
        var revmap = new List<T>();

        var sorted = list.ToArray();
        Array.Sort(sorted);

        for (var idx = 0; idx < sorted.Length; idx++)
            if (map.TryAdd(sorted[idx], revmap.Count))
                revmap.Add(sorted[idx]);

        return (map, revmap);
    }
}
