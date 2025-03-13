using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using SystemUtil;
using SendMessageUtil;
using uOSC;

public class SendMessageSystem : MonoBehaviour
{
    [SerializeField] TextManagementSystem textManegamentSystem;
    [SerializeField] MainSystem mainSystem;
    [SerializeField] uOscClient client;
    private bool isFinishSend = true;
    string windowName = "VRChat";

    // void Start()
    // {
    //     SendMessageToVRC.ChangeWindowName(windowName);
    // }

    public void SendMessage()
    {
        if(isFinishSend)
        {
            isFinishSend = false;
            Task _;
            if(mainSystem.GetSendTarget() == MainSystemUtil.SendTarget.Chat)
                client.Send("/chatbox/input", textManegamentSystem.GetCurrentText(), true);
            else
                _ = SendMessageToVRC.SendMessageToVRCWithWinAPI(windowName, textManegamentSystem.GetCurrentText());
            textManegamentSystem.InitText();
            isFinishSend = true;
        }
    }
}
