using System;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using TMPro;

namespace OverlayVRUtil
{
    public static class OverlaySystemUtil
    {
        public static void InitOpenVR()
        {
            if(OpenVR.System != null) return;

            var err = EVRInitError.None;
            OpenVR.Init(ref err, EVRApplicationType.VRApplication_Overlay);
            if (err != EVRInitError.None)
            {
                throw new Exception("OpenVRの初期化に失敗しました: " + err);
            }
        }
        public static void ShutdownOpenVR()
        {
            if (OpenVR.System != null)
            {
                OpenVR.Shutdown();
            }
        }
    }
    public static class OverlayUtil
    {
        public static ulong CreateOverlay(string key, string name)
        {
            var handle = OpenVR.k_ulOverlayHandleInvalid;
            var err = OpenVR.Overlay.CreateOverlay(key, name, ref handle);
            EVRErrThrowException(err, "オーバーレイの作成に失敗しました");
            return handle;
        }
        public static void DestroyOverlay(ulong handle)
        {
            if(handle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var err = OpenVR.Overlay.DestroyOverlay(handle);
                EVRErrThrowException(err, "オーバーレイの破棄に失敗しました");
            }
        }
        public static void FlipOverlayVertical(ulong handle)
        {
            var bounds = new VRTextureBounds_t
            {
                uMin = 0,
                uMax = 1,
                vMin = 1,
                vMax = 0
            };
            var err = OpenVR.Overlay.SetOverlayTextureBounds(handle, ref bounds);
            EVRErrThrowException(err, "テクスチャの設定に失敗しました");
        }
        public static void SetOverlaySize(ulong handle, float size)
        {
            var err = OpenVR.Overlay.SetOverlayWidthInMeters(handle, size);
            EVRErrThrowException(err, "オーバーレイのサイズ設定に失敗しました");
        }
        public static void HideOverlay(ulong handle)
        {
            var err = OpenVR.Overlay.HideOverlay(handle);
            EVRErrThrowException(err, "オーバーレイの表示設定に失敗しました");
        }
        public static void ShowOverlay(ulong handle)
        {
            var err = OpenVR.Overlay.ShowOverlay(handle);
            EVRErrThrowException(err, "オーバーレイの表示設定に失敗しました");
        }
        public static void SetOverlayRenderTexture(ulong handle, RenderTexture rendertexture)
        {
            if(!rendertexture.IsCreated()) return;
            var nativeTexturePtr = rendertexture.GetNativeTexturePtr();
            var texture = new Texture_t
            {
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.DirectX,
                handle = nativeTexturePtr
            };
            var err = OpenVR.Overlay.SetOverlayTexture(handle, ref texture);
            EVRErrThrowException(err, "テクスチャの描画に失敗しました");
        }
        public static SteamVR_Utils.RigidTransform GetControllerTransform(ETrackedControllerRole trackHand)
        {
            //default Transform
            var pos = new Vector3(0f, 0f, 0f);
            var rot = Quaternion.Euler(0,0,0);
            var defaultTransform = new SteamVR_Utils.RigidTransform(pos, rot);

            var controllerIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole(trackHand);
            if(controllerIndex == OpenVR.k_unTrackedDeviceIndexInvalid) return defaultTransform;

            var poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poses);

            if(!poses[controllerIndex].bPoseIsValid) return defaultTransform;
            else return new SteamVR_Utils.RigidTransform(poses[controllerIndex].mDeviceToAbsoluteTracking);
        }
        public static SteamVR_Utils.RigidTransform GetHmdTransform()
        {
            //default Transform
            var pos = new Vector3(0f, 0f, 0f);
            var rot = Quaternion.Euler(0,0,0);
            var defaultTransform = new SteamVR_Utils.RigidTransform(pos, rot);

            var poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poses);

