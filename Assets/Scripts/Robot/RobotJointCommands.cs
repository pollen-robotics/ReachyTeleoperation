using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Grpc.Core;
using Reachy.Sdk.Joint;
using Reachy.Sdk.Kinematics;

namespace TeleopReachy
{
    public class RobotJointCommands : MonoBehaviour
    {
        private gRPCDataController dataController;
        private ConnectionStatus connectionStatus;
        private RobotStatus robotStatus;

        private RobotConfig robotConfig;

        public Coroutine setSmoothCompliance;
        public Coroutine waitToSetRobotFullSpeed;

        public UnityEvent<Emotion> event_OnEmotionOver;

        // Token to cancel emotions
        private CancellationTokenSource askForCancellation = new CancellationTokenSource();

        // Start is called before the first frame update
        void Start()
        {
            dataController = gRPCManager.Instance.gRPCDataController;
            connectionStatus = gRPCManager.Instance.ConnectionStatus;

            robotConfig = RobotDataManager.Instance.RobotConfig;

            robotStatus = RobotDataManager.Instance.RobotStatus;
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

        public void SendFullBodyCommands(ArmIKRequest leftArmRequest, ArmIKRequest rightArmRequest, HeadIKRequest headRequest)
        {
            FullBodyCartesianCommand bodyCommand = new FullBodyCartesianCommand();
            if (robotConfig.HasLeftArm() && robotStatus.IsLeftArmOn())
            {
                bodyCommand.LeftArm = leftArmRequest;
            }
            if (robotConfig.HasRightArm() && robotStatus.IsRightArmOn())
            {
                bodyCommand.RightArm = rightArmRequest;
            }
            if (robotConfig.HasHead() && robotStatus.IsHeadOn())
            {
                bodyCommand.Head = headRequest;
            }

            dataController.SendBodyCommand(bodyCommand);
        }

        public void SendGrippersCommands(float leftGripperOpening, float rightGripperOpening)
        {
            List<JointCommand> grippersCommand = new List<JointCommand>();

            if (robotConfig.HasLeftGripper() && robotStatus.IsLeftArmOn())
            {
                var jointCom = new JointCommand();
                jointCom.Id = new JointId { Name = "l_gripper" };
                jointCom.GoalPosition = leftGripperOpening;

                grippersCommand.Add(jointCom);
            }

            if (robotConfig.HasRightGripper() && robotStatus.IsRightArmOn())
            {
                var jointCom = new JointCommand();
                jointCom.Id = new JointId { Name = "r_gripper" };
                jointCom.GoalPosition = rightGripperOpening;

                grippersCommand.Add(jointCom);
            }

            JointsCommand gripperCommand = new JointsCommand
            {
                Commands = { grippersCommand },
            };

            dataController.SendGrippersCommand(gripperCommand);
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

            dataController.SendJointsCommand(armsCommand);
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

            dataController.SendJointsCommand(armsCommand);
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
            if(setSmoothCompliance != null) yield return setSmoothCompliance;

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

            dataController.SendJointsCommand(command);
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

            dataController.SendJointsCommand(speedCommand);
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

            dataController.SendJointsCommand(torqueCommand);

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

            dataController.SendJointsCommand(complianceCommand);

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

            dataController.SendJointsCommand(speedCommand);
            robotStatus.SetMotorsSpeedLimited(false);
        }

        public async void ReachySad()
        {
            Debug.Log("Reachy is sad");
            CancellationToken cancellationToken = askForCancellation.Token;

            JointsCommand antennasSpeedLimit = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 1.5f},
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 1.5f },
                    }
            };
            JointsCommand antennasSpeedLimit2 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 0.7f},
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 0.7f },
                    }
            };
            JointsCommand antennasCommand1 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(140) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-140) },
                    }
            };
            JointsCommand antennasCommand2 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(120) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-120) },
                    }
            };

            JointsCommand antennasCommandBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    }
            };

            try
            {
                dataController.SendJointsCommand(antennasSpeedLimit);
                dataController.SendJointsCommand(antennasCommand1);
                await Task.Delay(2000);
                cancellationToken.ThrowIfCancellationRequested();
                dataController.SendJointsCommand(antennasSpeedLimit2);
                dataController.SendJointsCommand(antennasCommand2);
                await Task.Delay(600);
                dataController.SendJointsCommand(antennasCommand1);
                await Task.Delay(600);
                cancellationToken.ThrowIfCancellationRequested();
                dataController.SendJointsCommand(antennasCommand2);
                await Task.Delay(600);
                dataController.SendJointsCommand(antennasCommand1);
                await Task.Delay(1000);
                dataController.SendJointsCommand(antennasSpeedLimit);
                dataController.SendJointsCommand(antennasCommandBack);
                cancellationToken.ThrowIfCancellationRequested();
                //EmotionIsOver(new EmotionEventArgs(Emotion.Sad));
                event_OnEmotionOver.Invoke(Emotion.Sad);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("Reachy sad has been canceled: " + e);
                //EmotionIsOver(new EmotionEventArgs(Emotion.Sad));
                event_OnEmotionOver.Invoke(Emotion.Sad);
            }
        }

        public async void ReachyHappy()
        {
            Debug.Log("Reachy is happy");
            CancellationToken cancellationToken = askForCancellation.Token;

            JointsCommand antennasCommand1 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(10) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-10) },
                    }
            };
            JointsCommand antennasCommand2 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(-10) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(10) },
                    }
            };

            JointsCommand antennasCommandBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    }
            };
            JointsCommand antennasSpeedBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 0 },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 0 },
                    }
            };

            try
            {
                dataController.SendJointsCommand(antennasSpeedBack);
                for (int i = 0; i < 9; i++)
                {
                    dataController.SendJointsCommand(antennasCommand1);
                    await Task.Delay(100);
                    dataController.SendJointsCommand(antennasCommand2);
                    await Task.Delay(100);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await Task.Delay(200);
                dataController.SendJointsCommand(antennasCommandBack);
                dataController.SendJointsCommand(antennasSpeedBack);
                cancellationToken.ThrowIfCancellationRequested();
                //EmotionIsOver(new EmotionEventArgs(Emotion.Happy));
                event_OnEmotionOver.Invoke(Emotion.Happy);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("Reachy happy has been canceled: " + e);
                //EmotionIsOver(new EmotionEventArgs(Emotion.Happy));
                event_OnEmotionOver.Invoke(Emotion.Happy);
            }

        }

        public async void ReachyConfused()
        {
            Debug.Log("Reachy is confused");
            CancellationToken cancellationToken = askForCancellation.Token;

            JointsCommand antennasSpeedLimit = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 2.3f},
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 2.3f },
                    }
            };
            JointsCommand antennasCommand1 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(-20) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-80) },
                    }
            };
            JointsCommand antennasCommandBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    }
            };
            JointsCommand antennasSpeedBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 0 },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 0 },
                    }
            };

            try
            {
                dataController.SendJointsCommand(antennasSpeedLimit);
                dataController.SendJointsCommand(antennasCommand1);
                await Task.Delay(2000);
                cancellationToken.ThrowIfCancellationRequested();
                dataController.SendJointsCommand(antennasCommandBack);
                cancellationToken.ThrowIfCancellationRequested();
                event_OnEmotionOver.Invoke(Emotion.Confused);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("Reachy confused has been canceled: " + e);
                event_OnEmotionOver.Invoke(Emotion.Confused);
            }
        }

        public async void ReachyAngry()
        {
            Debug.Log("Reachy is angry");
            CancellationToken cancellationToken = askForCancellation.Token;

            JointsCommand antennasSpeedLimit1 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 5f},
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 5f },
                    }
            };
            JointsCommand antennasSpeedLimit2 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna"  }, SpeedLimit = 2.3f},
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 2.3f },
                    }
            };
            JointsCommand antennasCommand1 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(80) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-80) },
                    }
            };
            JointsCommand antennasCommand2 = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(40) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(-40) },
                    }
            };
            JointsCommand antennasCommandBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, GoalPosition=Mathf.Deg2Rad*(0) },
                    }
            };
            JointsCommand antennasSpeedBack = new JointsCommand
            {
                Commands = {
                    new JointCommand { Id=new JointId { Name = "l_antenna" }, SpeedLimit = 0 },
                    new JointCommand { Id=new JointId { Name = "r_antenna" }, SpeedLimit = 0 },
                    }
            };

            try
            {
                for (int i = 0; i < 2; i++)
                {
                    dataController.SendJointsCommand(antennasSpeedBack);
                    dataController.SendJointsCommand(antennasCommand1);
                    await Task.Delay(1000);
                    cancellationToken.ThrowIfCancellationRequested();
                    dataController.SendJointsCommand(antennasSpeedLimit2);
                    dataController.SendJointsCommand(antennasCommand2);
                    await Task.Delay(500);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                dataController.SendJointsCommand(antennasSpeedBack);
                dataController.SendJointsCommand(antennasCommand1);
                await Task.Delay(1500);
                cancellationToken.ThrowIfCancellationRequested();
                dataController.SendJointsCommand(antennasSpeedLimit2);

                dataController.SendJointsCommand(antennasCommandBack);
                cancellationToken.ThrowIfCancellationRequested();
                event_OnEmotionOver.Invoke(Emotion.Angry);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("Reachy angry has been canceled: " + e);
                event_OnEmotionOver.Invoke(Emotion.Angry);
            }
        }

        private void AskForCancellationCurrentMovementsPlaying()
        {
            askForCancellation.Cancel();
            StartCoroutine(DisposeToken());
        }

        IEnumerator DisposeToken()
        {
            yield return new WaitForSeconds(0.1f);

            askForCancellation.Dispose();
            askForCancellation = new CancellationTokenSource();
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
                    if(setSmoothCompliance != null) StopCoroutine(setSmoothCompliance);
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
                // StartTeleoperation();
            }
        }
    }
}
