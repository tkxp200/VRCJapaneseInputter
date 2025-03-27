using System;
using UnityEngine;
using Valve.VR;
using SystemUtil;

public class MainSystem : MonoBehaviour
{
    public readonly Vector2 windowSize = new Vector2(500, 420);
    public readonly Vector3 raycastBias = new Vector2(250, 210);
    [SerializeField] ButtonUISystem buttonUISystem;
    [SerializeField] DashBoardUISystem dashBoardUISystem;
    [SerializeField] TextManagementSystem textManagementSystem;
    [SerializeField] OverlaySystem overlaySystem;
    [SerializeField] SettingSystem settingSystem;
    public ETrackedControllerRole trackHand = ETrackedControllerRole.RightHand;
    private MainSystemUtil.InputTypes inputType = MainSystemUtil.InputTypes.Hiragana;
    private MainSystemUtil.TrackDevice trackDevice = MainSystemUtil.TrackDevice.WORLD;
    private MainSystemUtil.SendTarget sendTarget = MainSystemUtil.SendTarget.Chat;
    private int overlaySizex10;
    private int overlayDistancex100;
    private int dragThreshold;
    private bool isUseTransLiterate;

    public float GetOverlaySize()
    {
        return overlaySizex10/10f;
    }

    public float GetOverlayDistance()
    {
        return overlayDistancex100/100f;
    }

    public int GetOverlaySizex10()
    {
        return overlaySizex10;
    }

    public int GetOverlayDistancex100()
    {
        return overlayDistancex100;
    }

    public int GetDragThreshold()
    {
        return dragThreshold;
    }

    public bool GetUseTransLiterate()
    {
        return isUseTransLiterate;
    }

    public MainSystemUtil.InputTypes GetInputType()
    {
        return inputType;
    }

    public MainSystemUtil.SendTarget GetSendTarget()
    {
        return sendTarget;
    }

    public MainSystemUtil.TrackDevice GetTrackDevice()
    {
        return trackDevice;
    }

    public void ChangeSendTarget()
    {
        if(sendTarget == MainSystemUtil.SendTarget.Chat) sendTarget = MainSystemUtil.SendTarget.Window;
        else sendTarget = MainSystemUtil.SendTarget.Chat;
        buttonUISystem.ChangeSendButtonText(sendTarget);
    }

    public void ChangeInputType()
    {
        inputType = (MainSystemUtil.InputTypes)(((int)inputType + 1) % (Enum.GetNames(typeof(MainSystemUtil.InputTypes)).Length-1));
        buttonUISystem.SetButtonVisible(inputType);
    }

    public void ChangeInputTypeToNumber()
    {
        if(inputType != MainSystemUtil.InputTypes.Number)
        {
            inputType = MainSystemUtil.InputTypes.Number;
        }
        else
        {
            inputType = (MainSystemUtil.InputTypes)0;
        }
        buttonUISystem.SetButtonVisible(inputType);
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

    public void SetTrackDevice(MainSystemUtil.TrackDevice setTrackDevice)
    {
        trackDevice = setTrackDevice;
        dashBoardUISystem.ChangeTrackDeviceUI(trackDevice);
        settingSystem.SaveSetting();
    }

    public void IncreaseOverlaySize()
    {
        if(overlaySizex10 < 10) overlaySizex10++;
        dashBoardUISystem.ChangeSizeUI(overlaySizex10);
        overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void DecreaseOverlaySize()
    {
        if(overlaySizex10 > 1) overlaySizex10--;
        dashBoardUISystem.ChangeSizeUI(overlaySizex10);
        overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void SetOverlaySize(int setOverlaySizex10, bool isReload)
    {
        overlaySizex10 = setOverlaySizex10;
        dashBoardUISystem.ChangeSizeUI(overlaySizex10);
        if(isReload) overlaySystem.ChangeOverlaySize();
        settingSystem.SaveSetting();
    }

    public void IncreaseOverlayDistance()
    {
        if(overlayDistancex100 < 30) overlayDistancex100++;
        dashBoardUISystem.ChangeDistanceUI(overlayDistancex100);
        settingSystem.SaveSetting();
    }

    public void DecreaseOverlayDistance()
    {
        if(overlayDistancex100 > -30) overlayDistancex100--;
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
        if(dragThreshold < 100) dragThreshold+=5;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void DecreaseDragThreshold()
    {
        if(dragThreshold > 20) dragThreshold-=5;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void SetDragThreshold(int setThreshold)
    {
        dragThreshold = setThreshold;
        dashBoardUISystem.ChangeDragThresholdUI(dragThreshold);
        settingSystem.SaveSetting();
    }

    public void ChangeUseTransLiterate()
    {
        isUseTransLiterate = !isUseTransLiterate;
        dashBoardUISystem.ChangeActiveTransLiterate(isUseTransLiterate);
        settingSystem.SaveSetting();
    }

    public void SetUseTransLiterate(bool setUseTransLiterate)
    {
        isUseTransLiterate = setUseTransLiterate;
        dashBoardUISystem.ChangeActiveTransLiterate(isUseTransLiterate);
        settingSystem.SaveSetting();
    }
}
