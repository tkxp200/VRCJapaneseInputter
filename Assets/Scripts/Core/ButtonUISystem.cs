using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SystemUtil;
using TMPro;

public class ButtonUISystem : MonoBehaviour
{
    [SerializeField] GameObject numberButtonParent;
    [SerializeField] GameObject hiraganaButtonParent;
    [SerializeField] GameObject katakanaButtonParent;
    [SerializeField] GameObject alphabetButtonParent;
    [SerializeField] GameObject cursorButtonParent;
    [SerializeField] GameObject transLiterateButtonParent;
    [SerializeField] GameObject enterButton;
    [SerializeField] GameObject copyButton;
    [SerializeField] TextMeshProUGUI sendButtonText;

    public void ChangeSendButtonText(MainSystemUtil.SendTarget sendTarget)
    {
        if(sendTarget == MainSystemUtil.SendTarget.Chat) sendButtonText.text = "chat";
        else sendButtonText.text = "win";
    }

    public void SetButtonVisible(MainSystemUtil.InputTypes inputType)
    {
        switch(inputType)
        {
            case MainSystemUtil.InputTypes.Hiragana:
            {
                katakanaButtonParent.SetActive(false);
                alphabetButtonParent.SetActive(false);
                numberButtonParent.SetActive(false);
                hiraganaButtonParent.SetActive(true);
                break;
            }
            case MainSystemUtil.InputTypes.Katakana:
            {
                alphabetButtonParent.SetActive(false);
                numberButtonParent.SetActive(false);
                hiraganaButtonParent.SetActive(false);
                katakanaButtonParent.SetActive(true);
                break;
            }
            case MainSystemUtil.InputTypes.Alphabet:
            {
                katakanaButtonParent.SetActive(false);
                numberButtonParent.SetActive(false);
                hiraganaButtonParent.SetActive(false);
                alphabetButtonParent.SetActive(true);
                break;
            }
            case MainSystemUtil.InputTypes.Number:
            {
                katakanaButtonParent.SetActive(false);
                alphabetButtonParent.SetActive(false);
                hiraganaButtonParent.SetActive(false);
                numberButtonParent.SetActive(true);
                break;
            }
        }
    }

    public void SetCursorButtonVisible()
    {
        transLiterateButtonParent.SetActive(false);
        cursorButtonParent.SetActive(true);
    }

    public void SetTransLiterateButtonVisible()
    {
        cursorButtonParent.SetActive(false);
        transLiterateButtonParent.SetActive(true);
    }

    public void SetEnterButtonVisible()
    {
        copyButton.SetActive(false);
        enterButton.SetActive(true);
    }

    public void SetCopyButtonVisible()
    {
        enterButton.SetActive(false);
        copyButton.SetActive(true);
    }
}
