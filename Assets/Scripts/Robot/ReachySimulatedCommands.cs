using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachy.Sdk.Joint;
using Reachy.Sdk.Kinematics;
using Reachy;

namespace TeleopReachy
{
    public class ReachySimulatedCommands : RobotCommands
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



        // Start is called before the first frame update
        void Start()
        {
            Init();
            headTracker = UserTrackerManager.Instance.HeadTracker;
            handsTracker = UserTrackerManager.Instance.HandsTracker;
            userMovementsInput = UserInputManager.Instance.UserMovementsInput;

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

            if (robotConfig.IsVirtual() || !robotStatus.IsLeftArmOn())
                SetLeftArmToModelPose();

            if (robotConfig.IsVirtual() || !robotStatus.IsRightArmOn())
                SetRightArmToModelPose();

            if (robotConfig.IsVirtual() || !robotStatus.IsHeadOn())
                SetHeadToModelPose();
            SendFullBodyCommands(leftArmRequest, rightArmRequest, headTarget);

            float pos_right_gripper = userMovementsInput.GetRightGripperTarget();
            float pos_left_gripper = userMovementsInput.GetLeftGripperTarget();
            SendGrippersCommands(pos_left_gripper, pos_right_gripper);
        }

        protected override void ActualSendBodyCommands(FullBodyCartesianCommand bodyCommand)
        {
            reachyFakeServer.SendFullBodyCartesianCommands(bodyCommand);
        }

        protected override void ActualSendGrippersCommands(JointsCommand gripperCommand)
        {
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

        protected override void SendJointsCommands(JointsCommand jointsCommand)
        {
            reachyFakeServer.SendJointsCommands(jointsCommand);
        }

    }
}

