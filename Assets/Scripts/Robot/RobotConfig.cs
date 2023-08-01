using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Grpc.Core;
using Reachy.Sdk.Joint;
using Reachy;

namespace TeleopReachy
{
    public class RobotConfig : MonoBehaviour
    {
        private gRPCRobotParams gRPCRobotParams;
        private gRPCDataController dataController;
        private gRPCMobileBaseController mobileController;
        // private WebRTCRestartService restartService;
        private ConnectionStatus connectionStatus;

        private JointsId allJointsId;

        private bool has_right_arm;
        private bool has_left_arm;
        private bool has_head;
        private bool has_left_gripper;
        private bool has_right_gripper;
        private bool has_mobile_platform;

        private bool is_virtual;

        public RobotGenerationCode RobotGeneration { get; private set; }

        public bool HasRobotGeneration { get; private set; }
        private bool has_robot_config;

        public UnityEvent event_OnConfigChanged;

        // Awake is called before Start functions
        void Start()
        {
            dataController = gRPCManager.Instance.gRPCDataController;
            mobileController = gRPCManager.Instance.gRPCMobileBaseController;
            connectionStatus = gRPCManager.Instance.ConnectionStatus;

            dataController.event_OnRobotJointsReceived.AddListener(GetJointsId);
            mobileController.event_OnMobileBaseDetected.AddListener(SetMobilePlatform);
            connectionStatus.event_OnConnectionStatusHasChanged.AddListener(CheckConfig);

            gRPCRobotParams = gRPCManager.Instance.gRPCRobotParams;
            gRPCRobotParams.event_OnRobotGenerationReceived.AddListener(UpdateRobotGeneration);

            has_robot_config = false;
            HasRobotGeneration = false;

            has_right_arm = false;
            has_left_arm = false;
            has_head = false;
            has_mobile_platform = false;
            has_left_gripper = false;
            has_right_gripper = false;

            UpdateRobotGeneration();
            is_virtual = Robot.IsCurrentRobotVirtual();
        }

        void ResetConfig()
        {
            Debug.Log("[Robot config]: ResetConfig");
            has_right_arm = false;
            has_left_arm = false;
            has_head = false;
            has_mobile_platform = false;
            has_left_gripper = false;
            has_right_gripper = false;

            has_robot_config = false;

            event_OnConfigChanged.Invoke();
        }

        void CheckConfig()
        {
            Debug.Log("[Robot config]: CheckConfig");
            if (connectionStatus.HasRobotJustLeftDataRoom())
            {
                ResetConfig();
            }
        }

        void UpdateRobotGeneration()
        {
            RobotGeneration = gRPCRobotParams.RobotGeneration;
            if (RobotGeneration != RobotGenerationCode.UNDEFINED) HasRobotGeneration = true;
        }

        void SetMobilePlatform()
        {
            Debug.Log("[Robot config]: SetMobilePlatform");
            has_mobile_platform = true;
            event_OnConfigChanged.Invoke();
        }


        void GetJointsId(JointsId AllJointsIds)
        {
            allJointsId = AllJointsIds;
            GetReachyConfig();
        }

        private void GetReachyConfig()
        {
            Debug.Log("[Robot config]: GetReachyConfig");
            if (allJointsId.Names.Contains("r_shoulder_pitch"))
            {
                has_right_arm = true;
            }
            if (allJointsId.Names.Contains("l_shoulder_pitch"))
            {
                has_left_arm = true;
            }
            if (allJointsId.Names.Contains("neck_pitch"))
            {
                has_head = true;
            }
            if (allJointsId.Names.Contains("l_gripper_finger"))
            {
                has_left_gripper = true;
            }
            if (allJointsId.Names.Contains("r_gripper_finger"))
            {
                has_right_gripper = true;
            }
            has_robot_config = true;

            event_OnConfigChanged.Invoke();
        }

        public bool HasRightArm()
        {
            return has_right_arm;
        }
        public bool HasLeftArm()
        {
            return has_left_arm;
        }

        public bool HasHead()
        {
            return has_head;
        }

        public bool HasLeftGripper()
        {
            return has_left_gripper;
        }

        public bool HasRightGripper()
        {
            return has_right_gripper;
        }

        public bool HasMobilePlatform()
        {
            return has_mobile_platform;
        }

        public void SetMobilePlatform(bool mobile_platform_detected)
        {
            Debug.Log("[Robot config]: SetMobilePlatform");
            has_mobile_platform = mobile_platform_detected;
            event_OnConfigChanged.Invoke();
        }

        public JointsId GetAllJointsId()
        {
            return allJointsId;
        }

        public bool GotReachyConfig()
        {
            return has_robot_config;
        }

        public bool IsVirtual()
        {
            return is_virtual;
        }

        public override string ToString()
        {
            return string.Format(@"has_right_arm = {0},
            has_left_arm= {1},
            has_head= {2},
            has_mobile_platform= {3},
            has_left_gripper= {4},
            has_right_gripper= {5},
            has_robot_config= {6}",
            has_right_arm, has_left_arm, has_head, has_mobile_platform, has_left_gripper,
            has_right_gripper, has_robot_config);
        }

    }
}
