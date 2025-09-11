using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR;

public class DashBoardSendTrackHand : MonoBehaviour
{
    [SerializeField] ETrackedControllerRole targetHand;
    [SerializeField] private UnityEvent<ETrackedControllerRole> clickEvent;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SendHand);
    }

    public void SendHand()
    {
        clickEvent.Invoke(targetHand);
    }
}
