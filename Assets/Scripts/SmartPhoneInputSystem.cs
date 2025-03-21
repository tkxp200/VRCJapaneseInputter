using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlayVRUtil;
using UnityEngine.UI;
using OverlayInputUtil;
using SystemUtil;
using TMPro;

public class SmartPhoneInputSystem : MonoBehaviour
{
    [SerializeField] MainSystem mainSystem;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] TextManagementSystem textManagementSystem;
    [SerializeField] GameObject subButtonParent;
    [SerializeField] List<GameObject> subButton;
    private SmartPhoneInputUtil.SmartPhoneKeyType currentKeyType;
    private int currentSubKeyCount = 0;
    private SmartPhoneInputUtil.ButtonPosition currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.CENTER;
    private bool isMainButtonPressed = false;
    private Vector2 triggerDownPosition;
    private List<string> keys;
    private const string smartPhoneTag = "SmartPhoneButton";

    void Start()
    {
        actionSystem.OnTriggerDown += TriggerDown;
        actionSystem.OnTriggerUp += TriggerUp;
        actionSystem.OnHitPositionMove += HitPositionMove;
    }

    void HitPositionMove(Vector2? hitPosition)
    {
        if(isMainButtonPressed)
        {
            if(hitPosition != null)
            {
                var hitPositionDiff = (Vector2)hitPosition - triggerDownPosition;
                var x = Mathf.Abs(hitPositionDiff.x);
                var y = Mathf.Abs(hitPositionDiff.y);
                if(x >= y && x >= mainSystem.GetDragThreshold())
                {
                    if(hitPositionDiff.x < 0) currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.LEFT;
                    else currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.RIGHT;
                    ShowSubKeyIndex(currentButtonPosition);
                }
                else if(y > x && y >= mainSystem.GetDragThreshold())
                {
                    if(hitPositionDiff.y > 0) currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.TOP;
                    else if(currentSubKeyCount == subButton.Count) currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.BOTTOM;
                    ShowSubKeyIndex(currentButtonPosition);
                }
                else
                {
                    currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.CENTER;
                    ShowAllSubkey();
                }
            }
            else
            {
                isMainButtonPressed = false;
                HideAllSubkey();
            }
        }
    }

    void TriggerDown(Vector2 hitPosition, GameObject buttonObject)
    {
        triggerDownPosition = hitPosition;
        if(buttonObject != null && buttonObject.CompareTag(smartPhoneTag)) buttonObject.GetComponent<Button>().onClick.Invoke();
    }

    void TriggerUp(Vector2 hitPosition, GameObject buttonObject)
    {
        if(isMainButtonPressed)
        {
            isMainButtonPressed = false;
            if(currentKeyType == SmartPhoneInputUtil.SmartPhoneKeyType.DAKUTEN)
            {
                switch(currentButtonPosition)
                {
                    case SmartPhoneInputUtil.ButtonPosition.CENTER :
                    {
                        textManagementSystem.OnClickParseButton();
                        break;
                    }
                    case SmartPhoneInputUtil.ButtonPosition.LEFT :
                    {
                        textManagementSystem.ParseDakutenText();
                        break;
                    }
                    case SmartPhoneInputUtil.ButtonPosition.TOP :
                    {
                        textManagementSystem.AddText(keys[(int)currentButtonPosition]);
                        break;
                    }
                    case SmartPhoneInputUtil.ButtonPosition.RIGHT :
                    {
                        textManagementSystem.ParseHandakutenText();
                        break;
                    }
                }
            }
            else textManagementSystem.AddText(keys[(int)currentButtonPosition]);
            currentButtonPosition = SmartPhoneInputUtil.ButtonPosition.CENTER;
            HideAllSubkey();
        }
    }

    void ShowSubKeyIndex(SmartPhoneInputUtil.ButtonPosition index)
    {
        for (int i = 0; i < currentSubKeyCount; i++)
        {
            //ButtonPositionのCENTERは使わないので1減らす
            if(i != (int)index-1) subButton[i].SetActive(false);
            else subButton[i].SetActive(true);
        }
    }

    void ShowAllSubkey()
    {
        for (int i = 0; i < currentSubKeyCount; i++)
        {
            subButton[i].SetActive(true);
        }
    }

    void HideAllSubkey()
    {
        for (int i = 0; i < currentSubKeyCount; i++)
        {
            subButton[i].SetActive(false);
        }
    }

    public void ShowSubKeys(SmartPhoneInputUtil.SmartPhoneKeyType key, Vector3 mainButtonPosition)
    {
        if (!isMainButtonPressed)
        {
            isMainButtonPressed = true;
            currentKeyType = key;
            if(mainSystem.GetInputType() == MainSystemUtil.InputTypes.Hiragana)
                keys = SmartPhoneInputUtil.HiraganaSubKeys[key];
            else if(mainSystem.GetInputType() == MainSystemUtil.InputTypes.Katakana)
                keys = SmartPhoneInputUtil.KatakanaSubKeys[key];
            else if(mainSystem.GetInputType() == MainSystemUtil.InputTypes.Alphabet)
                keys = SmartPhoneInputUtil.AlphabetSubKeys[key];
            subButtonParent.transform.localPosition = mainButtonPosition;
            currentSubKeyCount = keys.Count-1; //CENTERは含めない

            for (int i = 0; i < currentSubKeyCount; i++)
            {
                subButton[i].SetActive(true);
                subButton[i].GetComponentInChildren<TextMeshProUGUI>().text = keys[i+1];//インデックス0(CENTER)の文字は表示しない
            }
        }
    }
}
