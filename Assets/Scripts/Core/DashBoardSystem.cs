using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OverlayVRUtil;

public class DashBoardSystem : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] DashBoardUISystem dashBoardUISystem;
    [SerializeField] float size = 2.5f;
    [SerializeField] Color defaultButtonColor;
    [SerializeField] Color hoveredButtonColor;
    [SerializeField] Color pressedButtonColor;
    private ulong dashboardHandle = OpenVR.k_ulOverlayHandleInvalid;
    private ulong thumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
    private GameObject hoveredButtonObject;
    private GameObject pressedButtonObject;
    private bool isButtonPressed = false;

    void Start()
    {
        OverlaySystemUtil.InitOpenVR();
        (dashboardHandle, thumbnailHandle) = DashBoardUtil.CreateDashBoardOverlay("VRCJapaneseInputter_DBkey", "VRCJPInputter Setting");
        var filePath = Application.streamingAssetsPath + "/icon.png";
        DashBoardUtil.SetDashBoardIcon(thumbnailHandle, filePath);

        DashBoardUtil.SetDashBoardMouseScale(dashboardHandle, renderTexture);

        OverlayUtil.FlipOverlayVertical(dashboardHandle);
        OverlayUtil.SetOverlaySize(dashboardHandle, size);
    }

    void Update()
    {
        OverlayUtil.SetOverlayRenderTexture(dashboardHandle, renderTexture);

        var vrEvent = new VREvent_t();
        var uncbVREvent = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));

        while (OpenVR.Overlay.PollNextOverlayEvent(dashboardHandle, ref vrEvent, uncbVREvent))
        {
            switch ((EVREventType)vrEvent.eventType)
            {
                case EVREventType.VREvent_MouseMove:
                {
                    if(!isButtonPressed)
                    {
                        var hitPosition = new Vector2(vrEvent.data.mouse.x, renderTexture.height - vrEvent.data.mouse.y);
                        var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, hitPosition);
                        if(buttonObject != null) buttonObject.GetComponent<Image>().color = hoveredButtonColor;
                        if(hoveredButtonObject != null && buttonObject != hoveredButtonObject)
                            hoveredButtonObject.GetComponent<Image>().color = defaultButtonColor;
                        hoveredButtonObject = buttonObject;
                    }
                    break;
                }
                case EVREventType.VREvent_MouseButtonDown:
                {
                    if(!isButtonPressed && !dashBoardUISystem.GetAboutVisible())
                    {
                        isButtonPressed = true;
                        var hitPosition = new Vector2(vrEvent.data.mouse.x, renderTexture.height - vrEvent.data.mouse.y);
                        var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, hitPosition);
                        if(buttonObject != null)
                        {
                            pressedButtonObject = buttonObject;
                            buttonObject.GetComponent<Image>().color = pressedButtonColor;
                        }
                    }
                    break;
                }
                case EVREventType.VREvent_MouseButtonUp:
                {
                    if(isButtonPressed && !dashBoardUISystem.GetAboutVisible())
                    {
                        var hitPosition = new Vector2(vrEvent.data.mouse.x, renderTexture.height - vrEvent.data.mouse.y);
                        var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, hitPosition);
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
                        dashBoardUISystem.HideAbout();
                    }
                    break;
                }
            }
        }
    }

    public void OnClickButton()
    {
        Debug.Log("Pressed");
    }

    private void OnApplicationQuit()
    {
        OverlayUtil.DestroyOverlay(dashboardHandle);
    }

    private void OnDestroy()
    {
        OverlaySystemUtil.ShutdownOpenVR();
    }
}
