using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OverlayVRUtil;

public class SettingPanelSystem : MonoBehaviour
{
    [SerializeField] SettingUISystem settingUISystem;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Color defaultButtonColor;
    [SerializeField] Color hoveredButtonColor;
    [SerializeField] Color pressedButtonColor;
    private GameObject hoveredButtonObject;
    private GameObject pressedButtonObject;
    private bool isButtonPressed = false;
    const string TAG = "HoverableButton";


    void Start()
    {
        actionSystem.OnTriggerDown += TriggerDown;
        actionSystem.OnTriggerUp += TriggerUp;
        actionSystem.OnHitPositionMove += HitPositionMove;
    }

    void HitPositionMove(Vector2? hitPosition)
    {
        if(hitPosition is not null && !isButtonPressed)
        {
            var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, (Vector2)hitPosition);
            if(hoveredButtonObject != null && buttonObject != hoveredButtonObject)
            {
                hoveredButtonObject.GetComponent<Image>().color = defaultButtonColor;
            }
            if(buttonObject != null && buttonObject.CompareTag(TAG))
            {
                buttonObject.GetComponent<Image>().color = hoveredButtonColor;
                hoveredButtonObject = buttonObject;
            }
        }

    }

    void TriggerDown(Vector2 hitPosition, GameObject buttonObject)
    {
        // if(!isButtonPressed && !settingUISystem.GetAboutVisible())
        if(!isButtonPressed)
        {
            isButtonPressed = true;
            if(buttonObject != null)
            {
                pressedButtonObject = buttonObject;
                // buttonObject.GetComponent<Image>().color = pressedButtonColor;
            }
        }
    }

    void TriggerUp(Vector2 hitPosition, GameObject buttonObject)
    {
        // if(isButtonPressed && !dashBoardUISystem.GetAboutVisible())
        if(isButtonPressed)
        {
            if(buttonObject != null && buttonObject == pressedButtonObject)
            {
                buttonObject.GetComponent<Button>().onClick.Invoke();
            }
            else if(pressedButtonObject != null)
            {
                pressedButtonObject.GetComponent<Image>().color = defaultButtonColor;
            }
            isButtonPressed = false;
        }
        else
        {
            // dashBoardUISystem.HideAbout();
        }
    }

    public void OnClickButton()
    {
        Debug.Log("Pressed");
    }
}
