using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using SystemUtil;
using TMPro;

public class DashBoardUISystem : MonoBehaviour
{
    [SerializeField] List<GameObject> trackHandOptions;
    [SerializeField] GameObject trackHandActiveGround;
    [SerializeField] List<GameObject> trackDeviceOptions;
    [SerializeField] GameObject trackDeviceActiveGround;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] TextMeshProUGUI dragThresholdText;
    [SerializeField] TextMeshProUGUI sizeText;
    [SerializeField] GameObject joystickActiveGround;
    [SerializeField] GameObject transLiterateActiveGround;
    [SerializeField] GameObject aboutObject;
    private bool isAboutActive = false;

    public void ChangeTrackHandUI(ETrackedControllerRole trackHand)
    {
        if(trackHand == ETrackedControllerRole.LeftHand)
        {
            trackHandActiveGround.transform.localPosition = trackHandOptions[0].transform.localPosition;
        }
        else if(trackHand == ETrackedControllerRole.RightHand)
        {
            trackHandActiveGround.transform.localPosition = trackHandOptions[1].transform.localPosition;
        }
    }

    public void ChangeTrackDeviceUI(MainSystemUtil.TrackDevice trackDevice)
    {
        if(trackDevice == MainSystemUtil.TrackDevice.WORLD)
        {
            trackDeviceActiveGround.transform.localPosition = trackDeviceOptions[0].transform.localPosition;
        }
        if(trackDevice == MainSystemUtil.TrackDevice.HMD)
        {
            trackDeviceActiveGround.transform.localPosition = trackDeviceOptions[1].transform.localPosition;
        }
    }

    public void ChangeDistanceUI(int distanceValue)
    {
        distanceText.text = distanceValue.ToString();
    }

    public void ChangeDragThresholdUI(int thresholdValue)
    {
        dragThresholdText.text = thresholdValue.ToString();
    }

    public void ChangeSizeUI(int sizeValue)
    {
        sizeText.text = sizeValue.ToString();
    }

    public void ChangeActiveJoystick(bool isUseJoystick)
    {
        joystickActiveGround.SetActive(isUseJoystick);
    }


    public void ChangeActiveTransLiterate(bool isUseTransLiterate)
    {
        transLiterateActiveGround.SetActive(isUseTransLiterate);
    }

    public void ShowAbout()
    {
        if(!isAboutActive)
        {
            isAboutActive = true;
            aboutObject.SetActive(true);
        }
    }

    public void HideAbout()
    {
        if(isAboutActive)
        {
            aboutObject.SetActive(false);
            isAboutActive = false;
        }
    }

    public bool GetAboutVisible()
    {
        return isAboutActive;
    }
}
