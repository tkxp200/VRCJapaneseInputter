using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OverlayVRUtil;
using OverlayActionUtil;

public class ActionSystem : MonoBehaviour
{
    public event Action<Vector2, GameObject> OnTriggerDown;
    public event Action<Vector2, GameObject> OnTriggerUp;
    public event Action<Vector2?> OnHitPositionMove;
    public event Action<Vector2?> OnCursorPositionMove;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] private MainSystem mainSystem;
    [SerializeField] private OverlaySystem overlaySystem;
    private Vector2? currentHitPosition = null;
    private ulong rightTriggerActionHandle = 0;
    private ulong leftTriggerActionHandle = 0;
    private ulong showOverlayButtonHandle = 0;
    private ulong showOverlayStickHandle = 0;
    private ulong actionSetHandle = 0;
    private ulong overlayHandle;
    private bool isStickReseted = true;

    void Start()
    {
        OverlaySystemUtil.InitOpenVR();
        ActionUtil.SetActionManifest(Application.streamingAssetsPath + "/SteamVR/actions.json");
        actionSetHandle = ActionUtil.GetActionSetHandlePath("/actions/ControllerInput");
        rightTriggerActionHandle = ActionUtil.GetActionHandlePath($"/actions/ControllerInput/in/RightTriggerAction");
        leftTriggerActionHandle = ActionUtil.GetActionHandlePath($"/actions/ControllerInput/in/LeftTriggerAction");
        showOverlayButtonHandle = ActionUtil.GetActionHandlePath($"/actions/ControllerInput/in/ShowOverlayButton");
        showOverlayStickHandle = ActionUtil.GetActionHandlePath($"/actions/ControllerInput/in/ShowOverlayStick");
        overlayHandle = overlaySystem.GetOverlayHandle();
    }

    void Update()
    {
        if(overlaySystem.GetOverlayVisible())
        {
            UpdateHitPosition();
        }
        UpdateAction();
    }

    void UpdateHitPosition()
    {
        Vector2? hitPosition = OverlayUtil.GetOverlayIntersectionForController(overlayHandle, mainSystem.GetTrackHand());
        if(hitPosition != null)
        {
            hitPosition = hitPosition * mainSystem.GetWindowSize() + mainSystem.GetRaycastBias();
            if(hitPosition != currentHitPosition)
            {
                currentHitPosition = hitPosition;
                OnHitPositionMove?.Invoke((Vector2)hitPosition);
                OnCursorPositionMove?.Invoke((Vector2)hitPosition-mainSystem.GetRaycastBias());
            }
        }
        else
        {
            currentHitPosition = null;
            OnHitPositionMove?.Invoke(null);
            OnCursorPositionMove?.Invoke(null);
        }
    }

    void UpdateAction()
    {
        var actionSetList = new VRActiveActionSet_t[]
        {
            new VRActiveActionSet_t()
            {
                ulActionSet = actionSetHandle,
                ulRestrictedToDevice = OpenVR.k_ulInvalidInputValueHandle,
            }
        };

        InputDigitalActionData_t triggerResult = default;
        var activeActionSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRActiveActionSet_t));
        var err = OpenVR.Input.UpdateActionState(actionSetList, activeActionSize);
        ActionUtil.EVRInputErrThrowException(err, "アクションの更新に失敗しました");

        var showOverlayStickResult = ActionUtil.GetAnalogActionData(showOverlayStickHandle);
        var showOverlayButtonResult = ActionUtil.GetDigitalActionData(showOverlayButtonHandle);

        //スティックのy座標が-0.9以上の場合発動
        if(mainSystem.GetUseJoystick() && showOverlayStickResult.bActive && isStickReseted && showOverlayStickResult.y < -0.9f)
        {
            isStickReseted = false;
            overlaySystem.ShowOverlay();
        }
        else
        {
            isStickReseted = true;
        }

        if(mainSystem.GetUseJoystick() && showOverlayButtonResult.bState)
        {
            overlaySystem.ShowOverlay();
        }

        if(overlaySystem.GetOverlayVisible() && currentHitPosition != null)
        {
            if(mainSystem.GetTrackHand() == ETrackedControllerRole.RightHand)
            {
                triggerResult = ActionUtil.GetDigitalActionData(rightTriggerActionHandle);
            }
            else if(mainSystem.GetTrackHand() == ETrackedControllerRole.LeftHand)
            {
                triggerResult = ActionUtil.GetDigitalActionData(leftTriggerActionHandle);
            }
            else ActionUtil.EVRInputErrThrowException(err, "アクションの更新に失敗しました");

            if(triggerResult.bChanged && triggerResult.bState)
            {
                var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, (Vector2)currentHitPosition);
                OnTriggerDown?.Invoke((Vector2)currentHitPosition, buttonObject);
            }
            else if(triggerResult.bChanged && !triggerResult.bState)
            {
                var buttonObject = OverlayUtil.GetButtonObjectByPosition(eventSystem, graphicRaycaster, (Vector2)currentHitPosition);
                OnTriggerUp?.Invoke((Vector2)currentHitPosition, buttonObject);
            }
        }
    }

    private void Destroy()
    {
        OverlaySystemUtil.ShutdownOpenVR();
    }
}
