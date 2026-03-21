using System;
using UnityEngine;
using Valve.VR;
using SystemUtil;
using System.IO;

public class SettingSystem : MonoBehaviour
{
    const string APP_KEY = "kars.overlay.vrcji";
    [SerializeField] MainSystem mainSystem;
    const string trackHandKey = "TrackHand";
    private ETrackedControllerRole defaultTrackHand = ETrackedControllerRole.RightHand;
    const string overlaySizeKey = "OverlaySizex100";
    private int defaultOverlaySizex100 = 30;
    const string overlayDistanceKey = "OverlayDistancex100";
    private int defaultOverlayDistancex100 = 10;
    const string dragThresholdKey = "DragThreshold";
    private int defaultDragThreshold = 40;
    const string isUseJoystickKey = "UseJoystick";
    private bool defaultIsUseJoystick = true;
    private string isAutoLaunchKey = "AutoLaunch";
    private bool defaultIsAutoLaunch = false;
    const string isInitializedKey = "Initialized_v3_0_0";

    static string[] oldKeys = {
        "OverlaySizex10",
        "UseTransLiterate",
        "TrackDevice"
    };

    void Awake()
    {
        SetLoadSettings();
    }

    private void SetLoadSettings()
    {
        ETrackedControllerRole trackHandSetting = (ETrackedControllerRole)PlayerPrefs.GetInt(trackHandKey, (int)defaultTrackHand);
        int overlaySizex10Setting = PlayerPrefs.GetInt(overlaySizeKey, defaultOverlaySizex100);
        int overlayDistancex100Setting = PlayerPrefs.GetInt(overlayDistanceKey, defaultOverlayDistancex100);
        int dragThresholdSetting = PlayerPrefs.GetInt(dragThresholdKey, defaultDragThreshold);
        bool isUseJoystickSetting = Convert.ToBoolean(PlayerPrefs.GetInt(isUseJoystickKey, Convert.ToInt32(defaultIsUseJoystick)));
        // bool isAutoLaunchSetting = Convert.ToBoolean(PlayerPrefs.GetInt(isAutoLaunchKey, Convert.ToInt32(defaultIsAutoLaunch)));
        var isAutoLaunchSetting = OpenVR.Applications.GetApplicationAutoLaunch(APP_KEY);


        mainSystem.SetTrackHand(trackHandSetting);
        mainSystem.SetOverlaySize(overlaySizex10Setting, false);
        mainSystem.SetOverlayDistance(overlayDistancex100Setting);
        mainSystem.SetDragThreshold(dragThresholdSetting);
        mainSystem.SetUseJoystick(isUseJoystickSetting);
        mainSystem.SetAutoLaunch(isAutoLaunchSetting);
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(trackHandKey, (int)mainSystem.GetTrackHand());
        PlayerPrefs.SetInt(overlaySizeKey, mainSystem.GetOverlaySizex100());
        PlayerPrefs.SetInt(overlayDistanceKey, mainSystem.GetOverlayDistancex100());
        PlayerPrefs.SetInt(dragThresholdKey, mainSystem.GetDragThreshold());
        PlayerPrefs.SetInt(isUseJoystickKey, Convert.ToInt32(mainSystem.GetUseJoystick()));
        PlayerPrefs.SetInt(isAutoLaunchKey, Convert.ToInt32(mainSystem.GetAutoLaunch()));
    }

    public void ResetSetting()
    {
        PlayerPrefs.DeleteAll();
        mainSystem.SetTrackHand(defaultTrackHand);
        mainSystem.SetOverlaySize(defaultOverlaySizex100, true);
        mainSystem.SetOverlayDistance(defaultOverlayDistancex100);
        mainSystem.SetDragThreshold(defaultDragThreshold);
        mainSystem.SetUseJoystick(defaultIsUseJoystick);
        mainSystem.SetAutoLaunch(defaultIsAutoLaunch);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if(PlayerPrefs.GetInt(isInitializedKey) == 1) return;

        foreach (var key in oldKeys)
        {
            if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
        }

        // if(!OpenVR.Applications.IsApplicationInstalled(APP_KEY))
        //     OpenVR.Applications.AddApplicationManifest(Path.Combine(Application.streamingAssetsPath, @"./manifest.vrmanifest"), false);

        PlayerPrefs.SetInt(isInitializedKey, 1);
    }
}
