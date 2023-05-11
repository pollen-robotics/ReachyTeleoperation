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
        private Transform reachyEyeView;

        private EyeScript eyeScript;

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

            eyeScript = reachyEyeView.GetComponent<EyeScript>();

            reachyEyeView.gameObject.SetActive(false);
        }

        void ShowReachyView()
        {
            reachyEyeView.gameObject.SetActive(true);
        }

        void HideReachyView()
        {
            UnityEngine.Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;
            reachyEyeView.gameObject.SetActive(false);
        }

        void SwitchTo2DMode()
        {
            eyeScript.switchVisionMode(true);
        }

        void SwitchTo3DMode()
        {
            eyeScript.switchVisionMode(false);
        }

        void ModifyTextureTransparency(bool isRobotInVideoRoom)
        {
            if(isRobotInVideoRoom)
            {
                eyeScript.SetImageOpaque();
            }
            else
            {
                if(robotStatus.IsRobotTeleoperationActive())
                {
                    eyeScript.SetImageTransparent();
                }
            }
        }
    }
}