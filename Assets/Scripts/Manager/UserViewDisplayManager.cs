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

        private LeftEyeScript leftEyeScript;

        private RobotStatus robotStatus;
        private gRPCVideoController videoController;

        void Start()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStartTeleoperation.AddListener(ShowReachyView);
            robotStatus.event_OnStopTeleoperation.AddListener(HideReachyView);

            videoController = gRPCManager.Instance.gRPCVideoController;
            videoController.event_OnVideoRoomStatusHasChanged.AddListener(ModifyTextureTransparency);

            leftEyeScript = reachyLeftEyeView.GetComponent<LeftEyeScript>();

            reachyLeftEyeView.gameObject.SetActive(false);
        }

        void ShowReachyView()
        {
            reachyLeftEyeView.gameObject.SetActive(true);
        }

        void HideReachyView()
        {
            UnityEngine.Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;
            reachyLeftEyeView.gameObject.SetActive(false);
        }

        void ModifyTextureTransparency(bool isRobotInVideoRoom)
        {
            if(isRobotInVideoRoom)
            {
                leftEyeScript.SetImageOpaque();
            }
            else
            {
                if(robotStatus.IsRobotTeleoperationActive())
                {
                    leftEyeScript.SetImageTransparent();
                }
            }
        }
    }
}