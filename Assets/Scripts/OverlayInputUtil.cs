using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayInputUtil
{

    public static class TextConversionUtil
    {
        public static Dictionary<char, string> smallTextDictionary = new Dictionary<char, string>()
        {
            {'あ', "ぁ"},
            {'い', "ぃ"},
            {'う', "ぅ"},
            {'え', "ぇ"},
            {'お', "ぉ"},
            {'つ', "っ"},
            {'や', "ゃ"},
            {'ゆ', "ゅ"},
            {'よ', "ょ"},
            {'ア', "ァ"},
            {'イ', "ィ"},
            {'ウ', "ゥ"},
            {'エ', "ェ"},
            {'オ', "ォ"},
            {'ツ', "ッ"},
            {'ヤ', "ャ"},
            {'ユ', "ュ"},
            {'ヨ', "ョ"},
            {'A', "a"},
            {'B', "b"},
            {'C', "c"},
            {'D', "d"},
            {'E', "e"},
            {'F', "f"},
            {'G', "g"},
            {'H', "h"},
            {'I', "i"},
            {'J', "j"},
            {'K', "k"},
            {'L', "l"},
            {'M', "m"},
            {'N', "n"},
            {'O', "o"},
            {'P', "p"},
            {'Q', "q"},
            {'R', "r"},
            {'S', "s"},
            {'T', "t"},
            {'U', "u"},
            {'V', "v"},
            {'W', "w"},
            {'X', "x"},
            {'Y', "y"},
            {'Z', "z"}

        };

        public static Dictionary<char, string> dakutenTextDictionary = new Dictionary<char, string>()
        {
            {'ぁ', "あ゛"},
            {'ぃ', "い゛"},
            {'ぅ', "ゔ"},
            {'ぇ', "え゛"},
            {'ぉ', "お゛"},
            {'か', "が"},
            {'き', "ぎ"},
            {'く', "ぐ"},
            {'け', "げ"},
            {'こ', "ご"},
            {'さ', "ざ"},
            {'し', "じ"},
            {'す', "ず"},
            {'せ', "ぜ"},
            {'そ', "ぞ"},
            {'た', "だ"},
            {'ち', "ぢ"},
            {'つ', "づ"},
            {'っ', "づ"},
            {'づ', "っ゛"},
            {'て', "で"},
            {'と', "ど"},
            {'は', "ば"},
            {'ひ', "び"},
            {'ふ', "ぶ"},
            {'へ', "べ"},
            {'ほ', "ぼ"},
            {'ゃ', "や゛"},
            {'ゅ', "ゆ゛"},
            {'ょ', "よ゛"},
            {'ァ', "ア゛"},
            {'ィ', "イ゛"},
            {'ゥ', "ヴ"},
            {'ェ', "エ゛"},
            {'ォ', "オ゛"},
            {'カ', "ガ"},
            {'キ', "ギ"},
            {'ク', "グ"},
            {'ケ', "ゲ"},
            {'コ', "ゴ"},
            {'サ', "ザ"},
            {'シ', "ジ"},
            {'ス', "ズ"},
            {'セ', "ゼ"},
            {'ソ', "ゾ"},
            {'タ', "ダ"},
            {'チ', "ヂ"},
            {'ツ', "ヅ"},
            {'ッ', "ヅ"},
            {'ヅ', "ッ゛"},
            {'テ', "デ"},
            {'ト', "ド"},
            {'ハ', "バ"},
            {'ヒ', "ビ"},
            {'フ', "ブ"},
            {'ヘ', "ベ"},
            {'ホ', "ボ"},
            {'ャ', "ヤ゛"},
            {'ュ', "ユ゛"},
            {'ヨ', "ヨ゛"}
        };

        public static Dictionary<char, string> handakutenTextDictionary = new Dictionary<char, string>()
        {
            {'は', "ぱ"},
            {'ひ', "ぴ"},
            {'ふ', "ぷ"},
            {'へ', "ぺ"},
            {'ほ', "ぽ"},
            {'ば', "ぱ"},
            {'び', "ぴ"},
            {'ぶ', "ぷ"},
            {'べ', "ぺ"},
            {'ぼ', "ぽ"},
            {'ハ', "パ"},
            {'ヒ', "ピ"},
            {'フ', "プ"},
            {'ヘ', "ペ"},
            {'ホ', "ポ"},
            {'バ', "パ"},
            {'ビ', "ピ"},
            {'ブ', "プ"},
            {'ベ', "ペ"},
            {'ボ', "ポ"}
        };

        public static Dictionary<char, string> AlphabetConvertDictionary = new Dictionary<char, string>()
        {
            {'a', "A"},
            {'b', "B"},
            {'c', "C"},
            {'d', "D"},
            {'e', "E"},
            {'f', "F"},
            {'g', "G"},
            {'h', "H"},
            {'i', "I"},
            {'j', "J"},
            {'k', "K"},
            {'l', "L"},
            {'m', "M"},
            {'n', "N"},
            {'o', "O"},
            {'p', "P"},
            {'q', "Q"},
            {'r', "R"},
            {'s', "S"},
            {'t', "T"},
            {'u', "U"},
            {'v', "V"},
            {'w', "W"},
            {'x', "X"},
            {'y', "Y"},
            {'z', "Z"},
            {'A', "a"},
            {'B', "b"},
            {'C', "c"},
            {'D', "d"},
            {'E', "e"},
            {'F', "f"},
            {'G', "g"},
            {'H', "h"},
            {'I', "i"},
            {'J', "j"},
            {'K', "k"},
            {'L', "l"},
            {'M', "m"},
            {'N', "n"},
            {'O', "o"},
            {'P', "p"},
            {'Q', "q"},
            {'R', "r"},
            {'S', "s"},
            {'T', "t"},
            {'U', "u"},
            {'V', "v"},
            {'W', "w"},
            {'X', "x"},
            {'Y', "y"},
            {'Z', "z"}
        };
    }
    public static class SmartPhoneInputUtil
    {
        public enum ButtonPosition
        {
            CENTER, LEFT, TOP, RIGHT, BOTTOM
        }

        public enum SmartPhoneKeyType
        {
            A, K, S, T, N, H, M, Y, R, DAKUTEN, W, SYMBOL
        }

        public static Dictionary<SmartPhoneKeyType, List<string>> HiraganaSubKeys = new Dictionary<SmartPhoneKeyType, List<string>>()
        {
            { SmartPhoneKeyType.A, new List<string>       { "あ", "い", "う", "え", "お" } },
            { SmartPhoneKeyType.K, new List<string>       { "か", "き", "く", "け", "こ" } },
            { SmartPhoneKeyType.S, new List<string>       { "さ", "し", "す", "せ", "そ" } },
            { SmartPhoneKeyType.T, new List<string>       { "た", "ち", "つ", "て", "と" } },
            { SmartPhoneKeyType.N, new List<string>       { "な", "に", "ぬ", "ね", "の" } },
            { SmartPhoneKeyType.H, new List<string>       { "は", "ひ", "ふ", "へ", "ほ" } },
            { SmartPhoneKeyType.M, new List<string>       { "ま", "み", "む", "め", "も" } },
            { SmartPhoneKeyType.Y, new List<string>       { "や", "(", "ゆ",  ")", "よ" } },
            { SmartPhoneKeyType.R, new List<string>       { "ら", "り", "る", "れ", "ろ" } },
            { SmartPhoneKeyType.DAKUTEN, new List<string> { "小", "゛", "ー", "゜" } },
            { SmartPhoneKeyType.W, new List<string>       { "わ", "を", "ん", "～" } },
            { SmartPhoneKeyType.SYMBOL, new List<string>  { "、", "。", "？", "！" } }
        };

        public static Dictionary<SmartPhoneKeyType, List<string>> KatakanaSubKeys = new Dictionary<SmartPhoneKeyType, List<string>>()
        {
            { SmartPhoneKeyType.A, new List<string>       { "ア", "イ", "ウ", "エ", "オ" } },
            { SmartPhoneKeyType.K, new List<string>       { "カ", "キ", "ク", "ケ", "コ" } },
            { SmartPhoneKeyType.S, new List<string>       { "サ", "シ", "ス", "セ", "ソ" } },
            { SmartPhoneKeyType.T, new List<string>       { "タ", "チ", "ツ", "テ", "ト" } },
            { SmartPhoneKeyType.N, new List<string>       { "ナ", "ニ", "ヌ", "ネ", "ノ" } },
            { SmartPhoneKeyType.H, new List<string>       { "ハ", "ヒ", "フ", "ヘ", "ホ" } },
            { SmartPhoneKeyType.M, new List<string>       { "マ", "ミ", "ム", "ヌ", "モ" } },
            { SmartPhoneKeyType.Y, new List<string>       { "ヤ", "{", "ユ",  "}", "ヨ" } },
            { SmartPhoneKeyType.R, new List<string>       { "ラ", "リ", "ル", "レ", "ロ" } },
            { SmartPhoneKeyType.DAKUTEN, new List<string> { "゛", "小", "[", "゜" } },
            { SmartPhoneKeyType.W, new List<string>       { "ワ", "ヲ", "]", "ン" } },
            { SmartPhoneKeyType.SYMBOL, new List<string>  { "ー", ".", "!?", "￥" } }
        };

        public static Dictionary<SmartPhoneKeyType, List<string>> AlphabetSubKeys = new Dictionary<SmartPhoneKeyType, List<string>>()
        {
            { SmartPhoneKeyType.A, new List<string>       { "@", "/", ":", "_", "1" } },
            { SmartPhoneKeyType.K, new List<string>       { "a", "b", "c", "#", "2" } },
            { SmartPhoneKeyType.S, new List<string>       { "d", "e", "f", "$", "3" } },
            { SmartPhoneKeyType.T, new List<string>       { "g", "h", "i", "%", "4" } },
            { SmartPhoneKeyType.N, new List<string>       { "j", "k", "l", "^", "5" } },
            { SmartPhoneKeyType.H, new List<string>       { "m", "n", "o", "*", "6" } },
            { SmartPhoneKeyType.M, new List<string>       { "p", "q", "r", "s", "7" } },
            { SmartPhoneKeyType.Y, new List<string>       { "t", "u", "v",  "+", "8" } },
            { SmartPhoneKeyType.R, new List<string>       { "w", "x", "y", "z", "9" } },
            { SmartPhoneKeyType.W, new List<string>       { "”", "&", "0", "-" } },
            { SmartPhoneKeyType.SYMBOL, new List<string>  { "’", ".", ";", "|" } }
        };
    }
}
