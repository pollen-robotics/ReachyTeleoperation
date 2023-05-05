using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Grpc.Core;
using System.Threading.Tasks;

using Reachy.Sdk.Joint;
using Reachy.Sdk.Kinematics;


namespace TeleopReachy
{
    public class gRPCDataController : gRPCBase
    {
        private JointService.JointServiceClient client = null;
        private FullBodyCartesianCommandService.FullBodyCartesianCommandServiceClient clientCartesian = null;

        private bool needUpdateCommandBody;
        private bool needUpdateCommandGripper;
        private bool needUpdateState;

        private JointsId allJointsId;

        public UnityEvent<bool> event_DataControllerStatusHasChanged;

        public UnityEvent<JointsId> event_OnRobotJointsReceived;

        public UnityEvent<Dictionary<JointId, float>> event_OnStateUpdateTemperature;

        public UnityEvent<Dictionary<JointId, float>> event_OnStateUpdatePresentPositions;

        void Start()
        {
            needUpdateCommandBody = false;
            needUpdateCommandGripper = false;
            needUpdateState = false;

            InitChannel("server_data_port");
            if (channel != null)
            {
                client = new JointService.JointServiceClient(channel);
                clientCartesian = new FullBodyCartesianCommandService.FullBodyCartesianCommandServiceClient(channel);
                Task.Run(() => GetJointsId());
            }
        }

        protected override void RecoverFromNetWorkIssue()
        {
            Task.Run(() => GetJointsId());
            gRPCManager.Instance.gRPCMobileBaseController.AskForMobilityReset();
        }

        protected override void NotifyDisconnection()
        {
            Debug.LogWarning("GRPC DataController disconnected");
            isRobotInRoom = false;
            event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
        }

        private void GetJointsId()
        {
            try
            {
                Debug.Log(client);
                allJointsId = client.GetAllJointsId(new Google.Protobuf.WellKnownTypes.Empty());
                event_OnRobotJointsReceived.Invoke(allJointsId);
                isRobotInRoom = true;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
                needUpdateCommandBody = true;
                needUpdateCommandGripper = true;
                needUpdateState = true;
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed: " + e);
                rpcException = "Error in GetJointsId():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }


        public async void SendJointsCommand(JointsCommand jointsCommand)
        {
            try
            {
                await client.SendJointsCommandsAsync(jointsCommand);
            }
            catch (RpcException e)
            {
                Debug.LogWarning("Communication RPC failed: in SendJointsPositions():" + e);
                rpcException = "Error in SendJointsPositions():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        public async void SendGrippersCommand(JointsCommand jointsCommand)
        {
            try
            {
                if (needUpdateCommandGripper)
                {
                    needUpdateCommandGripper = false;
                    await client.SendJointsCommandsAsync(jointsCommand);
                    needUpdateCommandGripper = true;
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("Communication RPC failed: in SendJointsPositions():" + e);
                rpcException = "Error in SendJointsPositions():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        public async void SendBodyCommand(FullBodyCartesianCommand command)
        {
            try
            {
                if (needUpdateCommandBody)
                {
                    needUpdateCommandBody = false;
                    await clientCartesian.SendFullBodyCartesianCommandsAsync(command);
                    needUpdateCommandBody = true;
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("GRPC failed: in SendBodyCommand():" + e);
                rpcException = "Error in SendBodyCommand():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        public async void SendImmediateBodyCommand(FullBodyCartesianCommand command)
        {
            try
            {
                await clientCartesian.SendFullBodyCartesianCommandsAsync(command);
            }
            catch (RpcException e)
            {
                Debug.LogWarning("GRPC failed: in SendImmediateBodyCommand():" + e);
                rpcException = "Error in SendImmediateBodyCommand():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        public async void GetJointsState()
        {
            try
            {
                if (needUpdateState)
                {
                    needUpdateState = false;

                    List<JointId> ids = new List<JointId>();
                    foreach (var item in allJointsId.Names)
                    {
                        var joint = new JointId();
                        joint.Name = item;

                        ids.Add(joint);
                    };

                    JointsStateRequest jointsRequest = new JointsStateRequest
                    {
                        Ids = { ids },
                        RequestedFields = { JointField.Name, JointField.PresentPosition, JointField.GoalPosition, JointField.Temperature },
                    };

                    var reply = await client.GetJointsStateAsync(jointsRequest);

                    Dictionary<JointId, float> present_positions = new Dictionary<JointId, float>();
                    //Dictionary<JointId, float> goal_positions = new Dictionary<JointId, float>();
                    Dictionary<JointId, float> temperatures = new Dictionary<JointId, float>();

                    for (int i = 0; i < reply.States.Count; i++)
                    {
                        float command = Mathf.Rad2Deg * (float)reply.States[i].PresentPosition;
                        present_positions.Add(reply.Ids[i], command);
                        //float expectedcommand = Mathf.Rad2Deg * (float)reply.States[i].GoalPosition;
                        //goal_positions.Add(reply.Ids[i], expectedcommand);
                        float temperature = (float)reply.States[i].Temperature;
                        temperatures.Add(reply.Ids[i], temperature);
                    }
                    //OnJointsStateReceivedEvent(new StateUpdateEventArgs(present_positions, goal_positions, temperatures));
                    event_OnStateUpdateTemperature.Invoke(temperatures);
                    event_OnStateUpdatePresentPositions.Invoke(present_positions);
                    needUpdateState = true;
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed: " + e);
                rpcException = "Error in GetJointsState():\n" + e.ToString();
                isRobotInRoom = false;
                event_DataControllerStatusHasChanged.Invoke(isRobotInRoom);
            }
        }
    }
}