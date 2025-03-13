using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityWebRequestUtil;

namespace CGITransLiterateUtil
{
    public static class TransLiterateSystem
    {
        private const string apiUrl = "https://www.google.com/transliterate";

        public static async Task<Newtonsoft.Json.Linq.JArray> GetJapaneseConversionAsync(string input)
        {
            string url = $"{apiUrl}?langpair=ja-Hira|ja&text={UnityWebRequest.EscapeURL(input)}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var result =  JsonConvert.DeserializeObject<List<List<object>>>(request.downloadHandler.text);
                    var item = result[0][1] as Newtonsoft.Json.Linq.JArray;
                    item.AddFirst(result[0][0].ToString());
                    return item;
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    return null;
                }
            }
        }
    }
}