using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeleopReachy
{
    public class TeleoperationManager : MonoBehaviour
    {
        private TransitionRoomManager transitionRoomManager;

        private RobotStatus robotStatus;
        private ConnectionStatus connectionStatus;

        [SerializeField]
        private Transform reachy;

        private RobotConfig robotConfig;

        void Start()
        {
            EventManager.StartListening(EventNames.QuitMirrorScene, StartTeleoperation);
            EventManager.StartListening(EventNames.BackToMirrorScene, StopTeleoperation);
            EventManager.StartListening(EventNames.HeadsetRemoved, SuspendTeleoperation);
            robotConfig = RobotDataManager.Instance.RobotConfig;

            robotStatus = RobotDataManager.Instance.RobotStatus;
            connectionStatus = gRPCManager.Instance.ConnectionStatus;
            connectionStatus.event_OnConnectionStatusHasChanged.AddListener(CheckChanges);
        }

        void StartTeleoperation()
        {
            Debug.Log("[TeleoperationManager]: StartTeleoperation");

            DisplayReachy(false);

            robotStatus.TurnRobotStiff();
            /*if (robotConfig.HasHead())
            {
                robotStatus.SetEmotionsActive(true);
            }*/
            if (robotConfig.HasMobilePlatform() && connectionStatus.IsRobotInMobileRoom())
            {
                robotStatus.SetMobilityActive(true);
            }
            if (robotStatus.IsRobotPositionLocked || robotStatus.IsGraspingLockActivated())
            {
                if (robotStatus.IsLeftGripperClosed())
                {
                    UserInputManager.Instance.UserMovementsInput.ForceLeftGripperStatus(true);
                    robotStatus.SetGraspingLockActivated(true);
                }
                if (robotStatus.IsRightGripperClosed())
                {
                    UserInputManager.Instance.UserMovementsInput.ForceRightGripperStatus(true);
                    robotStatus.SetGraspingLockActivated(true);
                }
            }
            robotStatus.StartRobotTeleoperation();
        }

        void StopTeleoperation()
        {
            Debug.Log("[TeleoperationManager]: StopTeleoperation");
            //robotStatus.SetEmotionsActive(false);
            robotStatus.SetMobilityActive(false);
            robotStatus.StopRobotTeleoperation();
            DisplayReachy(true);
        }

        void SuspendTeleoperation()
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                robotStatus.SuspendRobotTeleoperation();
            }
        }

        private void DisplayReachy(bool enabled)
        {
            reachy.switchRenderer(enabled);
            if (robotConfig.HasHead())
                reachy.GetChild(0).switchRenderer(enabled);
            if (robotConfig.HasLeftArm())
                reachy.GetChild(1).switchRenderer(enabled);
            if (robotConfig.HasRightArm())
                reachy.GetChild(3).switchRenderer(enabled);
            if (robotConfig.HasMobilePlatform())
                reachy.GetChild(5).switchRenderer(enabled);
        }

        void CheckChanges()
        {
            if (!connectionStatus.IsRobotInMobileRoom())
            {
                robotStatus.SetMobilityActive(false);
            }
            else
            {
                robotStatus.SetMobilityActive(true);
            }
        }
    }
}