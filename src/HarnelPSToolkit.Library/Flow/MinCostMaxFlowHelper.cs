using System;
using System.Collections.Generic;

namespace HarnelPSToolkit.Library.Flow;

/// <summary>
/// FFK+SPFA
/// </summary>
[IncludeIfReferenced]
public class MinCostMaxFlowHelper<TNode>
    where TNode : struct
{
    private struct FlowEdge
    {
        public int Dst;
        public int Opp;
        public long CostPerFlow;
        public long Flow;
        public long Capacity;

        public FlowEdge(int dst, int opp, long costPerFlow, long capacity)
        {
            Dst = dst;
            Opp = opp;
            CostPerFlow = costPerFlow;
            Flow = 0;
            Capacity = capacity;
        }

        public bool CanFlow => Flow < Capacity;
    }

    private Dictionary<TNode, int> _map;
    private List<RefableList<FlowEdge>> _graph;

    public MinCostMaxFlowHelper()
    {
        _map = new Dictionary<TNode, int>();
        _graph = new List<RefableList<FlowEdge>>();
    }

    private int GetId(TNode node)
    {
        if (!_map.TryGetValue(node, out var v))
        {
            v = _map.Count;
            _map[node] = v;
            _graph.Add(new RefableList<FlowEdge>());
        }

        return v;
    }
    public void AddEdge(TNode src, TNode dst, long costPerFlow, long capacity)
    {
        var srcId = GetId(src);
        var dstId = GetId(dst);

        var srcopp = _graph[dstId].Count;
        var dstopp = _graph[srcId].Count;

        _graph[srcId].Add(new(dstId, srcopp, costPerFlow, capacity));
        _graph[dstId].Add(new(srcId, dstopp, -costPerFlow, 0));
    }

    public (long flowSum, long costSum) FindMinCostMaxFlow(TNode source, TNode sink)
    {
        var sourceId = GetId(source);
        var sinkId = GetId(sink);
        var flowSum = 0L;
        var costSum = 0L;

        var n = _graph.Count;
        var q = new Queue<int>();
        var spfa = new (long costPerFlow, long flow, int track, bool init, bool inq)[n];

        while (true)
        {
            SPFA(sourceId, sinkId, q, spfa);

            if (!spfa[sinkId].init)
                break;

            var cpf = spfa[sinkId].costPerFlow;
            var flow = spfa[sinkId].flow;

            var curr = sinkId;
            while (curr != sourceId)
            {
                var track = spfa[curr].track;
                ref var backward = ref _graph[curr][track];
                ref var forward = ref _graph[backward.Dst][backward.Opp];

                forward.Flow += flow;
                backward.Flow -= flow;
                curr = backward.Dst;
            }

            flowSum += flow;
            costSum += flow * cpf;
        }

        return (flowSum, costSum);
    }

    public (long flowSum, long costSum) FindMinCostAnyFlow(TNode source, TNode sink)
    {
        var sourceId = GetId(source);
        var sinkId = GetId(sink);
        var flowSum = 0L;
        var costSum = 0L;

        var n = _graph.Count;
        var q = new Queue<int>();
        var spfa = new (long costPerFlow, long flow, int track, bool init, bool inq)[n];

        while (true)
        {
            SPFA(sourceId, sinkId, q, spfa);

            if (!spfa[sinkId].init)
                break;

            var cpf = spfa[sinkId].costPerFlow;
            var flow = spfa[sinkId].flow;

            var curr = sinkId;
            while (curr != sourceId)
            {
                var track = spfa[curr].track;
                ref var backward = ref _graph[curr][track];
                ref var forward = ref _graph[backward.Dst][backward.Opp];

                forward.Flow += flow;
                backward.Flow -= flow;
                curr = backward.Dst;
            }

            if (cpf > 0)
                break;

            flowSum += flow;
            costSum += flow * cpf;
        }

        return (flowSum, costSum);
    }

    private void SPFA(int sourceId, int sinkId, Queue<int> q, (long costPerFlow, long flow, int track, bool init, bool inq)[] spfa)
    {
        Array.Clear(spfa);

        spfa[sourceId] = (0, Int64.MaxValue, 0, true, true);
        q.Enqueue(sourceId);

        while (q.TryDequeue(out var src))
        {
            spfa[src].inq = false;

            var g = _graph[src];
            var count = g.Count;
            for (var idx = 0; idx < count; idx++)
            {
                ref var forward = ref g[idx];

                if (!forward.CanFlow)
                    continue;

                var f = Math.Min(spfa[src].flow, forward.Capacity - forward.Flow);
                var cpf = spfa[src].costPerFlow + forward.CostPerFlow;
                ref var d = ref spfa[forward.Dst];

                if (!d.init || cpf < d.costPerFlow)
                {
                    d.flow = f;
                    d.init = true;
                    d.track = forward.Opp;
                    d.costPerFlow = cpf;

                    if (!d.inq)
                    {
                        d.inq = true;
                        q.Enqueue(forward.Dst);
                    }
                }
            }
        }
    }
}
