using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using LOUDSPatriciaTrie;

namespace JPInputTool.Utils.DictionaryTools
{

    public static class DictionaryGenerator
    {
        const char TAB_CHAR = '\t';
        const char WHITE_CHAR = ' ';
        const char COMMA_CHAR = ',';

        public static LOUDSTrie<MorphemeEntry> GenerateConversionDictionary(string directlyPath, int capacity = 1000000)
        {
            return GenerateConversionDictionary(Directory.EnumerateFiles(directlyPath), capacity);
        }

        public static LOUDSTrie<BiGramEntry> GenerateEnglishBiGramDictionary(string filePath, int capacity = 5000)
        {
            Dictionary<string, List<BiGramEntry>> list = new(capacity);
            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var lineSpan = line.AsSpan();

                int separateIndex = lineSpan.IndexOf(WHITE_CHAR);
                if (separateIndex < 0) continue;
                var firstSpan = lineSpan.Slice(0, separateIndex);
                lineSpan = lineSpan.Slice(separateIndex + 1);

                separateIndex = lineSpan.IndexOf(COMMA_CHAR);
                if (separateIndex < 0) continue;
                var secondSpan = lineSpan.Slice(0, separateIndex);
                lineSpan = lineSpan.Slice(separateIndex + 1);

                var first = firstSpan.ToString();
                BiGramEntry entry = new(secondSpan.ToString(), (int)(long.Parse(lineSpan) / 100));

                if (list.TryGetValue(first, out var entries))
                {
                    entries.Add(entry);
                }
                else
                {
                    list.Add(first, new() { entry });
                }
            }
            LOUDSTrie<BiGramEntry> trie = new(list);
            return trie;
        }

        public static LOUDSTrie<int> GenerateEnglishUniGramDictionary(string filePath, int capacity = 10000)
        {
            Dictionary<string, List<int>> list = new(capacity);
            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var lineSpan = line.AsSpan();

                int separateIndex = lineSpan.IndexOf(COMMA_CHAR);
                if (separateIndex < 0) continue;
                var firstSpan = lineSpan.Slice(0, separateIndex);
                lineSpan = lineSpan.Slice(separateIndex + 1);

                separateIndex = lineSpan.IndexOf(COMMA_CHAR);
                if (separateIndex < 0) continue;
                var secondSpan = lineSpan.Slice(0, separateIndex);

                var first = firstSpan.ToString();
                if (list.TryGetValue(first, out var entries))
                {
                    entries.Add((int)(long.Parse(secondSpan) / 100));
                }
                else
                {
                    list.Add(first, new() { (int)(long.Parse(secondSpan) / 100) });
                }
            }
            LOUDSTrie<int> trie = new(list);
            return trie;
        }

        public static LOUDSTrie<BiGramEntry> GenerateBiGramDictionary(string filePath, int capacity = 10000)
        {
            Dictionary<string, List<BiGramEntry>> list = new(capacity);
            foreach (var line in File.ReadLines(filePath))
            {
                var lineSpan = line.AsSpan();

                int separateIndex = lineSpan.IndexOf(WHITE_CHAR);
                if (separateIndex < 0) continue;
                var firstSpan = lineSpan.Slice(0, separateIndex);
                lineSpan = lineSpan.Slice(separateIndex + 1);

                separateIndex = lineSpan.IndexOf(TAB_CHAR);
                if (separateIndex < 0) continue;
                var secondSpan = lineSpan.Slice(0, separateIndex);
                lineSpan = lineSpan.Slice(separateIndex + 1);

                var first = firstSpan.ToString();
                BiGramEntry entry = new(secondSpan.ToString(), int.Parse(lineSpan));

                if (list.TryGetValue(first, out var entries))
                {
                    entries.Add(entry);
                }
                else
                {
                    list.Add(first, new() { entry });
                }
            }
            LOUDSTrie<BiGramEntry> trie = new(list);
            return trie;
        }

        public static LOUDSTrie<MorphemeEntry> GenerateConversionDictionary(IEnumerable<string> fileList, int capacity = 1000000)
        {
            Dictionary<string, List<MorphemeEntry>> list = new(capacity);
            foreach (var filename in fileList)
            {
                foreach (var line in File.ReadLines(filename))
                {
                    var lineSpan = line.AsSpan();

                    int tabIndex = lineSpan.IndexOf(TAB_CHAR);
                    if (tabIndex < 0) continue;
                    var keySpan = lineSpan.Slice(0, tabIndex);
                    lineSpan = lineSpan.Slice(tabIndex + 1);

                    tabIndex = lineSpan.IndexOf(TAB_CHAR);
                    if (tabIndex < 0) continue;
                    var leftId = int.Parse(lineSpan.Slice(0, tabIndex));
                    lineSpan = lineSpan.Slice(tabIndex + 1);

                    tabIndex = lineSpan.IndexOf(TAB_CHAR);
                    if (tabIndex < 0) continue;
                    var rightId = int.Parse(lineSpan.Slice(0, tabIndex));
                    lineSpan = lineSpan.Slice(tabIndex + 1);

                    tabIndex = lineSpan.IndexOf(TAB_CHAR);
                    if (tabIndex < 0) continue;
                    var cost = int.Parse(lineSpan.Slice(0, tabIndex));
                    lineSpan = lineSpan.Slice(tabIndex + 1);

                    var key = keySpan.ToString();
                    var entry = new MorphemeEntry(leftId, rightId, cost, lineSpan.ToString());

                    if (list.TryGetValue(key, out var entries))
                    {
                        entries.Add(entry);
                    }
                    else
                    {
                        list.Add(key, new() { entry });
                    }
                }
            }
            LOUDSTrie<MorphemeEntry> trie = new(list);
            return trie;
        }

        public static async Task SaveTrie<T>(LOUDSTrie<T> trie, string filePath)
        {
            await LOUDSTrieIO.SaveTrieAsync(trie, filePath);
        }

    }
}