            if(!poses[OpenVR.k_unTrackedDeviceIndex_Hmd].bPoseIsValid) return defaultTransform;
            else return new SteamVR_Utils.RigidTransform(poses[OpenVR.k_unTrackedDeviceIndex_Hmd].mDeviceToAbsoluteTracking);
        }
        public static Vector2? GetOverlayIntersectionForController(
            ulong overlayHandle, ETrackedControllerRole trackHand, int angle = 45) //コントローラの指し示す方向を調整するため45°バイアスとして設定
        {
            var hitPoint = new Vector2(0f, 0f);
            var controllerTransform = OverlayUtil.GetControllerTransform(trackHand);
            var direction = (controllerTransform.rot * Quaternion.AngleAxis(angle, Vector3.right)) * Vector3.forward;
            var overlayParams = new VROverlayIntersectionParams_t
            {
                vSource = new HmdVector3_t
                {
                    v0 = controllerTransform.pos.x,
                    v1 = controllerTransform.pos.y ,
                    v2 = -controllerTransform.pos.z
                },
                vDirection = new HmdVector3_t
                {
                    v0 = direction.x,
                    v1 = direction.y,
                    v2 = -direction.z
                },
                eOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding
            };
            VROverlayIntersectionResults_t overlayResults = default;

            var hit = OpenVR.Overlay.ComputeOverlayIntersection(overlayHandle, ref overlayParams, ref overlayResults);
            if(hit)
            {
                hitPoint.x = (overlayResults.vUVs.v0-0.5f);
                hitPoint.y = (0.5f-overlayResults.vUVs.v1);
                return hitPoint;
            }
            else return null;
        }
        public static Button GetButtonByPosition(EventSystem eventSystem, GraphicRaycaster graphicRaycaster, Vector2 cursorPosition)
        {
            var pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = cursorPosition;

            var raycastResultList = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, raycastResultList);
            var raycastResult = raycastResultList.Find(element => element.gameObject.GetComponent<Button>());
            if(raycastResult.gameObject == null) return null;
            else return raycastResult.gameObject.GetComponent<Button>();
        }
        public static GameObject GetButtonObjectByPosition(EventSystem eventSystem, GraphicRaycaster graphicRaycaster, Vector2 cursorPosition)
        {
            var pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = cursorPosition;

            var raycastResultList = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, raycastResultList);
            var raycastResult = raycastResultList.Find(element => element.gameObject.GetComponent<Button>());
            if(raycastResult.gameObject == null) return null;
            else return raycastResult.gameObject;
        }
        public static void SetTransformAbsolute(ulong handle, Vector3 pos, Quaternion rot)
        {
            var rigidTransform = new SteamVR_Utils.RigidTransform(pos, rot);
            var matrix = rigidTransform.ToHmdMatrix34();
            var err = OpenVR.Overlay.SetOverlayTransformAbsolute(
                handle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref matrix);
            EVRErrThrowException(err, "オーバーレイの位置設定に失敗しました");
        }
        public static void EVRErrThrowException(EVROverlayError error, string message)
        {
            if(error != EVROverlayError.None)
            {
                throw new Exception($"{message} : {error}");
            }
        }
    }

    public static class DashBoardUtil
    {
        public static (ulong, ulong) CreateDashBoardOverlay(string key, string name)
        {
            ulong dashboardHandle = OpenVR.k_ulOverlayHandleInvalid;
            ulong thumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
            var err = OpenVR.Overlay.CreateDashboardOverlay(key, name, ref dashboardHandle, ref thumbnailHandle);
            OverlayUtil.EVRErrThrowException(err, "ダッシュボードの作成に失敗しました");
            return (dashboardHandle, thumbnailHandle);
        }
        public static void SetDashBoardIcon(ulong overlayHandle, string filePath)
        {
            var err = OpenVR.Overlay.SetOverlayFromFile(overlayHandle, filePath);
            OverlayUtil.EVRErrThrowException(err, "ダッシュボードアイコンの設定に失敗しました");
        }
        public static void SetDashBoardMouseScale(ulong dashboardHandle, RenderTexture renderTexture)
        {
            var mouseScalingFactor = new HmdVector2_t()
            {
                v0 = renderTexture.width,
                v1 = renderTexture.height
            };
            var err = OpenVR.Overlay.SetOverlayMouseScale(dashboardHandle, ref mouseScalingFactor);
            OverlayUtil.EVRErrThrowException(err, "Mouse Scaling Factor の設定に失敗しました");
        }
    }
}
