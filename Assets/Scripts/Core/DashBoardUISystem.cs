using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using SystemUtil;
using TMPro;

public class DashBoardUISystem : MonoBehaviour
{
    [SerializeField] List<GameObject> trackHandOptions;
    [SerializeField] GameObject trackHandActiveGround;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] TextMeshProUGUI dragThresholdText;
    [SerializeField] TextMeshProUGUI sizeText;
    [SerializeField] GameObject joystickActiveGround;
    [SerializeField] GameObject autoLaunchActiveGround;
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


    public void ChangeActiveAutoLaunch(bool isAutoLaunch)
    {
        autoLaunchActiveGround.SetActive(isAutoLaunch);
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
