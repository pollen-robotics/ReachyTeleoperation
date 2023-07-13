using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Grpc.Core;
using Reachy.Sdk.Joint;
using Reachy.Sdk.Kinematics;

namespace TeleopReachy
{
    public class RobotJointCommands : RobotCommands
    {
        private gRPCDataController dataController;
        private ConnectionStatus connectionStatus;

        public Coroutine setSmoothCompliance;
        public Coroutine waitToSetRobotFullSpeed;


        // Start is called before the first frame update
        void Start()
        {
            Init();
            dataController = gRPCManager.Instance.gRPCDataController;
            connectionStatus = gRPCManager.Instance.ConnectionStatus;

            robotStatus.event_OnInitializeRobotStateRequested.AddListener(InitializeRobotState);
            robotStatus.event_OnRobotStiffRequested.AddListener(SetRobotStiff);
            robotStatus.event_OnRobotCompliantRequested.AddListener(SetRobotCompliant);
            robotStatus.event_OnRobotSmoothlyCompliantRequested.AddListener(SetRobotSmoothlyCompliant);

            robotStatus.event_OnSuspendTeleoperation.AddListener(SuspendTeleoperation);
            robotStatus.event_OnResumeTeleoperation.AddListener(ResumeTeleoperation);

            robotStatus.event_OnStartTeleoperation.AddListener(StartTeleoperation);
            robotStatus.event_OnStopTeleoperation.AddListener(StopTeleoperation);

            setSmoothCompliance = null;
            waitToSetRobotFullSpeed = null;
        }

        void OnDestroy()
        {
            robotStatus.SetLeftArmOn(false);
            robotStatus.SetRightArmOn(false);
            robotStatus.SetHeadOn(false);
            if (!robotConfig.IsVirtual())
                SetRobotCompliant();
        }

        protected override void ActualSendGrippersCommands(JointsCommand gripperCommand)
        {
            dataController.SendGrippersCommand(gripperCommand);
        }

        protected override void ActualSendBodyCommands(FullBodyCartesianCommand bodyCommand)
        {
            dataController.SendBodyCommand(bodyCommand);
        }

        private void SetRobotSmoothlyCompliant()
        {
            Debug.Log("[RobotJointCommands]: SetRobotSmoothlyCompliant");
            setSmoothCompliance = StartCoroutine(SmoothCompliance(2));
        }

        private void SetRobotStiff()
        {
            ToggleStiffness();
            robotStatus.SetRobotCompliant(false);
        }

        private void ToggleStiffness()
        {
            if (robotConfig.HasLeftArm())
            {
                if (robotStatus.IsLeftArmOn())
                    SetRobotStiff("l_");
                else
                    SetRobotCompliant("l_");
            }
            if (robotConfig.HasRightArm())
            {
                if (robotStatus.IsRightArmOn())
                    SetRobotStiff("r_");
                else
                    SetRobotCompliant("r_");
            }
            if (robotConfig.HasHead())
            {
                if (robotStatus.IsHeadOn())
                    SetRobotStiff("neck_");
                else
                    SetRobotCompliant("neck_");
            }
        }

        //partName should be l_, r_ or neck_

        private void SetRobotStiff(string partName = "")
        {
            Debug.Log("[RobotJointCommands]: SetRobotStiff " + partName);
            if (setSmoothCompliance != null)
            {
                StopCoroutine(setSmoothCompliance);
            }
            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                if (partName == "" || item.StartsWith(partName))
                {
                    var jointCom = new JointCommand();
                    jointCom.Id = new JointId { Name = item };
                    jointCom.Compliant = false;
                    jointCom.TorqueLimit = 100;

                    if (item.Contains("antenna"))
                    {
                        jointCom.GoalPosition = Mathf.Deg2Rad * (0);
                    }

                    listCommand.Add(jointCom);
                }
            };

            JointsCommand armsCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(armsCommand);
        }

        private void SetRobotCompliant()
        {
            ToggleStiffness();
            robotStatus.SetRobotCompliant(true);
        }

        //partName should be l_, r_ or neck_
        private void SetRobotCompliant(string partName = "")
        {
            Debug.Log("[RobotJointCommands]: SetRobotCompliant " + partName);
            if (setSmoothCompliance != null)
            {
                StopCoroutine(setSmoothCompliance);
            }
            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                if (partName == "" || item.StartsWith(partName))
                {
                    var jointCom = new JointCommand();
                    jointCom.Id = new JointId { Name = item };
                    jointCom.Compliant = true;

                    if (item.Contains("antenna"))
                    {
                        jointCom.GoalPosition = Mathf.Deg2Rad * (0);
                    }

                    listCommand.Add(jointCom);
                }
            };

