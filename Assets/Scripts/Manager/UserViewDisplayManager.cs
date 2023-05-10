using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Grpc.Core;
using Reachy.Sdk.Camera;

namespace TeleopReachy
{
    public class UserViewDisplayManager : MonoBehaviour
    {
        [SerializeField]
        private Transform reachyLeftEyeView;
        [SerializeField]
        private Transform reachyRightEyeView;

        private LeftEyeScript leftEyeScript;
        private RightEyeScript rightEyeScript;

        [SerializeField]
        private GameObject prefabRightEyeCamera;
        // [SerializeField]
        // private GameObject prefabOfflineMenuRightEyeDisplayer;
        // [SerializeField]
        // private GameObject prefabTeleoperationRightView;

        private GameObject rightCamera;
        // private GameObject teleoperationRightView;
        // private GameObject offlineMenuRightEyeDisplayer;

        private RobotStatus robotStatus;
        private gRPCVideoController videoController;

        void Start()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStartTeleoperation.AddListener(ShowReachyView);
            robotStatus.event_OnStopTeleoperation.AddListener(HideReachyView);

            robotStatus.event_OnSwitchTo2DVision.AddListener(SwitchTo2DMode);
            robotStatus.event_OnSwitchTo3DVision.AddListener(SwitchTo3DMode);

            videoController = gRPCManager.Instance.gRPCVideoController;
            videoController.event_OnVideoRoomStatusHasChanged.AddListener(ModifyTextureTransparency);

            EventManager.StartListening(EventNames.LoadConnectionScene, SwitchTo2DMode);

            leftEyeScript = reachyLeftEyeView.GetComponent<LeftEyeScript>();

            reachyLeftEyeView.gameObject.SetActive(false);
        }

        void ShowReachyView()
        {
            if(robotStatus.Is2DVisionModeOn())
            {
                reachyLeftEyeView.gameObject.SetActive(true);
            }
            else
            {
                // UnityEngine.Camera.main.stereoTargetEye = StereoTargetEyeMask.Left;
                reachyLeftEyeView.gameObject.SetActive(true);
                reachyRightEyeView.gameObject.SetActive(true);
            }
        }

        void HideReachyView()
        {
            UnityEngine.Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;
            reachyLeftEyeView.gameObject.SetActive(false);
            if(!robotStatus.Is2DVisionModeOn())
            {
                reachyRightEyeView.gameObject.SetActive(false);
            }
        }

        void SwitchTo3DMode()
        {
            rightCamera = (GameObject)Instantiate(prefabRightEyeCamera);
            // reachyRightEyeView.GetComponent<Canvas>().worldCamera = rightCamera.GetComponent<UnityEngine.Camera>();
            // teleoperationRightView = (GameObject)Instantiate(prefabTeleoperationRightView);
            // teleoperationRightView.GetComponent<Canvas>().worldCamera = rightCamera.GetComponent<UnityEngine.Camera>();
            // offlineMenuRightEyeDisplayer = (GameObject)Instantiate(prefabOfflineMenuRightEyeDisplayer);
            // offlineMenuRightEyeDisplayer.GetComponent<Canvas>().worldCamera = rightCamera.GetComponent<UnityEngine.Camera>();
            // offlineMenuRightEyeDisplayer.GetComponent<Canvas>().planeDistance = 1.5f;
            // reachyRightEyeView = teleoperationRightView.transform.GetChild(0);
            // rightEyeScript = reachyRightEyeView.GetComponent<RightEyeScript>();
            // reachyRightEyeView.gameObject.SetActive(false);
        }

        void SwitchTo2DMode()
        {

            Debug.Log("SwitchTo2DMode");
            // if (teleoperationRightView != null)
            // {
            //     Destroy(teleoperationRightView);
            // }
            // if (offlineMenuRightEyeDisplayer != null)
            // {
            //     Destroy(offlineMenuRightEyeDisplayer);
            // }
            
            if (rightCamera != null)
            {
                Destroy(rightCamera);
            }
        }

        void ModifyTextureTransparency(bool isRobotInVideoRoom)
        {
            if(isRobotInVideoRoom)
            {
                leftEyeScript.SetImageOpaque();
                if(rightEyeScript != null) rightEyeScript.SetImageOpaque();
            }
            else
            {
                if(robotStatus.IsRobotTeleoperationActive())
                {
                    leftEyeScript.SetImageTransparent();
                    if(rightEyeScript != null) rightEyeScript.SetImageTransparent();
                }
            }
        }
    }
}