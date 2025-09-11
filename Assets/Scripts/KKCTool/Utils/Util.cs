#nullable enable
using System.Collections.Generic;
using MemoryPack;

namespace JPInputTool.Utils
{

    public readonly struct ResultPair
    {
        [MemoryPackOrder(0)]
        public readonly string Candidate;
        [MemoryPackOrder(1)]
        public readonly int Cost;

        public ResultPair(string candidate, int cost)
        {
            Candidate = candidate;
            Cost = cost;
        }
    }

    [MemoryPackable]
    public partial record MorphemeEntry
    {
        [MemoryPackOrder(0)]
        public int LeftId;
        [MemoryPackOrder(1)]
        public int RightId;
        [MemoryPackOrder(2)]
        public int Cost;
        [MemoryPackOrder(3)]
        public string Surface;

        public MorphemeEntry(int leftId, int rightId, int cost, string surface)
        {
            LeftId = leftId;
            RightId = rightId;
            Cost = cost;
            Surface = surface;
        }

        public static MorphemeEntry Empty => new MorphemeEntry(0, 0, 0, "");
    }

    [MemoryPackable]
    public partial record BiGramEntry
    {
        public string Next;
        public int Cost;

        public BiGramEntry(string next, int cost)
        {
            Next = next;
            Cost = cost;
        }
    }

    public class Node
    {
        public MorphemeEntry Entry { get; }
        public int BestCost;
        public int OrigLength;

        public Node(MorphemeEntry entry, int origLength)
        {
            Entry = entry;
            OrigLength = origLength;
        }
    }

    public class Edge
    {
        public Node FromNode { get; }
        public Node ToNode { get; }
        public int Cost { get; }

        public Edge(Node fromNode, Node toNode, int cost)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Cost = cost;
        }
    }

    public record PathNode
    {
        public Node CurrentNode;
        public PathNode? Previous;
        public int BackwardCost;

        public PathNode(Node current, PathNode? previous, int backwardCost)
        {
            CurrentNode = current;
            Previous = previous;
            BackwardCost = backwardCost;
        }
    };

    public class Lattice
    {
        public Dictionary<Node, List<Edge>> OutEdges { get; } = new();
        public Dictionary<Node, List<Edge>> InEdges { get; } = new();

        public void AddEdge(Node from, Node to, int cost)
        {
            var edge = new Edge(from, to, cost);

            if (!OutEdges.ContainsKey(from))
                OutEdges[from] = new();
            OutEdges[from].Add(edge);
            if (!InEdges.ContainsKey(to))
                InEdges[to] = new();
            InEdges[to].Add(edge);
        }
    }
}
