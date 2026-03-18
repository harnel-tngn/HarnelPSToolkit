using System;
using System.Collections.Generic;
using System.Linq;

namespace HarnelPSToolkit.Library.Graphs;

[IncludeIfReferenced]
public static class SCC
{
    public static (int[] sccIndices, int sccCount) TarjanToSccIndices(List<int>[] graph)
    {
        var n = graph.Length;

        var visStack = new Stack<int>();
        var sccIndices = new int[n];
        var nodes = new (bool instack, int relabel, int minreach)[n];

        var relabelCounter = 1;
        var sccCounter = 0;

        for (var idx = 0; idx < n; idx++)
            if (nodes[idx].relabel == 0)
                TarjanToSccIndicesDFS(graph, sccIndices, nodes, visStack, ref relabelCounter, ref sccCounter, idx);

        return (sccIndices, sccCounter);
    }

    public static List<int>[] SccIndicesToSccList(int[] sccIndices, int sccCount)
    {
        var sccs = new List<int>[sccCount];
        for (var idx = 0; idx < sccCount; idx++)
            sccs[idx] = new List<int>();

        for (var idx = 0; idx < sccIndices.Length; idx++)
            sccs[sccIndices[idx]].Add(idx);

        return sccs;
    }

    public static List<int>[] SccIndicesToSccGraph(List<int>[] graph, int[] sccIndices, int sccCount)
    {
        var sccGraph = new HashSet<int>[sccCount];
        for (var idx = 0; idx < sccCount; idx++)
            sccGraph[idx] = new HashSet<int>();

        for (var src = 0; src < graph.Length; src++)
            foreach (var dst in graph[src])
            {
                var srcscc = sccIndices[src];
                var dstscc = sccIndices[dst];
                if (srcscc == dstscc)
                    continue;

                sccGraph[srcscc].Add(dstscc);
            }

        return sccGraph.Select(v => v.ToList()).ToArray();
    }

    public static List<int>[] SccIndicesToSccRevGraph(List<int>[] graph, int[] sccIndices, int sccCount)
    {
        var sccRevGraph = new HashSet<int>[sccCount];
        for (var idx = 0; idx < sccCount; idx++)
            sccRevGraph[idx] = new HashSet<int>();

        for (var src = 0; src < graph.Length; src++)
            foreach (var dst in graph[src])
            {
                var srcscc = sccIndices[src];
                var dstscc = sccIndices[dst];
                if (srcscc == dstscc)
                    continue;

                sccRevGraph[dstscc].Add(srcscc);
            }

        return sccRevGraph.Select(v => v.ToList()).ToArray();
    }

    private static void TarjanToSccIndicesDFS(
        List<int>[] graph,
        int[] sccIndices,
        (bool instack, int relabel, int minreach)[] nodes,
        Stack<int> visStack,
        ref int relabelCounter,
        ref int sccCounter,
        int curr)
    {
        visStack.Push(curr);

        ref var info = ref nodes[curr];
        info.instack = true;
        info.relabel = relabelCounter++;
        info.minreach = info.relabel;

        foreach (var child in graph[curr])
        {
            ref var cinfo = ref nodes[child];

            if (cinfo.relabel == 0)
            {
                // Forward edge
                TarjanToSccIndicesDFS(graph, sccIndices, nodes, visStack, ref relabelCounter, ref sccCounter, child);
                info.minreach = Math.Min(info.minreach, cinfo.minreach);
            }
            else if (cinfo.instack)
            {
                // Backward edge
                info.minreach = Math.Min(info.minreach, cinfo.minreach);
            }
        }

        if (info.relabel == info.minreach)
        {
            while (true)
            {
                var top = visStack.Pop();
                sccIndices[top] = sccCounter;
                nodes[top].instack = false;

                if (top == curr)
                    break;
            }

            sccCounter++;
        }
    }
}
