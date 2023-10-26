using UnityEngine;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reachy;

using Grpc.Core;
using Grpc.Core.Utils;
using Reachy.Sdk.Joint;
using Reachy.Sdk.Fan;
using Reachy.Sdk.Kinematics;
using Reachy.Sdk.Mobility;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class ReachySimulatedServer : MonoBehaviour
{
    public static ReachyController reachy;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("Arm_kinematics.dll", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    [DllImport("libArm_kinematics.so", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    [DllImport("libArm_kinematics.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_ANDROID
    [DllImport("libArm_kinematics.android.so", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void setup();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("Arm_kinematics.dll", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    [DllImport("libArm_kinematics.so", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    [DllImport("libArm_kinematics.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif UNITY_ANDROID
    [DllImport("libArm_kinematics.android.so", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void inverse(ArmSide side, double[] M, double[] q);

    void Start()
    {
        reachy = GameObject.Find("Reachy").GetComponent<ReachyController>();
        setup(); // Setup Arm_kinematics
    }


    public JointsCommandAck SendJointsCommands(JointsCommand jointsCommand)
    {
        try
        {
            Dictionary<JointId, float> commands = new Dictionary<JointId, float>();
            Dictionary<JointId, bool> compliancy = new Dictionary<JointId, bool>();
            for (int i = 0; i < jointsCommand.Commands.Count; i++)
            {
                if (jointsCommand.Commands[i].GoalPosition != null)
                {
                    float command = Mathf.Rad2Deg * (float)jointsCommand.Commands[i].GoalPosition;
                    commands.Add(jointsCommand.Commands[i].Id, command);
                }

                if (jointsCommand.Commands[i].Compliant != null)
                {
                    bool isCompliant = (bool)jointsCommand.Commands[i].Compliant;
                    compliancy.Add(jointsCommand.Commands[i].Id, isCompliant);
                }
            }
            reachy.HandleCommand(commands);
            reachy.HandleCompliancy(compliancy);
            return new JointsCommandAck { Success = true };
        }
        catch
        {
            return new JointsCommandAck { Success = false };
        }
    }

    public JointsState GetJointsState(JointsStateRequest jointRequest)
    {
        Dictionary<JointId, JointField> request = new Dictionary<JointId, JointField>();

        for (int i = 0; i < jointRequest.Ids.Count; i++)
        {
            request.Add(jointRequest.Ids[i], JointField.PresentPosition);
        }
        var motors = reachy.GetCurrentMotorsState(request);

        List<JointState> listJointStates = new List<JointState>();
        List<JointId> listJointIds = new List<JointId>();
        foreach (var item in motors)
        {
            var jointState = new JointState();
            jointState.Name = item.name;
            jointState.Uid = (uint?)item.uid;
            if (jointRequest.RequestedFields.Contains(JointField.PresentPosition))
            {
                jointState.PresentPosition = item.present_position;
            }
            if (jointRequest.RequestedFields.Contains(JointField.PresentSpeed))
            {
                jointState.PresentSpeed = 0;
            }
            if (jointRequest.RequestedFields.Contains(JointField.PresentLoad))
            {
                jointState.PresentLoad = 0;
            }
            if (jointRequest.RequestedFields.Contains(JointField.Temperature))
            {
                jointState.Temperature = 0;
            }
            if (jointRequest.RequestedFields.Contains(JointField.Compliant))
            {
                jointState.Compliant = item.isCompliant;
            }
            if (jointRequest.RequestedFields.Contains(JointField.GoalPosition))
            {
                jointState.GoalPosition = item.goal_position;
            }
            if (jointRequest.RequestedFields.Contains(JointField.SpeedLimit))
            {
                jointState.SpeedLimit = 0;
            }
            if (jointRequest.RequestedFields.Contains(JointField.TorqueLimit))
            {
                jointState.TorqueLimit = 100;
            }
            if (jointRequest.RequestedFields.Contains(JointField.Pid))
            {
                jointState.Pid = new PIDValue { Pid = new PIDGains { P = 0, I = 0, D = 0 } };
            }
            if (jointRequest.RequestedFields.Contains(JointField.All))
            {
                jointState.PresentPosition = item.present_position;
                jointState.PresentSpeed = 0;
                jointState.PresentLoad = 0;
                jointState.Temperature = 0;
                jointState.Compliant = false;
                jointState.GoalPosition = item.goal_position;
                jointState.SpeedLimit = 0;
                jointState.TorqueLimit = 100;
                jointState.Pid = new PIDValue { Pid = new PIDGains { P = 0, I = 0, D = 0 } };
            }

            listJointStates.Add(jointState);
            listJointIds.Add(new JointId { Name = item.name });
        };

        JointsState state = new JointsState
        {
            Ids = { listJointIds },
            States = { listJointStates },
        };

        return state;
    }

    public JointsId GetAllJointsId(Google.Protobuf.WellKnownTypes.Empty empty)
    {
        List<uint> ids = new List<uint>();
        List<string> names = new List<string>();

        for (int i = 0; i < reachy.motors.Length; i++)
        {
            ids.Add((uint)i);
            names.Add(reachy.motors[i].name);
        }

        JointsId allIds = new JointsId
        {
            Names = { names },
            Uids = { ids },
        };

        return allIds;
    }

    public FullBodyCartesianCommandAck SendFullBodyCartesianCommands(FullBodyCartesianCommand fullBodyCartesianCommand)
    {
        try
        {
            List<JointCommand> jointCommandList = new List<JointCommand>();

            if (fullBodyCartesianCommand.LeftArm != null)
            {
                ArmIKSolution leftArmSolution = ComputeArmIK(fullBodyCartesianCommand.LeftArm);

                int iter = 0;
                foreach (var l_id in leftArmSolution.ArmPosition.Positions.Ids)
                {
                    jointCommandList.Add(new JointCommand
                    {
                        Id = l_id,
                        GoalPosition = (float?)leftArmSolution.ArmPosition.Positions.Positions[iter],
                    });
                    iter += 1;
                }
            }
            if (fullBodyCartesianCommand.RightArm != null)
            {
                ArmIKSolution rightArmSolution = ComputeArmIK(fullBodyCartesianCommand.RightArm);

                int iter = 0;
                foreach (var l_id in rightArmSolution.ArmPosition.Positions.Ids)
                {
                    jointCommandList.Add(new JointCommand
                    {
                        Id = l_id,
                        GoalPosition = (float?)rightArmSolution.ArmPosition.Positions.Positions[iter],
                    });
                    iter += 1;
                }
            }

            if (fullBodyCartesianCommand.Head != null)
            {
                /*UnityEngine.Quaternion headRotation = new UnityEngine.Quaternion((float)fullBodyCartesianCommand.Head.Q.X,
                (float)fullBodyCartesianCommand.Head.Q.Y,
                -(float)fullBodyCartesianCommand.Head.Q.Z,
                (float)fullBodyCartesianCommand.Head.Q.W);*/

                UnityEngine.Quaternion headRotation = new UnityEngine.Quaternion((float)fullBodyCartesianCommand.Head.Q.Y,
                -(float)fullBodyCartesianCommand.Head.Q.Z,
                -(float)fullBodyCartesianCommand.Head.Q.X,
                (float)fullBodyCartesianCommand.Head.Q.W);


                Vector3 neck_commands = Mathf.Deg2Rad * headRotation.eulerAngles;
                jointCommandList.Add(new JointCommand
                {
                    Id = new JointId { Name = "neck_roll" },
                    GoalPosition = (float?)neck_commands[2],
                });
                jointCommandList.Add(new JointCommand
                {
                    Id = new JointId { Name = "neck_pitch" },
                    GoalPosition = (float?)neck_commands[0],
                });
                jointCommandList.Add(new JointCommand
                {
                    Id = new JointId { Name = "neck_yaw" },
                    GoalPosition = -(float?)neck_commands[1],
                });
            }

            JointsCommand jointsCommand = new JointsCommand { Commands = { jointCommandList } };
            SendJointsCommands(jointsCommand);

            return new FullBodyCartesianCommandAck
            {
                LeftArmCommandSuccess = true,
                RightArmCommandSuccess = true,
                HeadCommandSuccess = true
            };
        }
        catch
        {
            return new FullBodyCartesianCommandAck
            {
                LeftArmCommandSuccess = false,
                RightArmCommandSuccess = false,
                HeadCommandSuccess = false
            };
        }
    }

    private ArmIKSolution ComputeArmIK(ArmIKRequest ikRequest)
    {
        ArmIKSolution sol;

        double[] M = new double[16];
        if (ikRequest.Target.Pose.Data.Count != 16)
        {
            sol = new ArmIKSolution
            {
                Success = false,
                ArmPosition = new ArmJointPosition
                {
                    Side = ikRequest.Target.Side,
                    Positions = new JointPosition
                    {
                        Ids = { new Reachy.Sdk.Joint.JointId { } },
                        Positions = { },
                    },
                },
            };

            return sol;
        }

        for (int i = 0; i < 16; i++)
        {
            M[i] = ikRequest.Target.Pose.Data[i];
        }
        double[] q = new double[7];
        inverse(ikRequest.Target.Side, M, q);

        List<double> listq = new List<double>(q);

        List<JointId> listJointIds = new List<JointId>();
        if (ikRequest.Target.Side == ArmSide.Right)
        {
            listJointIds.Add(new JointId { Name = "r_shoulder_pitch" });
            listJointIds.Add(new JointId { Name = "r_shoulder_roll" });
            listJointIds.Add(new JointId { Name = "r_arm_yaw" });
            listJointIds.Add(new JointId { Name = "r_elbow_pitch" });
            listJointIds.Add(new JointId { Name = "r_forearm_yaw" });
            listJointIds.Add(new JointId { Name = "r_wrist_pitch" });
            listJointIds.Add(new JointId { Name = "r_wrist_roll" });
        }
        else
        {
            listJointIds.Add(new JointId { Name = "l_shoulder_pitch" });
            listJointIds.Add(new JointId { Name = "l_shoulder_roll" });
            listJointIds.Add(new JointId { Name = "l_arm_yaw" });
            listJointIds.Add(new JointId { Name = "l_elbow_pitch" });
            listJointIds.Add(new JointId { Name = "l_forearm_yaw" });
            listJointIds.Add(new JointId { Name = "l_wrist_pitch" });
            listJointIds.Add(new JointId { Name = "l_wrist_roll" });
        }

        sol = new ArmIKSolution
        {
            Success = true,
            ArmPosition = new ArmJointPosition
            {
                Side = ikRequest.Target.Side,
                Positions = new JointPosition
                {
                    Ids = { listJointIds },
                    Positions = { listq },
                },
            },
        };

        return sol;
    }
}
