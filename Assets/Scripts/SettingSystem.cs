using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using SystemUtil;

public class SettingSystem : MonoBehaviour
{
    [SerializeField] MainSystem mainSystem;
    private string trackHandKey = "TrackHand";
    private ETrackedControllerRole defaultTrackHand = ETrackedControllerRole.RightHand;
    private string trackDeviceKey = "TrackDevice";
    private MainSystemUtil.TrackDevice defaultTrackDevice = MainSystemUtil.TrackDevice.HMD;
    private string overlaySizeKey = "OverlaySizex10";
    private int defaultOverlaySizex10 = 3;
    private string overlayDistanceKey = "OverlayDistancex100";
    private int defaultOverlayDistancex100 = 10;
    private string dragThresholdKey = "DragThreshold";
    private int defaultDragThreshold = 40;
    private string isUseTransLiterateKey = "UseTransLiterate";
    private bool defaultIsUseTransLiterate = false;

    void Awake()
    {
        SetLoadSettings();
    }

    private void SetLoadSettings()
    {
        ETrackedControllerRole trackHandSetting =  (ETrackedControllerRole)PlayerPrefs.GetInt(trackHandKey, (int)defaultTrackHand);
        MainSystemUtil.TrackDevice trackDeviceSetting = (MainSystemUtil.TrackDevice)PlayerPrefs.GetInt(trackDeviceKey, (int)defaultTrackDevice);
        int overlaySizex10Setting = PlayerPrefs.GetInt(overlaySizeKey, defaultOverlaySizex10);
        int overlayDistancex100Setting = PlayerPrefs.GetInt(overlayDistanceKey, defaultOverlayDistancex100);
        int dragThresholdSetting = PlayerPrefs.GetInt(dragThresholdKey, defaultDragThreshold);
        bool isUseTransLiterateSettig = Convert.ToBoolean(PlayerPrefs.GetInt(isUseTransLiterateKey, Convert.ToInt32(defaultIsUseTransLiterate)));

        mainSystem.SetTrackHand(trackHandSetting);
        mainSystem.SetTrackDevice(trackDeviceSetting);
        mainSystem.SetOverlaySize(overlaySizex10Setting, false);
        mainSystem.SetOverlayDistance(overlayDistancex100Setting);
        mainSystem.SetDragThreshold(dragThresholdSetting);
        mainSystem.SetUseTransLiterate(isUseTransLiterateSettig);
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(trackHandKey, (int)mainSystem.GetTrackHand());
        PlayerPrefs.SetInt(trackDeviceKey, (int)mainSystem.GetTrackDevice());
        PlayerPrefs.SetInt(overlaySizeKey, mainSystem.GetOverlaySizex10());
        PlayerPrefs.SetInt(overlayDistanceKey, mainSystem.GetOverlayDistancex100());
        PlayerPrefs.SetInt(dragThresholdKey, mainSystem.GetDragThreshold());
        PlayerPrefs.SetInt(isUseTransLiterateKey, Convert.ToInt32(mainSystem.GetUseTransLiterate()));
    }

    public void ResetSetting()
    {
        PlayerPrefs.DeleteAll();
        mainSystem.SetTrackHand(defaultTrackHand);
        mainSystem.SetTrackDevice(defaultTrackDevice);
        mainSystem.SetOverlaySize(defaultOverlaySizex10, true);
        mainSystem.SetOverlayDistance(defaultOverlayDistancex100);
        mainSystem.SetDragThreshold(defaultDragThreshold);
        mainSystem.SetUseTransLiterate(defaultIsUseTransLiterate);
    }
}
