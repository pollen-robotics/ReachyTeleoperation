using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using Grpc.Core;
using System;

using Reachy.Sdk.Config;

namespace TeleopReachy
{
    public enum RobotGenerationCode
    {
        UNDEFINED, V2021, V2023
    }


    public class gRPCRobotParams : gRPCBase
    {
        private ConfigService.ConfigServiceClient client;

        public UnityEvent event_OnRobotGenerationReceived;

        public RobotGenerationCode RobotGeneration { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            InitChannel("server_data_port");
            if (channel != null)
            {
                client = new ConfigService.ConfigServiceClient(channel);
                Task.Run(() => GetRobotGeneration());
            }
        }

        private void GetRobotGeneration()
        {
            try
            {
                RobotGenerationCode robotGeneration;
                Enum.TryParse<RobotGenerationCode>("V" + client.GetReachyConfig(new Google.Protobuf.WellKnownTypes.Empty()).Generation.ToString(), out robotGeneration);
                RobotGeneration = robotGeneration;
                event_OnRobotGenerationReceived.Invoke();
            }
            catch (RpcException e)
            {
                Debug.Log("Robot Generation is 2021. " + e);
                RobotGeneration = RobotGenerationCode.V2021;
                event_OnRobotGenerationReceived.Invoke();
            }
        }
    }
}
