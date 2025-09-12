using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SystemUtil;

public class DashBoardSendTrackDevice : MonoBehaviour
{
    [SerializeField] MainSystemUtil.TrackDevice targetDevice;
    [SerializeField] private UnityEvent<MainSystemUtil.TrackDevice> clickEvent;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SendDevice);
    }

    public void SendDevice()
    {
        clickEvent.Invoke(targetDevice);
    }
}
