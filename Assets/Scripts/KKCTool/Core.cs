using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using LOUDSPatriciaTrie;
using JPInputTool.Utils.DictionaryTools;
using JPInputTool.Utils;
using System.Text;
using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace JPInputTool.Core
{

    public class JPConverter
    {
        // その品詞が文節の途中に出現するかどうかを返す関数
        // 辞書更新した際は確認必須
        // 詳細はMozc src/data/dictionary_oss/id.defを参照
        private static bool IsSegmentPartId(int id)
        {
            return id switch
            {
                >= 1    and <= 11   => true, // フィラ―
                >= 29   and <= 576  => true, // 助動詞、助詞、接尾動詞
                >= 1937 and <= 2038 => true, // 接尾名詞
                >= 2195 and <= 2389 => true, // 接尾形容詞
                >= 2592 and <= 2594 => true, // 接続詞
                >= 2642 and <= 2657 => true, // 記号(?,!,wなど)
                _ => false
            };
        }

        const int USE_PREDICTION_MIN_TEXT_LENGTH = 3;

        const string BOS_STR = "<S>";
        private LOUDSTrie<MorphemeEntry> conversionTrie = null!;
        private LOUDSTrie<BiGramEntry> biGramTrie = null!;
        private LOUDSTrie<BiGramEntry> engBiGramTrie = null!;
        private LOUDSTrie<int> uniGramTrie = null!;
        private int[,] connectionCosts = null!;

        private JPConverter() { }

        public static async Task<JPConverter> CreateAsync(string conversionTrieFilePath, string biGramFilePath, string biGramEngTriePath, string uniGramTriePath, string connectionCostFilePath)
        {
            JPConverter coreInstance = new();
            await coreInstance.InitAsync(conversionTrieFilePath, biGramFilePath, biGramEngTriePath, uniGramTriePath, connectionCostFilePath);
            return coreInstance;
        }

        private async Task InitAsync(string conversionTrieFilePath, string biGramFilePath, string biGramEngTriePath, string uniGramTriePath, string connectionCostFilePath)
        {
            LoadConnectionCost(connectionCostFilePath);
            await LoadTriesAsync(conversionTrieFilePath, biGramFilePath, biGramEngTriePath, uniGramTriePath);
        }

        private async Task LoadTriesAsync(string conversionTrieFilePath, string biGramFilePath, string biGramEngTriePath, string uniGramTriePath)
        {
            conversionTrie = await TrieLoader.LoadConversionTrie(conversionTrieFilePath);
            biGramTrie = await TrieLoader.LoadBiGramTrie(biGramFilePath);
            engBiGramTrie = await TrieLoader.LoadBiGramTrie(biGramEngTriePath);
            uniGramTrie = await TrieLoader.LoadUniGramTrie(uniGramTriePath);
        }

        private void LoadConnectionCost(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var idLength = int.Parse(lines[0]);
            connectionCosts = new int[idLength, idLength];
            for (int i = 0; i < idLength; ++i)
            {
                for (int j = 0; j < idLength; ++j)
                {
                    connectionCosts[i, j] = int.Parse(lines[idLength * i + j + 1]);
                }
            }
        }

        public List<(ResultPair, string, int)> GetCandidates(string text, int n = 10)
        {
            return GetCandidates(text.AsSpan(), n);
        }

        // TODO:コストによる候補提示を実装する
        public List<(ResultPair, string, int)> GetCandidates(ReadOnlySpan<char> text, int n = 10)
        {
            if (text.Length == 0) return new();
            List<(ResultPair, string, int)> results = new();

            if (text.Length >= USE_PREDICTION_MIN_TEXT_LENGTH)
            {
                var predResults = GetPrediction(text);
                int count = 0;
                int index = -1;
                while (count < predResults.Count && index + 1 < predResults.Count)
                {
                    index++;
                    results.Add((predResults[index], predResults[index].Candidate, text.Length));
                    count++;
                }
            }

            var convPaths = GetNBestPaths(text, n * 2);
            for (int i = 0; i < Math.Min(n * 2 - results.Count, convPaths.Count); i++)
            {
                StringBuilder builder = new();
                int origLength = 0;
                string lastWord = null;

                var item = convPaths[i].Item1[1];
                origLength += item.OrigLength;
                builder.Append(item.Entry.Surface);
                for(int j = 2; j <  convPaths[i].Item1.Count; j++)
                {
                    item = convPaths[i].Item1[j];
                    if (IsSegmentPartId(item.Entry.LeftId))
                    {
                        origLength += item.OrigLength;
                        builder.Append(item.Entry.Surface);
                    }
                    else
                    {
                        lastWord = convPaths[i].Item1[j - 1].Entry.Surface;
                        break;
                    }
                }
                results.Add((new(builder.ToString(), convPaths[i].Item2), lastWord, origLength));
            }

            for (int i = 0; i < Math.Min(n * 2 - results.Count, convPaths.Count); i++)
            {
                StringBuilder builder = new();
                foreach (var item in convPaths[i].Item1)
                {
                    builder.Append(item.Entry.Surface);
                }
                results.Add((new(builder.ToString(), convPaths[i].Item2), convPaths[i].Item1[^2].Entry.Surface, text.Length));
            }

            return results
                    .GroupBy(item => item.Item1.Candidate)
                    .Select(group => group.OrderBy(item => item.Item1.Cost).First())
                    .ToList();
        }

        public List<ResultPair> GetSuggestion(string text)
        {
            return GetSuggestion(text.AsSpan());
        }

        public List<ResultPair> GetSuggestion(ReadOnlySpan<char> text)
        {
            if (IsOnlyAlphaNumeric(text)) return GetEnglishSuggestion(text);
            else return GetJapaneseSuggestion(text);
        }

        public List<ResultPair> GetJapaneseSuggestion(ReadOnlySpan<char> text)
        {
            BiGramEntry[] results;
            if (text == "")
                results = biGramTrie.ExactMatchSearch(BOS_STR);
            else
                results = biGramTrie.ExactMatchSearch(text);
            if (results.Length == 0) return new();

            return results
                .OrderByDescending(e => e.Cost)
                .Select(e => new ResultPair(e.Next, e.Cost))
                .ToList();
        }

        public List<ResultPair> GetEnglishSuggestion(string text)
        {
            return GetEnglishSuggestion(text.AsSpan());
        }

        public List<ResultPair> GetEnglishSuggestion(ReadOnlySpan<char> text)
        {
            if (text.Length == 0) return new();
            var results = engBiGramTrie.ExactMatchSearch(text);
            if (results.Length == 0) return new();

            return results
                .OrderByDescending(e => e.Cost)
                .Select(e => new ResultPair(e.Next, e.Cost))
                .ToList();
        }

        public List<ResultPair> GetPrediction(string text)
        {
            return GetPrediction(text.AsSpan());
        }

        public List<ResultPair> GetPrediction(ReadOnlySpan<char> text)
        {
            if (IsOnlyAlphaNumeric(text)) return GetEnglishPrediction(text);
            else return GetJapanesePrediction(text);
        }

        public List<ResultPair> GetJapanesePrediction(ReadOnlySpan<char> text)
        {
            var results = conversionTrie.PredictiveSearch(text);
            if (results.Count == 0) return new();

            return results
                .SelectMany(result => result.Item2)
                .GroupBy(item => item.Surface)
                .Select(group => group.OrderBy(entry => entry.Cost).First())
                .OrderBy(entry => entry.Cost)
                .Select(entry => new ResultPair(entry.Surface, entry.Cost))
                .ToList();
        }

        public List<ResultPair> GetEnglishPrediction(string text)
        {
            return GetEnglishPrediction(text.AsSpan());
        }

        public List<ResultPair> GetEnglishPrediction(ReadOnlySpan<char> text)
        {
            var results = uniGramTrie.PredictiveSearch(text);
            if (results.Count == 0) return new();
            return results
                .Select(result => new ResultPair(result.Item1, result.Item2.Min()))
                .OrderByDescending(entry => entry.Cost)
                .ToList();
        }

        public List<(ResultPair, string)> GetConversion(string text, int n = 10)
        {
            return GetConversion(text.AsSpan(), n);
        }

        public List<(ResultPair, string)> GetConversion(ReadOnlySpan<char> text, int n = 50)
        {
            var nBests = GetNBestPaths(text, n);

            List<(ResultPair, string)> conversions = new();
            foreach (var nBest in nBests)
            {
                StringBuilder builder = new();
                foreach (var item in nBest.Item1)
                {
                    builder.Append(item.Entry.Surface);
                }
                conversions.Add((new(builder.ToString(), nBest.Item2), nBest.Item1[^2].Entry.Surface));
            }
            return conversions
                .GroupBy(result => result.Item1.Candidate)
                .Select(group => group.OrderBy(result => result.Item1.Cost).First())
                .ToList();
        }

        private List<(List<Node>, int)> GetNBestPaths(ReadOnlySpan<char> text, int n = 50)
        {
            // Node bos = new(default);
            Node bos = new(MorphemeEntry.Empty, 0);
            Node eos = new(MorphemeEntry.Empty, 0);

            var lattice = CreateLattice(text, bos, eos);
            return GetNBests(lattice, bos, eos, n);
        }

        private Lattice CreateLattice(ReadOnlySpan<char> text, Node bos, Node eos)
        {
            var parentNodeList = new List<Node>[text.Length + 1];
            Lattice lattice = new();
            parentNodeList[0] = new() { bos };
            int bestCost;

            for (int i = 0; i < text.Length; i++)
            {
                if (parentNodeList[i] is null) continue;

                var results = conversionTrie.CommonPrefixSearch(text.Slice(i));

                foreach (var result in results)
                {
                    foreach (var item in result.Item2)
                    {
                        Node node = new(item, result.Item1.Length);
                        if (parentNodeList[i + result.Item1.Length] is null) parentNodeList[i + result.Item1.Length] = new();
                        parentNodeList[i + result.Item1.Length].Add(node);
                        bestCost = int.MaxValue;
                        foreach (var parent in parentNodeList[i])
                        {
                            var connectionCost = connectionCosts[parent.Entry.RightId, node.Entry.LeftId];
                            lattice.AddEdge(parent, node, connectionCost);
                            bestCost = Math.Min(bestCost, parent.BestCost + connectionCost);
                        }
                        node.BestCost = bestCost + node.Entry.Cost;
                    }
                }
            }
            bestCost = int.MaxValue;
            if (parentNodeList[text.Length] is null) return lattice;
            foreach (var parent in parentNodeList[text.Length])
            {
                if (parent is null) return lattice;
                var connectionCost = connectionCosts[parent.Entry.RightId, eos.Entry.LeftId];
                lattice.AddEdge(parent, eos, connectionCost);
                bestCost = Math.Min(bestCost, parent.BestCost + connectionCost);
            }
            eos.BestCost = bestCost + eos.Entry.Cost;

            return lattice;
        }

        private static List<(List<Node>, int)> GetNBests(Lattice lattice, Node bos, Node eos, int n)
        {
            //node, path, BackwardCost, HeuristicCost
            ESarkis.PriorityQueue<PathNode> queue = new();
            List<(List<Node>, int)> results = new();

            queue.Enqueue(new(eos, null, 0), eos.BestCost);

            while (queue.Count > 0)
            {
                PathNode item;
                int cost;
                (item, cost) = queue.Dequeue();
                if (item.CurrentNode == bos)
                {
                    List<Node> nodes = new();
                    var current = item;
                    while (current is not null)
                    {
                        nodes.Add(current.CurrentNode);
                        current = current.Previous;
                    }
                    results.Add((nodes, cost));
                }
                else if(lattice.InEdges.TryGetValue(item.CurrentNode, out var edges))
                {
                    foreach (var edge in edges)
                    {
                        int backwardCost = item.CurrentNode.Entry.Cost + item.BackwardCost + edge.Cost;
                        int heuristicCost = backwardCost + edge.FromNode.BestCost;
                        queue.Enqueue(new(edge.FromNode, item, backwardCost), heuristicCost);
                    }
                }
                if (results.Count >= n) break;
            }

            return results;
        }

        private static bool IsOnlyAlphaNumeric(ReadOnlySpan<char> text)
        {
            var enc = Encoding.GetEncoding("Shift_JIS");
            return enc.GetByteCount(text) == text.Length;
        }
    }
}