using System;
using UnityEngine;
using Valve.VR;
using SystemUtil;

public class MainSystem : MonoBehaviour
{
    const string APP_KEY = "kars.overlay.vrcji";
    public readonly Vector2 windowSize = new Vector2(500, 420);
    public readonly Vector3 raycastBias = new Vector2(250, 210);
    [SerializeField] KeyUISystem keyUISystem;
    [SerializeField] DashBoardUISystem dashBoardUISystem;
    [SerializeField] TextManagementSystem textManagementSystem;
    [SerializeField] OverlaySystem overlaySystem;
    [SerializeField] SettingSystem settingSystem;
    public ETrackedControllerRole trackHand = ETrackedControllerRole.RightHand;
    private MainSystemUtil.InputTypes inputType = MainSystemUtil.InputTypes.Hiragana;
    // private MainSystemUtil.TrackDevice trackDevice = MainSystemUtil.TrackDevice.WORLD;
    private MainSystemUtil.SendTarget sendTarget = MainSystemUtil.SendTarget.Chat;
    private int overlaySizex100;
    private int overlayDistancex100;
    private int dragThreshold;
    private bool isUseJoystick;
    private bool isAutoLaunch;

    public float GetOverlaySize()
    {
        return overlaySizex100 / 100f;
    }

    public float GetOverlayDistance()
    {
        return overlayDistancex100 / 100f;
    }

    public int GetOverlaySizex100()
    {
        return overlaySizex100;
    }

    public int GetOverlayDistancex100()
    {
        return overlayDistancex100;
    }

    public int GetDragThreshold()
    {
        return dragThreshold;
    }

    public bool GetUseJoystick()
    {
        return isUseJoystick;
    }

    public bool GetAutoLaunch()
    {
        return isAutoLaunch;
    }

    public MainSystemUtil.InputTypes GetInputType()
    {
        return inputType;
    }

    public MainSystemUtil.SendTarget GetSendTarget()
    {
        return sendTarget;
    }

    // public MainSystemUtil.TrackDevice GetTrackDevice()
    // {
    //     return trackDevice;
    // }

    public void ChangeSendTarget()
    {
        if (sendTarget == MainSystemUtil.SendTarget.Chat) sendTarget = MainSystemUtil.SendTarget.Window;
        else sendTarget = MainSystemUtil.SendTarget.Chat;
        keyUISystem.ChangeSendButtonText(sendTarget);
    }

    public void ChangeInputType()
    {
        textManagementSystem.EnterText();
        inputType = (MainSystemUtil.InputTypes)(((int)inputType + 1) % Enum.GetNames(typeof(MainSystemUtil.InputTypes)).Length);
        keyUISystem.SetButtonVisible(inputType);
    }

    public void ChangeInputTypeToNumber()
    {
        if (inputType != MainSystemUtil.InputTypes.Number)
        {
            inputType = MainSystemUtil.InputTypes.Number;
        }
        else
        {
            inputType = (MainSystemUtil.InputTypes)0;
        }
        keyUISystem.SetButtonVisible(inputType);
    }

    public Vector2 GetWindowSize()
    {
        return windowSize;
    }

    public Vector2 GetRaycastBias()
    {
        return raycastBias;
    }

    public ETrackedControllerRole GetTrackHand()
    {
        return trackHand;
    }

    public void SetTrackHand(ETrackedControllerRole setTrackHand)
    {
        trackHand = setTrackHand;
        dashBoardUISystem.ChangeTrackHandUI(trackHand);
        settingSystem.SaveSetting();
    }

    // public void SetTrackDevice(MainSystemUtil.TrackDevice setTrackDevice)
    // {
    //     trackDevice = setTrackDevice;
    //     dashBoardUISystem.ChangeTrackDeviceUI(trackDevice);
    //     settingSystem.SaveSetting();
    // }

    public void IncreaseOverlaySize()
    {
        if (overlaySizex100 < 100) overlaySizex100++;
        dashBoardUISystem.ChangeSizeUI(overlaySizex100);
        overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void DecreaseOverlaySize()
    {
        if (overlaySizex100 > 10) overlaySizex100--;
        dashBoardUISystem.ChangeSizeUI(overlaySizex100);
        overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void SetOverlaySize(int setOverlaySizex100, bool isReload)
    {
        overlaySizex100 = setOverlaySizex100;
        dashBoardUISystem.ChangeSizeUI(overlaySizex100);
        if (isReload) overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void IncreaseOverlayDistance()
    {
        if (overlayDistancex100 < 30) overlayDistancex100++;
        dashBoardUISystem.ChangeDistanceUI(overlayDistancex100);
        settingSystem.SaveSetting();
    }

    public void DecreaseOverlayDistance()
    {
        if (overlayDistancex100 > -30) overlayDistancex100--;
        dashBoardUISystem.ChangeDistanceUI(overlayDistancex100);
        settingSystem.SaveSetting();
    }

    public void SetOverlayDistance(int setOverlayDistancex100)
    {
        overlayDistancex100 = setOverlayDistancex100;
        dashBoardUISystem.ChangeDistanceUI(overlayDistancex100);
        settingSystem.SaveSetting();
    }

    public void IncreaseDragThreshold()
    {
        if (dragThreshold < 100) dragThreshold += 5;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void DecreaseDragThreshold()
    {
        if (dragThreshold > 20) dragThreshold -= 5;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void SetDragThreshold(int setThreshold)
    {
        dragThreshold = setThreshold;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void ChangeUseJoystick()
    {
        isUseJoystick = !isUseJoystick;
        dashBoardUISystem.ChangeActiveJoystick(isUseJoystick);
        settingSystem.SaveSetting();
    }

    public void SetUseJoystick(bool setUseJoystick)
    {
        isUseJoystick = setUseJoystick;
        dashBoardUISystem.ChangeActiveJoystick(isUseJoystick);
        settingSystem.SaveSetting();
    }

    public void ChangeAutoLaunch()
    {
        isAutoLaunch = !isAutoLaunch;
        OpenVR.Applications.SetApplicationAutoLaunch(APP_KEY, isAutoLaunch);
        dashBoardUISystem.ChangeActiveAutoLaunch(isAutoLaunch);
        settingSystem.SaveSetting();
    }

    public void SetAutoLaunch(bool setAutoLaunch)
    {
        isAutoLaunch = setAutoLaunch;
        OpenVR.Applications.SetApplicationAutoLaunch(APP_KEY, isAutoLaunch);
        dashBoardUISystem.ChangeActiveAutoLaunch(isAutoLaunch);
        settingSystem.SaveSetting();
    }
}
