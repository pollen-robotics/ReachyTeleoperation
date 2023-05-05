using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachy.Sdk.Joint;
using Reachy.Sdk.Kinematics;
using Reachy;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace TeleopReachy
{
    public class ReachySimulatedCommands : MonoBehaviour
    {
        private HeadTracker headTracker;
        private HandsTracker handsTracker;
        private UserMovementsInput userMovementsInput;

        [SerializeField]
        private ReachySimulatedServer reachyFakeServer;

        [SerializeField]
        private ReachyController reachy;

        [SerializeField]
        private ReachyController reachyModel;

        private JointPosition q0_left;
        private JointPosition q0_right;

        private RobotConfig robotConfig;
        private RobotStatus robotStatus;

        public UnityEvent<Emotion> event_OnEmotionOver;

        // Token to cancel emotions
        private CancellationTokenSource askForCancellation = new CancellationTokenSource();

        // Start is called before the first frame update
        void Start()
        {
            headTracker = UserTrackerManager.Instance.HeadTracker;
            handsTracker = UserTrackerManager.Instance.HandsTracker;
            userMovementsInput = UserInputManager.Instance.UserMovementsInput;

            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;

            reachy = GameObject.Find("Reachy").transform.GetComponent<ReachyController>();

            q0_left = new JointPosition
            {
                Ids = { new JointId{ Name = "l_shoulder_pitch"}, new JointId{ Name = "l_shoulder_roll"}, new JointId{ Name = "l_arm_yaw"},
                new JointId{ Name = "l_elbow_pitch"}, new JointId{ Name = "l_forearm_yaw"}, new JointId{ Name = "l_wrist_pitch"}, new JointId{ Name = "l_wrist_roll"} },
                Positions = { 0, 0, 0, -Mathf.PI / 2, 0, 0, 0 },
            };

            q0_right = new JointPosition
            {
                Ids = { new JointId{ Name = "r_shoulder_pitch"}, new JointId{ Name = "r_shoulder_roll"}, new JointId{ Name = "r_arm_yaw"},
                new JointId{ Name = "r_elbow_pitch"}, new JointId{ Name = "r_forearm_yaw"}, new JointId{ Name = "r_wrist_pitch"}, new JointId{ Name = "r_wrist_roll"} },
                Positions = { 0, 0, 0, -Mathf.PI / 2, 0, 0, 0 },
            };
        }

        // Update is called once per frame
        void Update()
        {
            ArmEndEffector rightEndEffector = userMovementsInput.GetRightEndEffectorTarget();
            ArmEndEffector leftEndEffector = userMovementsInput.GetLeftEndEffectorTarget();

            ArmIKRequest rightArmRequest = new ArmIKRequest { Target = rightEndEffector, Q0 = q0_right };
            ArmIKRequest leftArmRequest = new ArmIKRequest { Target = leftEndEffector, Q0 = q0_left };

            HeadIKRequest headTarget = headTracker.GetHeadTarget();

            float pos_right_gripper = userMovementsInput.GetRightGripperTarget();
            float pos_left_gripper = userMovementsInput.GetLeftGripperTarget();

            FullBodyCartesianCommand bodyCommand = new FullBodyCartesianCommand();
            if (robotConfig.IsVirtual() || (robotConfig.HasLeftArm() && robotStatus.IsLeftArmOn()))
                bodyCommand.LeftArm = leftArmRequest;
            if (robotConfig.IsVirtual() || !robotStatus.IsLeftArmOn())
                SetLeftArmToModelPose();

            if (robotConfig.IsVirtual() || (robotConfig.HasRightArm() && robotStatus.IsRightArmOn()))
                bodyCommand.RightArm = rightArmRequest;
            if (robotConfig.IsVirtual() || !robotStatus.IsRightArmOn())
                SetRightArmToModelPose();

            if (robotConfig.IsVirtual() || robotConfig.HasHead() && robotStatus.IsHeadOn())
                bodyCommand.Head = headTarget;
            if (robotConfig.IsVirtual() || !robotStatus.IsHeadOn())
                SetHeadToModelPose();

            List<JointCommand> grippersCommand = new List<JointCommand>();

            if (robotConfig.IsVirtual() || (robotConfig.HasLeftGripper() && robotStatus.IsLeftArmOn()))
            {
                var jointComL = new JointCommand();
                jointComL.Id = new JointId { Name = "l_gripper" };
                jointComL.GoalPosition = pos_left_gripper;

                grippersCommand.Add(jointComL);
            }

            if (robotConfig.IsVirtual() || (robotConfig.HasRightGripper() && robotStatus.IsRightArmOn()))
            {
                var jointComR = new JointCommand();
                jointComR.Id = new JointId { Name = "r_gripper" };
                jointComR.GoalPosition = pos_right_gripper;

                grippersCommand.Add(jointComR);
            }

            JointsCommand gripperCommand = new JointsCommand
            {
                Commands = { grippersCommand },
            };

            reachyFakeServer.SendFullBodyCartesianCommands(bodyCommand);
            reachyFakeServer.SendJointsCommands(gripperCommand);
        }

        void SetHeadToModelPose()
        {
            Dictionary<JointId, JointField> headJoints = new Dictionary<JointId, JointField>();
            JointField field = JointField.GoalPosition;
            var joint = new JointId();
            joint.Name = "neck_pitch";
            headJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "neck_roll";
            headJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "neck_yaw";
            headJoints.Add(joint, field);

            List<SerializableMotor> headMotors = reachyModel.GetCurrentMotorsState(headJoints);

            Dictionary<JointId, float> headTarget = new Dictionary<JointId, float>();
            for (int i = 0; i < headMotors.Count; i++)
            {
                joint = new JointId();
                joint.Name = headMotors[i].name;
                float goal = Mathf.Rad2Deg * headMotors[i].goal_position;
                headTarget.Add(joint, goal);
            }

            reachy.HandleCommand(headTarget);
        }

        void SetLeftArmToModelPose()
        {
            Dictionary<JointId, JointField> leftJoints = new Dictionary<JointId, JointField>();
            JointField field = JointField.GoalPosition;
            var joint = new JointId();
            joint.Name = "l_shoulder_pitch";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_shoulder_roll";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_arm_yaw";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_elbow_pitch";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_forearm_yaw";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_wrist_pitch";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_wrist_roll";
            leftJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "l_gripper";
            leftJoints.Add(joint, field);

            List<SerializableMotor> leftArmMotors = reachyModel.GetCurrentMotorsState(leftJoints);

            Dictionary<JointId, float> leftArmTarget = new Dictionary<JointId, float>();
            for (int i = 0; i < leftArmMotors.Count; i++)
            {
                joint = new JointId();
                joint.Name = leftArmMotors[i].name;
                float goal = Mathf.Rad2Deg * leftArmMotors[i].goal_position;
                leftArmTarget.Add(joint, goal);
            }

            reachy.HandleCommand(leftArmTarget);
        }

        void SetRightArmToModelPose()
        {
            Dictionary<JointId, JointField> rightJoints = new Dictionary<JointId, JointField>();
            JointField field = JointField.GoalPosition;
            var joint = new JointId();
            joint.Name = "r_shoulder_pitch";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_shoulder_roll";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_arm_yaw";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_elbow_pitch";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_forearm_yaw";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_wrist_pitch";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_wrist_roll";
            rightJoints.Add(joint, field);
            joint = new JointId();
            joint.Name = "r_gripper";
            rightJoints.Add(joint, field);

            List<SerializableMotor> rightArmMotors = reachyModel.GetCurrentMotorsState(rightJoints);

            Dictionary<JointId, float> rightArmTarget = new Dictionary<JointId, float>();
            for (int i = 0; i < rightArmMotors.Count; i++)
            {
                joint = new JointId();
                joint.Name = rightArmMotors[i].name;
                float goal = Mathf.Rad2Deg * rightArmMotors[i].goal_position;
                rightArmTarget.Add(joint, goal);
            }

            reachy.HandleCommand(rightArmTarget);
        }

        public async void ReachyHappy()
        {
            Debug.Log("Simulated Reachy is happy");
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
                reachyFakeServer.SendJointsCommands(antennasSpeedBack);
                for (int i = 0; i < 9; i++)
                {
                    reachyFakeServer.SendJointsCommands(antennasCommand1);
                    await Task.Delay(100);
                    reachyFakeServer.SendJointsCommands(antennasCommand2);
                    await Task.Delay(100);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await Task.Delay(200);
                reachyFakeServer.SendJointsCommands(antennasCommandBack);
                reachyFakeServer.SendJointsCommands(antennasSpeedBack);
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

        public async void ReachySad()
        {
            Debug.Log("Simulated Reachy is sad");
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
                reachyFakeServer.SendJointsCommands(antennasSpeedLimit);
                reachyFakeServer.SendJointsCommands(antennasCommand1);
                await Task.Delay(2000);
                cancellationToken.ThrowIfCancellationRequested();
                reachyFakeServer.SendJointsCommands(antennasSpeedLimit2);
                reachyFakeServer.SendJointsCommands(antennasCommand2);
                await Task.Delay(600);
                reachyFakeServer.SendJointsCommands(antennasCommand1);
                await Task.Delay(600);
                cancellationToken.ThrowIfCancellationRequested();
                reachyFakeServer.SendJointsCommands(antennasCommand2);
                await Task.Delay(600);
                reachyFakeServer.SendJointsCommands(antennasCommand1);
                await Task.Delay(1000);
                reachyFakeServer.SendJointsCommands(antennasSpeedLimit);
                reachyFakeServer.SendJointsCommands(antennasCommandBack);
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
                reachyFakeServer.SendJointsCommands(antennasSpeedLimit);
                reachyFakeServer.SendJointsCommands(antennasCommand1);
                await Task.Delay(2000);
                cancellationToken.ThrowIfCancellationRequested();
                reachyFakeServer.SendJointsCommands(antennasCommandBack);
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
                    reachyFakeServer.SendJointsCommands(antennasSpeedBack);
                    reachyFakeServer.SendJointsCommands(antennasCommand1);
                    await Task.Delay(1000);
                    cancellationToken.ThrowIfCancellationRequested();
                    reachyFakeServer.SendJointsCommands(antennasSpeedLimit2);
                    reachyFakeServer.SendJointsCommands(antennasCommand2);
                    await Task.Delay(500);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                reachyFakeServer.SendJointsCommands(antennasSpeedBack);
                reachyFakeServer.SendJointsCommands(antennasCommand1);
                await Task.Delay(1500);
                cancellationToken.ThrowIfCancellationRequested();
                reachyFakeServer.SendJointsCommands(antennasSpeedLimit2);

                reachyFakeServer.SendJointsCommands(antennasCommandBack);
                cancellationToken.ThrowIfCancellationRequested();
                event_OnEmotionOver.Invoke(Emotion.Angry);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("Reachy angry has been canceled: " + e);
                event_OnEmotionOver.Invoke(Emotion.Angry);
            }
        }
    }
}