            JointsCommand armsCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(armsCommand);
        }

        private void StartTeleoperation()
        {
            Debug.Log("[RobotJointCommands]: StartTeleoperation");
            waitToSetRobotFullSpeed = StartCoroutine(ResetReachyMotorsFullSpeed());
        }

        private void StopTeleoperation()
        {
            Debug.Log("[RobotJointCommands]: StopTeleoperation");
            if (connectionStatus.IsServerConnected())
            {
                AskForCancellationCurrentMovementsPlaying();
                if (waitToSetRobotFullSpeed != null)
                {
                    StopCoroutine(waitToSetRobotFullSpeed);
                }
                if (!robotStatus.IsRobotPositionLocked) SetRobotSmoothlyCompliant();
                ResetMotorsStartingSpeed();
            }
        }

        private void InitializeRobotState()
        {
            Debug.Log("[RobotJointCommands]: InitializeRobotState");
            StartCoroutine(ResetTorqueMax());
            ResetMotorsStartingSpeed();
        }

        IEnumerator ResetTorqueMax()
        {
            if (setSmoothCompliance != null) yield return setSmoothCompliance;

            float limit = 100;

            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                var jointCom = new JointCommand();
                jointCom.Id = new JointId { Name = item };
                jointCom.TorqueLimit = limit;

                if (item.Contains("antenna"))
                {
                    jointCom.GoalPosition = Mathf.Deg2Rad * (0);
                }

                listCommand.Add(jointCom);
            };

            JointsCommand command = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(command);
        }

        private void ResetMotorsStartingSpeed()
        {
            robotStatus.SetMotorsSpeedLimited(true);

            float speedLimit = Mathf.Deg2Rad * 35;

            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                if (!item.StartsWith("neck"))
                {
                    var jointCom = new JointCommand();
                    jointCom.Id = new JointId { Name = item };
                    if (!item.Contains("antenna"))
                    {
                        jointCom.SpeedLimit = speedLimit;
                        if (item.Contains("gripper"))
                        {
                            jointCom.SpeedLimit = 0;
                        }
                    }
                    else
                    {
                        jointCom.Id = new JointId { Name = item };
                        jointCom.SpeedLimit = 0;
                        jointCom.GoalPosition = 0;
                    }

                    listCommand.Add(jointCom);
                }
            };

            JointsCommand speedCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(speedCommand);
        }

        private void SetHeadLookingStraight()
        {
            Reachy.Sdk.Kinematics.Quaternion unitQ = new Reachy.Sdk.Kinematics.Quaternion
            {
                W = 1,
                X = 0,
                Y = 0,
                Z = 0,
            };

            FullBodyCartesianCommand bodyCommand = new FullBodyCartesianCommand();
            if (robotConfig.HasHead())
            {
                bodyCommand.Head = new HeadIKRequest { Q = unitQ };
            }

            dataController.SendImmediateBodyCommand(bodyCommand);
        }

        private IEnumerator SmoothCompliance(int duration)
        {
            float lowLimit = 1;
            float highLimit = 100;

            SetHeadLookingStraight();

            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                var jointCom = new JointCommand();
                jointCom.Id = new JointId { Name = item };
                if (!item.Contains("neck") && !item.Contains("antenna"))
                {
                    jointCom.TorqueLimit = lowLimit;

                }
                listCommand.Add(jointCom);
            };

            JointsCommand torqueCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(torqueCommand);

            yield return new WaitForSeconds(duration);

            listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Uids)
            {
                var jointCom = new JointCommand();
                jointCom.Id = new JointId { Uid = item };
                jointCom.Compliant = true;
                jointCom.TorqueLimit = highLimit;

                listCommand.Add(jointCom);
            };

            JointsCommand complianceCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(complianceCommand);

            robotStatus.SetRobotCompliant(true);
        }

        private IEnumerator ResetReachyMotorsFullSpeed()
        {
            Debug.Log("[RobotJointCommands]: ResetReachyMotorsFullSpeed");
            yield return new WaitForSeconds(3);

            float speedLimit = Mathf.Deg2Rad * 0;

            List<JointCommand> listCommand = new List<JointCommand>();
            foreach (var item in robotConfig.GetAllJointsId().Names)
            {
                if (!item.StartsWith("neck"))
                {
                    var jointCom = new JointCommand();
                    jointCom.Id = new JointId { Name = item };
                    jointCom.SpeedLimit = speedLimit;

                    if (item.Contains("gripper"))
                    {
                        jointCom.SpeedLimit = 0;
                    }

                    if (item.Contains("antenna"))
                    {
                        jointCom.SpeedLimit = 0;
                        jointCom.GoalPosition = 0;
                    }

                    listCommand.Add(jointCom);
                }

            };

            JointsCommand speedCommand = new JointsCommand
            {
                Commands = { listCommand },
            };

            SendJointsCommands(speedCommand);
            robotStatus.SetMotorsSpeedLimited(false);
        }

        protected override void SendJointsCommands(JointsCommand jointsCommand)
        {
            dataController.SendJointsCommand(jointsCommand);
        }

        void SuspendTeleoperation()
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                try
                {
                    if (waitToSetRobotFullSpeed != null)
                    {
                        StopCoroutine(waitToSetRobotFullSpeed);
                    }
                    ResetMotorsStartingSpeed();
                    if (setSmoothCompliance != null) StopCoroutine(setSmoothCompliance);
                    setSmoothCompliance = StartCoroutine(SmoothCompliance(5));
                    if (robotStatus.IsHeadOn()) SetHeadLookingStraight();
                }
                catch (Exception exc)
                {
                    Debug.Log($"[RobotJointCommands]: SuspendTeleoperation error: {exc}");
                }
            }
        }

        void ResumeTeleoperation()
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                if (!robotStatus.HasMotorsSpeedLimited())
                {
                    ResetMotorsStartingSpeed();
                }
            }
        }
    }
}
