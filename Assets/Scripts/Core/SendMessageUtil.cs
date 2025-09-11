#nullable enable
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SendMessageUtil
{
    public static class SendMessageToVRC
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        public static extern IntPtr PostMessageW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_CHAR = 0x0102;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint VK_RETURN = 0x0D;

        public static void SendMessageToVRCWithWinAPI(string windowName, string strMessage)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
            {
                Debug.Log("VRChatのウィンドウを取得できませんでした。");
                return;
            }
            foreach(char c in strMessage)
            {
                PostMessageW(hWnd, WM_CHAR, (IntPtr)c, IntPtr.Zero);
                // await Task.Delay(10);
            }
        }
    }
}
