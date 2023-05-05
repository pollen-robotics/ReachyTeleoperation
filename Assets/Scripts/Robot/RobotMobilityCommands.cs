using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Grpc.Core;
using Reachy.Sdk.Mobility;
using Reachy;

namespace TeleopReachy
{
    public class RobotMobilityCommands : MonoBehaviour
    {
        private gRPCMobileBaseController mobileController;

        private RobotStatus robotStatus;
        private RobotConfig robotConfig;

        void Start()
        {
            mobileController = gRPCManager.Instance.gRPCMobileBaseController;

            robotConfig = transform.GetComponent<RobotConfig>();
            robotStatus = transform.GetComponent<RobotStatus>();
            robotStatus.event_OnStartTeleoperation.AddListener(StartMobility);
            robotStatus.event_OnStopTeleoperation.AddListener(StopMobility);
            robotStatus.event_OnSuspendTeleoperation.AddListener(StopMobileBaseMovements);
        }

        private void StartMobility()
        {
            if (robotConfig.HasMobilePlatform() && robotStatus.IsMobilityOn())
            {
                SendZuuuVelocityMode();
            }
        }

        private void StopMobility()
        {
            if (robotConfig.HasMobilePlatform())
            {
                StopMobileBaseMovements();
            }
        }

        private void SendZuuuVelocityMode()
        {
            ZuuuModeCommand zuuuMode = new ZuuuModeCommand { Mode = ZuuuModePossiblities.CmdVel };
            mobileController.SendZuuuMode(zuuuMode);
        }


        public void SendMobileBaseDirection(Vector3 direction)
        {
            TargetDirectionCommand command = new TargetDirectionCommand
            {
                Direction = new DirectionVector
                {
                    X = direction[0],
                    Y = direction[1],
                    Theta = direction[2],
                }
            };
            mobileController.SendMobilityCommand(command);
        }

        void StopMobileBaseMovements()
        {
            try
            {
                Vector2 direction = new Vector2(0, 0);
                SendMobileBaseDirection(direction);
            }
            catch (Exception exc)
            {
                Debug.Log($"[RobotMobilityCommands]: StopMobileBaseMovements error: {exc}");
            }
        }
    }
}