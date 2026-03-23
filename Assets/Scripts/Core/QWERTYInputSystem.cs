using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;

public class QWERTYInputSystem : MonoBehaviour
{
    [SerializeField]
    private TextManagementSystem textManagementSystem;
    private string tempStr = "";

    public void OnClickQwertyInputButton(string s)
    {
        tempStr += s;
    }
}