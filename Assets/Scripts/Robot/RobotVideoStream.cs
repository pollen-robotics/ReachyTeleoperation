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
    public class RobotVideoStream : MonoBehaviour
    {
        private gRPCVideoController videoController;
        private RobotStatus robotStatus;

        void Start()
        {
            videoController = gRPCManager.Instance.gRPCVideoController;
            // videoController.OnVideoStreamReady += ResetOverlayTexture;
            // videoController.OnVideoRoomStatusHasChanged += CheckCurrentStatus;

            robotStatus = RobotDataManager.Instance.RobotStatus;
        }

        void Update()
        {
            if(robotStatus.Is2DVisionModeOn()) videoController.GetImage(CameraId.Left);
            else videoController.GetBothImages();
        }

    }
}