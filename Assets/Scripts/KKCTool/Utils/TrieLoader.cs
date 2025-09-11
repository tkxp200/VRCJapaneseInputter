using System.Threading.Tasks;
using LOUDSPatriciaTrie;

namespace JPInputTool.Utils.DictionaryTools
{

    public static class TrieLoader
    {
        public static async ValueTask<LOUDSTrie<MorphemeEntry>> LoadConversionTrie(string filePath)
        {
            return await LOUDSTrieIO.LoadTrieAsync<MorphemeEntry>(filePath);
        }

        public static async ValueTask<LOUDSTrie<BiGramEntry>> LoadBiGramTrie(string filePath)
        {
            return await LOUDSTrieIO.LoadTrieAsync<BiGramEntry>(filePath);
        }

        public static async ValueTask<LOUDSTrie<int>> LoadUniGramTrie(string filePath)
        {
            return await LOUDSTrieIO.LoadTrieAsync<int>(filePath);
        }
    }
}