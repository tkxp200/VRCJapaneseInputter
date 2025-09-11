using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using OverlayVRUtil;

public class CursorSystem : MonoBehaviour
{
    public GameObject cursorObject;
    [SerializeField] private OverlaySystem overlaySystem;
    [SerializeField] private ActionSystem actionSystem;
    private ulong overlayHandle;

    void Start()
    {
        actionSystem.OnCursorPositionMove += ChangeCursorPosition;
    }

    void ChangeCursorPosition(Vector2? cursorPosition)
    {
        if(cursorPosition != null)
        {
            SetCursorVisible(true);
            cursorObject.transform.localPosition = (Vector2)cursorPosition;
        }
        else SetCursorVisible(false);
        overlaySystem.ReloadOverlay();
    }

    public void SetCursorVisible(bool setVisible)
    {
        cursorObject.SetActive(setVisible);
    }
}
