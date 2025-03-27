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

    public void SendMessage()
    {
        if(isFinishSend)
        {
            isFinishSend = false;
            if(mainSystem.GetSendTarget() == MainSystemUtil.SendTarget.Chat)
                client.Send("/chatbox/input", textManegamentSystem.GetCurrentText(), true);
            else
                SendMessageToVRC.SendMessageToVRCWithWinAPI(windowName, textManegamentSystem.GetCurrentText());
            textManegamentSystem.InitText();
            isFinishSend = true;
        }
    }

    public void SendChatTyping(bool isTyping)
    {
        client.Send("/chatbox/typing", isTyping && mainSystem.GetSendTarget() == MainSystemUtil.SendTarget.Chat);
    }
}
