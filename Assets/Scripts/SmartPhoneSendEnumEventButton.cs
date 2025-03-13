using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using OverlayInputUtil;

public class SmartPhoneSendEnumEventButton : MonoBehaviour
{
    [SerializeField] SmartPhoneInputUtil.SmartPhoneKeyType keyType;
    [SerializeField] private UnityEvent<SmartPhoneInputUtil.SmartPhoneKeyType, Vector3> clickEvent;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SendEnum);
    }

    public void SendEnum()
    {
        clickEvent.Invoke(keyType, this.transform.localPosition);
    }
}
