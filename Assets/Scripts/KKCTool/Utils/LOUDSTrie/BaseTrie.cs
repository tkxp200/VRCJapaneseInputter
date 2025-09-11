using System;
using System.Collections.Generic;
using System.Linq;

namespace LOUDSTrieUtil
{

    public class BaseTrie<T>
    {
        private readonly BaseTrieNode rootNode;
        private List<T[]> keysets = null!;

        public BaseTrie(Dictionary<string, List<T>> keysets)
        {
            rootNode = new BaseTrieNode(null, false, 0);
            Build(keysets);
        }

        public List<T[]> Keysets()
        {
            return keysets;
        }

        public BaseTrieNode GetRootNode()
        {
            return rootNode;
        }

        private void Build(Dictionary<string, List<T>> keysets)
        {
            this.keysets = new(keysets.Count);
            int count = 0;
            foreach (var keyset in keysets.OrderBy(e => e.Key))
            {
                // this.keysets[count] = keyset.Value.ToArray();
                this.keysets.Add(keyset.Value.ToArray());
                BaseTrieNode prev = rootNode;
                BaseTrieNode current;
                for (int i = 0; i < keyset.Key.Length; i++)
                {
                    char c = keyset.Key[i];
                    if (prev.childs.TryGetValue(c, out var child))
                    {
                        prev = child;
                    }
                    else
                    {
                        current = new BaseTrieNode(c, i == keyset.Key.Length - 1, i == keyset.Key.Length - 1 ? count : -1);
                        prev.AddChild(c, current);
                        prev = current;
                    }
                }
                count++;
            }
        }


        private T[] Search(string key)
        {
            BaseTrieNode current = rootNode;
            foreach (var c in key)
            {
                if (current.childs.TryGetValue(c, out var next))
                {
                    current = next;
                }
                else
                {
                    return Array.Empty<T>();
                }
            }

            if (current.leaf && current.index >= 0) return keysets[(int)current.index];

            return Array.Empty<T>();
        }
    }
}
