using UnityEngine;
using JPInputTool.Core;
using JPInputTool.Utils.DictionaryTools;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using JPInputTool.Utils;

public class Test : MonoBehaviour
{
    static string directlyPath = Path.Combine(Application.dataPath, @"dic/conversion");
    static string connectionPath = Path.Combine(Application.dataPath, @"dic/connection_single_column.txt");
    static string biGramFilePath = Path.Combine(Application.dataPath, @"dic/source/2gm.txt");
    static string biGramEngFilePath = Path.Combine(Application.dataPath, @"dic/source/2gm_english.csv");
    static string uniGramFilePath = Path.Combine(Application.dataPath, @"dic/source/1gm.csv");
    static string convFilePath = Path.Combine(Application.dataPath, @"dic/conversion.lptrie");
    static string biGramTriePath = Path.Combine(Application.dataPath, @"dic/bigram_jp.lptrie");
    static string biGramEngTriePath = Path.Combine(Application.dataPath, @"dic/bigram_eng.lptrie");
    static string uniGramTriePath = Path.Combine(Application.dataPath, @"dic/unigram_eng.lptrie");


    const string text = "わたしはあしたいきます";

    void Start()
    {
        var _ = MainTestAsync();
    }

    public async Task MainTestAsync()
    {
        await GenerateDictionariesAsync();

        // var converter = await JPConverter.CreateAsync(convFilePath, biGramTriePath, biGramEngTriePath, uniGramTriePath, connectionPath);
        // Debug.Log($"Dictionary Loaded.");

        // var results = converter.GetSuggestion(text);
        // Debug.Log($"suggest '{text}':");
        // PrintResults(results);

        // var results = converter.GetPrediction(text);
        // Debug.Log($"prediction '{text}':");
        // PrintResults(results);

        // var results = converter.GetPrediction(text);
        // Debug.Log($"prediction '{text}':");
        // PrintResults(results);

        // var results = converter.GetConversion(text, 100);
        // Debug.Log($"conversion '{text}':");
        // PrintConvResults(results);

        // var results = converter.GetCandidates(text, 50);
        // Debug.Log($"candidates '{text}':");
        // PrintCandidates(results);

        // var results = converter.GetEnglishSuggestion(text);
        // Debug.Log($"suggest '{text}':");
        // PrintResults(results);

    }

    private async Task GenerateDictionariesAsync()
    {
        // var convTrie = DictionaryGenerator.GenerateConversionDictionary(Path.Combine(Application.dataPath, directlyPath));
        // await DictionaryGenerator.SaveTrie(convTrie, Path.Combine(Application.dataPath, convFilePath));

        var biGramTrie = DictionaryGenerator.GenerateBiGramDictionary(Path.Combine(Application.dataPath, biGramFilePath));
        await DictionaryGenerator.SaveTrie(biGramTrie, Path.Combine(Application.dataPath, biGramTriePath));

        // var engUniGramTrie = DictionaryGenerator.GenerateEnglishUniGramDictionary(Path.Combine(Application.dataPath, uniGramFilePath));
        // await DictionaryGenerator.SaveTrie(engUniGramTrie, Path.Combine(Application.dataPath, uniGramTriePath));

        // var engBiGramTrie = DictionaryGenerator.GenerateEnglishBiGramDictionary(Path.Combine(Application.dataPath, biGramEngFilePath));
        // await DictionaryGenerator.SaveTrie(engBiGramTrie, Path.Combine(Application.dataPath, biGramEngTriePath));
    }

    private void PrintResults(List<ResultPair> results)
    {
        int i = 0;
        foreach (var item in results)
        {
            if (i >= 100) break;
            Debug.Log($"{i + 1:00} Result: {item.Cost:000000}, {item.Candidate}");
            i++;
        }
    }

    private void PrintConvResults(List<(ResultPair, string)> results)
    {
        int i = 0;
        foreach (var item in results)
        {
            if (i >= 100) break;
            Debug.Log($"{i + 1:00} Conversion: {item.Item1.Cost:000000}, {item.Item1.Candidate}, last: {item.Item2}");
            i++;
        }
    }

    private void PrintCandidates(List<(ResultPair, string, int)> results)
    {
        int i = 0;
        foreach (var item in results)
        {
            if (i >= 100) break;
            Debug.Log($"{i + 1:00} Candidate: {item.Item1.Cost:000000}, {item.Item1.Candidate}, last: {item.Item2}, length: {item.Item3}");
            i++;
        }
    }
}
