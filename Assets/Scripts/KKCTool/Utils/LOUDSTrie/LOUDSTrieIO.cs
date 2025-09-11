using System.Threading.Tasks;
using System.IO;
using MemoryPack;

namespace LOUDSPatriciaTrie
{

    public static class LOUDSTrieIO
    {
        public static async ValueTask SaveTrieAsync<T>(LOUDSTrie<T> trie, string filePath)
        {
            await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await MemoryPackSerializer.SerializeAsync(fs, trie);
            }
        }

        public static async ValueTask<LOUDSTrie<T>> LoadTrieAsync<T>(string filePath)
        {
            await using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var result = await MemoryPackSerializer.DeserializeAsync<LOUDSTrie<T>>(fs);
                if (result is null) throw new InvalidDataException("Failed to load LOUDSTrie.");
                return result;
            }
        }
    }
}
