using System.Collections.Generic;

namespace LOUDSTrieUtil
{

    public class BaseTrieNode
    {
        public Dictionary<char, BaseTrieNode> childs { get; } = new();
        public char? key { get; }
        public bool leaf { get; }
        public int index { get; }

        public BaseTrieNode(char? key, bool leaf, int index)
        {
            this.key = key;
            this.leaf = leaf;
            this.index = index;
        }

        public void AddChild(char c, BaseTrieNode child)
        {
            childs.Add(c, child);
        }
    }
}
