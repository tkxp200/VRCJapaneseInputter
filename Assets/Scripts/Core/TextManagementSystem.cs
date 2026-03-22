using UnityEngine;
using OverlayInputUtil;
using TMPro;

public class TextManagementSystem : MonoBehaviour
{
    const char WHITE_CHAR = ' ';
    [SerializeField] CandidateSystem candidateSystem;
    [SerializeField] SendMessageSystem sendMessageSystem;
    [SerializeField] KeyUISystem keyUISystem;
    // [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI inputField;
    private string transLiteratedText = "";
    private string writingText = "";
    private int currentTransLiterateIndex;

    public void InitText()
    {
        transLiteratedText = "";
        writingText = "";
        ChangeInputFieldText(0);
    }

    public void AddText(string text)
    {
        writingText += text;
        ChangeInputFieldText(0);
    }

    public void PasteText(string text)
    {
        transLiteratedText += text;
        ChangeInputFieldText(0);
    }

    public void TextTransLiterate(string text, int size)
    {
        transLiteratedText += text;
        writingText = writingText.Substring(size);
        // buttonUISystem.SetTransLiterateButtonVisible();
        ChangeInputFieldText(0);
    }

    public void OnClickTransLiterateButton()
    {
        if (writingText.Length != 0)
        {
            // buttonUISystem.SetCursorButtonVisible();
            ChangeInputFieldText(writingText.Length);
        }
        else
        {
            transLiteratedText += WHITE_CHAR;
            ChangeInputFieldText(0);
        }
    }

    public void DeleteText()
    {
        if (writingText != "")
        {
            string oldText = writingText;
            writingText = oldText.Substring(0, oldText.Length - 1);
        }
        else if (transLiteratedText != "")
        {
            string oldText = transLiteratedText;
            transLiteratedText = oldText.Substring(0, oldText.Length - 1);
        }
        else return;
        ChangeInputFieldText(0);
    }

    public void AllDeleteText()
    {
        InitText();
    }

    public string GetCurrentText()
    {
        return transLiteratedText + writingText;
    }

    public string GetTransLiteratedText()
    {
        return transLiteratedText;
    }

    public void OnClickPasteButton()
    {
        PasteText(GUIUtility.systemCopyBuffer);
    }

    public void OnClickCopyButton()
    {
        EnterText();
        GUIUtility.systemCopyBuffer = GetCurrentText();
        InitText();
    }

    public void OnClickEnterButton(bool flag)
    {
        if (writingText.Length != 0) EnterText();
        else sendMessageSystem.SendMessage(flag);
    }

    public void EnterText()
    {
        transLiteratedText += writingText;
        writingText = "";
        ChangeInputFieldText(0);
    }

    public void ParseSmallText()
    {
        if(writingText != "")
        {
            string oldText = writingText;
            if(TextConversionUtil.smallTextDictionary.TryGetValue(oldText[oldText.Length-1], out var smallText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + smallText;
                ChangeInputFieldText(0);
            }
        }
    }

    public void ParseHandakutenText()
    {
        if(writingText != "")
        {
            string oldText = writingText;
            if(TextConversionUtil.handakutenTextDictionary.TryGetValue(oldText[oldText.Length-1], out var handakutenText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + handakutenText;
            }
            else
            {
                writingText = oldText + "゜";
            }
            ChangeInputFieldText(0);
        }
    }

    public void ParseDakutenText()
    {
        if(writingText != "")
        {
            string oldText = writingText;
            if(TextConversionUtil.dakutenTextDictionary.TryGetValue(oldText[oldText.Length-1], out var dakutenText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + dakutenText;
            }
            else
            {
                writingText = oldText + "゛";
            }
            ChangeInputFieldText(0);
        }
    }

    public void ParseAlphabetUpperLower()
    {
        if(writingText != "")
        {
            string oldText = writingText;
            if(TextConversionUtil.AlphabetConvertDictionary.TryGetValue(oldText[oldText.Length-1], out var convertedAlphabet))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + convertedAlphabet;
                ChangeInputFieldText(0);
            }
        }
    }

    public void OnClickParseButton()
    {
        if(writingText != "")
        {
            string oldText = writingText;
            if(TextConversionUtil.smallTextDictionary.TryGetValue(oldText[oldText.Length-1], out var smallText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + smallText;
            }
            else if(TextConversionUtil.dakutenTextDictionary.TryGetValue(oldText[oldText.Length-1], out var dakutenText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + dakutenText;
            }
            else if(TextConversionUtil.handakutenTextDictionary.TryGetValue(oldText[oldText.Length-1], out var handakutenText))
            {
                writingText = oldText.Substring(0, oldText.Length-1) + handakutenText;
            }
            else
            {
                ParseDakutenText();
                return;
            }
            ChangeInputFieldText(0);
        }
    }

    private void ChangeInputFieldText(int candidateIndex)
    {
        bool isNotTextEmpty = writingText != "" || transLiteratedText != "";
        // sendMessageSystem.SendChatTyping(isNotTextEmpty);
        // if (isNotTextEmpty) sendMessageSystem.SendMessage(true);

        var candidateText = writingText.Substring(0, candidateIndex);
        var remainingText = writingText.Substring(candidateIndex);
        inputField.text = $"{transLiteratedText}<u><mark=#00baf340>{candidateText}</mark>{remainingText}</u>";
        keyUISystem.UpdateInputFieldPosition();
        if (candidateIndex != 0)
        {
            // candidateSystem.GenerateCandidate($"{candidateText},");
            candidateSystem.GenerateConversion(candidateText);
            currentTransLiterateIndex = candidateIndex;
        }
        else
        {
            currentTransLiterateIndex = writingText.Length;
            candidateSystem.GenerateCandidate(writingText);
        }
    }

    public void OnClickLeftTransLiterateButton()
    {
        if(currentTransLiterateIndex-1 != 0)
        {
            currentTransLiterateIndex--;
            ChangeInputFieldText(currentTransLiterateIndex);
        }
    }

    public void OnClickRightTransLiterateButton()
    {
        if(currentTransLiterateIndex != writingText.Length)
        {
            currentTransLiterateIndex++;
            ChangeInputFieldText(currentTransLiterateIndex);
        }
        else
        {
            EnterText();
        }
    }

    void Start()
    {
        // inputField.ActivateInputField();
        InitText();
    }
}
