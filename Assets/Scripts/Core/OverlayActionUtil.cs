using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OverlayVRUtil;

namespace OverlayActionUtil
{
    public static class ActionUtil
    {
        public static void SetActionManifest(string path)
        {
            var err = OpenVR.Input.SetActionManifestPath(path);
            EVRInputErrThrowException(err, "Action Manifestパスの指定に失敗しました");
        }
        public static void EVRInputErrThrowException(EVRInputError error, string message)
        {
            if(error != EVRInputError.None)
            {
                throw new Exception($"{message} : + {error}");
            }
        }
        public static ulong GetActionSetHandlePath(string path)
        {
            ulong actionSetHandle = 0;
            var err = OpenVR.Input.GetActionSetHandle(path, ref actionSetHandle);
            EVRInputErrThrowException(err, "アクションセットの取得に失敗しました");
            return actionSetHandle;
        }
        public static ulong GetActionHandlePath(string path)
        {
            ulong actionHandle = 0;
            var err = OpenVR.Input.GetActionHandle(path, ref actionHandle);
            EVRInputErrThrowException(err, "アクションの取得に失敗しました");
            return actionHandle;
        }
        public static InputDigitalActionData_t GetDigitalActionData(ulong actionHandle)
        {
            var result = new InputDigitalActionData_t();
            var digitalActionSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(InputDigitalActionData_t));
            var err = OpenVR.Input.GetDigitalActionData(actionHandle, ref result, digitalActionSize, OpenVR.k_ulInvalidInputValueHandle);
            EVRInputErrThrowException(err, "アクションデータの取得に失敗しました");
            return result;
        }
        public static InputAnalogActionData_t GetAnalogActionData(ulong actionHandle)
        {
            var result = new InputAnalogActionData_t();
            var analogActionSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(InputAnalogActionData_t));
            var err = OpenVR.Input.GetAnalogActionData(actionHandle, ref result, analogActionSize, OpenVR.k_ulInvalidInputValueHandle);
            EVRInputErrThrowException(err, "アクションデータの取得に失敗しました");
            return result;
        }
    }
}
