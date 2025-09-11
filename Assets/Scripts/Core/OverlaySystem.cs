using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using OverlayVRUtil;
using SystemUtil;
using uOSC;

public class OverlaySystem : MonoBehaviour
{
    private const int controllerRotationBias = 50;
    public RenderTexture inputTexture;
    [SerializeField] private MainSystem mainSystem;
    private bool isOverlayVisible = false;
    private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
    private bool isGrub = false;
    private Vector3 grubBias;

    public void ReloadOverlay()
    {
        OverlayUtil.SetOverlayRenderTexture(overlayHandle, inputTexture);
    }

    void Awake()
    {
        // OverlaySystemUtil.InitOpenVR();
        overlayHandle = OverlayUtil.CreateOverlay("VRCJapaneseInputter_key", "VRCJapaneseInputter");
    }

    void Start()
    {
        OverlayUtil.FlipOverlayVertical(overlayHandle);
        OverlayUtil.SetOverlaySize(overlayHandle, mainSystem.GetOverlaySize());
        OverlayUtil.SetOverlayRenderTexture(overlayHandle, inputTexture);
        SetOverlayRenderTexture(true);
        HideOverlay();
    }

    void Update()
    {
        if(!isOverlayVisible) SetOverlayRenderTexture(true);
    }

    private void SetOverlayRenderTexture(bool useDistance)
    {
        var controllerTransform = OverlayUtil.GetControllerTransform(mainSystem.GetTrackHand());
        var hmdTransform = OverlayUtil.GetHmdTransform();
        Vector3 forward = controllerTransform.rot * Quaternion.AngleAxis(controllerRotationBias, Vector3.right) * Vector3.forward;
        Vector3 overlayPosition;
        if (useDistance) overlayPosition = controllerTransform.pos + forward * mainSystem.GetOverlayDistance();
        else overlayPosition = controllerTransform.pos - grubBias;
        Quaternion rotation;
        Vector3 direction = - controllerTransform.pos + overlayPosition;
        direction.Normalize();
        if (mainSystem.GetTrackDevice() == MainSystemUtil.TrackDevice.HMD)
            rotation = Quaternion.LookRotation(direction, hmdTransform.rot * Vector3.up);
        else rotation = Quaternion.LookRotation(forward, Vector3.up);
        OverlayUtil.SetTransformAbsolute(overlayHandle, overlayPosition, rotation);
        ReloadOverlay();
    }

    public void GrubEvent(bool state, bool changed)
    {
        var controllerTransform = OverlayUtil.GetControllerTransform(mainSystem.GetTrackHand());
        var overlayTransform = OverlayUtil.GetTransformAbsolute(overlayHandle);
        var diff = controllerTransform.pos - overlayTransform.pos;
        if (changed && Vector3.Magnitude(diff) < 0.3f)
        {
            isGrub = state;
            grubBias = diff;
        }
        if(isGrub) SetOverlayRenderTexture(false);
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
        else SetOverlayRenderTexture(true);
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
        // OverlaySystemUtil.ShutdownOpenVR();
    }
}
