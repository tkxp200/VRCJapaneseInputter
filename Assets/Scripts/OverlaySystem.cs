using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using OverlayVRUtil;
using SystemUtil;
using uOSC;

public class OverlaySystem : MonoBehaviour
{
    public RenderTexture inputTexture;
    [SerializeField] private MainSystem mainSystem;
    private bool isOverlayVisible = false;
    private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;

    public void ReloadOverlay()
    {
        OverlayUtil.SetOverlayRenderTexture(overlayHandle, inputTexture);
    }

    void Awake()
    {
        OverlaySystemUtil.InitOpenVR();
        overlayHandle = OverlayUtil.CreateOverlay("VRCJapaneseInputter_key", "VRCJapaneseInputter");
    }

    void Start()
    {
        OverlayUtil.FlipOverlayVertical(overlayHandle);
        OverlayUtil.SetOverlaySize(overlayHandle, mainSystem.GetOverlaySize());
        OverlayUtil.SetOverlayRenderTexture(overlayHandle, inputTexture);
        SetOverlayRenderTexture();
        HideOverlay();
    }

    void Update()
    {
        if(!isOverlayVisible) SetOverlayRenderTexture();
    }

    private void SetOverlayRenderTexture()
    {
        var controllerTransform = OverlayUtil.GetControllerTransform(mainSystem.GetTrackHand());
        var hmdTransform = OverlayUtil.GetHmdTransform();
        var diff = new Vector3(0f, 0f, mainSystem.GetOverlayDistance());
        Quaternion rotation;
        if(mainSystem.GetTrackDevice() == MainSystemUtil.TrackDevice.HMD) rotation = Quaternion.Euler(hmdTransform.rot.eulerAngles.x, hmdTransform.rot.eulerAngles.y, 0f);
        else rotation = controllerTransform.rot * Quaternion.Euler(90f, 0f, 0f);
        OverlayUtil.SetTransformAbsolute(overlayHandle, controllerTransform.pos + diff, rotation);
        ReloadOverlay();
    }

    public void ChangeOverlaySize()
    {
        OverlayUtil.SetOverlaySize(overlayHandle, mainSystem.GetOverlaySize());
        ReloadOverlay();
    }

    public void ShowOverlay()
    {
        if(!isOverlayVisible)
        {
            OverlayUtil.ShowOverlay(overlayHandle);
            isOverlayVisible = true;
        }
        else SetOverlayRenderTexture();
    }

    public void HideOverlay()
    {
        if(isOverlayVisible)
        {
            OverlayUtil.HideOverlay(overlayHandle);
            isOverlayVisible = false;
        }
    }

    public void OnOSCMessageReceived(Message message)
    {
        if(message.address == "/avatar/parameters/ShowVRCJPInputter" && (bool)message.values[0])
        {
            // if(!isOverlayVisible) ShowOverlay();
            // else SetOverlayRenderTexture();
            ShowOverlay();
        }
    }

    public bool GetOverlayVisible()
    {
        return isOverlayVisible;
    }

    public ulong GetOverlayHandle()
    {
        return overlayHandle;
    }

    private void OnApplicationQuit()
    {
        OverlayUtil.DestroyOverlay(overlayHandle);
    }

    private void Destroy()
    {
        OverlaySystemUtil.ShutdownOpenVR();
    }
}
