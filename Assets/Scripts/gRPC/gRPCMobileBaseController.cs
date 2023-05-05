using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Grpc.Core;
using System.Threading.Tasks;
using System;

using Reachy.Sdk.Mobility;
using Reachy.Sdk.Config;

namespace TeleopReachy
{
    public class gRPCMobileBaseController : gRPCBase
    {
        public UnityEvent<bool> event_OnMobileRoomStatusHasChanged;
        public UnityEvent event_OnMobileBaseDetected;
        public UnityEvent<float> event_OnMobileBaseBatteryLevelUpdate;

        private MobilityService.MobilityServiceClient clientMobility;

        private bool needUpdateCommandMobileBase;

        private bool needCreateChannel = false;
        private bool needUpdateBatteryLevel = false;


        void Start()
        {
            InitChannel("server_data_port");

            needUpdateCommandMobileBase = false;

            if (channel != null)
            {
                gRPCManager.Instance.gRPCRobotParams.event_OnRobotGenerationReceived.AddListener(AskForMobileBasePresence);
            }
        }

        protected override void RecoverFromNetWorkIssue()
        {
            AskForMobileBasePresence();
        }

        public void AskForMobilityReset()
        {
            RecoverFromNetWorkIssue();
        }

        protected override void NotifyDisconnection()
        {
            if (needCreateChannel)
            {
                Debug.LogWarning("GRPC DataController disconnected");
                isRobotInRoom = false;
                event_OnMobileRoomStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        private void AskForMobileBasePresence()
        {
            switch (gRPCManager.Instance.gRPCRobotParams.RobotGeneration)
            {
                case RobotGenerationCode.V2021:
                    {
                        Task.Run(() => GetMobileBasePresence_2021());
                        break;
                    }
                case RobotGenerationCode.V2023:
                    {
                        Task.Run(() => GetMobileBasePresence_2023());
                        break;
                    }
                case RobotGenerationCode.UNDEFINED:
                    {
                        Debug.LogWarning("Robot version not defined for mobile base");
                        break;
                    }
            }
        }

        private void NotifyMobileBasePresence(bool mobileBaseIsPresent)
        {
            if (mobileBaseIsPresent)
            {
                event_OnMobileBaseDetected.Invoke();
                needCreateChannel = true;
            }
        }

        private void GetMobileBasePresence_2021()
        {
            try
            {
                var clientMobilityPresence = new MobileBasePresenceService.MobileBasePresenceServiceClient(channel);
                bool mobileBaseIsPresent = clientMobilityPresence.GetMobileBasePresence(new Google.Protobuf.WellKnownTypes.Empty()).Presence.Value;
                NotifyMobileBasePresence(mobileBaseIsPresent);
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed: " + e);
                rpcException = "Error in GetMobileBasePresence():\n" + e.ToString();
            }
        }

        private void GetMobileBasePresence_2023()
        {
            try
            {
                var clientMobilityPresence = new ConfigService.ConfigServiceClient(channel);
                bool mobileBaseIsPresent = clientMobilityPresence.GetReachyConfig(new Google.Protobuf.WellKnownTypes.Empty()).MobileBasePresence;
                NotifyMobileBasePresence(mobileBaseIsPresent);
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed: " + e);
                rpcException = "Error in GetMobileBasePresence():\n" + e.ToString();
            }
        }

        private void Update()
        {
            if (needCreateChannel)
            {
                if(clientMobility == null)
                {
                    InitChannel("server_mobile_port");
                    clientMobility = new MobilityService.MobilityServiceClient(channel);
                    needUpdateBatteryLevel = true;
                }
                needCreateChannel = false;
            }
            if (needUpdateBatteryLevel)
            {
                needUpdateBatteryLevel = false;
                StartCoroutine(TemporizeBatteryRequest());
            }
        }

        public async void SendMobilityCommand(TargetDirectionCommand command)
        {
            try
            {
                if (needUpdateCommandMobileBase)
                {
                    needUpdateCommandMobileBase = false;
                    await clientMobility.SendDirectionAsync(command);
                    needUpdateCommandMobileBase = true;
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("GRPC failed: in SendMobilityCommand():" + e);
                rpcException = "Error in SendMobilityCommand():\n" + e.ToString();
                isRobotInRoom = false;
                event_OnMobileRoomStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        public async void SendZuuuMode(ZuuuModeCommand mode)
        {
            try
            {
                await clientMobility.SetZuuuModeAsync(mode);
            }
            catch (RpcException e)
            {
                Debug.LogWarning("GRPC failed: in SendZuuuMode():" + e);
                rpcException = "Error in SendZuuuMode():\n" + e.ToString();
                isRobotInRoom = false;
                needUpdateCommandMobileBase = false;
                event_OnMobileRoomStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        private void AskForBatteryLevel()
        {
            try
            {
                BatteryLevel batteryLevel = clientMobility.GetBatteryLevel(new Google.Protobuf.WellKnownTypes.Empty());
                event_OnMobileBaseBatteryLevelUpdate.Invoke(batteryLevel.Level.Value);
                if (!isRobotInRoom)
                {
                    isRobotInRoom = true;
                    needUpdateCommandMobileBase = true;
                    event_OnMobileRoomStatusHasChanged.Invoke(isRobotInRoom);
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("GRPC failed: in AskForBatteryLevel():" + e);
                rpcException = "Error in AskForBatteryLevel():\n" + e.ToString();
                isRobotInRoom = false;
                event_OnMobileRoomStatusHasChanged.Invoke(isRobotInRoom);
                needUpdateCommandMobileBase = false;
            }
        }

        IEnumerator TemporizeBatteryRequest()
        {
            Task.Run(() => AskForBatteryLevel());
            yield return new WaitForSeconds(10);
            needUpdateBatteryLevel = true;
        }
    }
}
