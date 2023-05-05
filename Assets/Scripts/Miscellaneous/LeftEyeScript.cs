using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Grpc.Core;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Reachy.Sdk.Mobility;
using Reachy.Sdk.Config;
namespace TeleopReachy
{
    public class LeftEyeScript : MonoBehaviour
    {

        private gRPCRobotParams robotParams;
        private Channel channel;
        private ConfigService.ConfigServiceClient configService;

        private bool needCameraParameterUpdate = false;
        private bool needColorUpdate = false;

        Renderer rend;

        float fx = 0;
        float fy = 0;
        float cx = 0;
        float cy = 0;
        float k1 = 0;
        float k2 = 0;
        float k3 = 0;
        float p1 = 0;
        float p2 = 0;

        float alpha = 1.0f;

        // Start is called before the first frame update
        void Start()
        {        
            gRPCManager.Instance.gRPCRobotParams.event_OnRobotGenerationReceived.AddListener(setCameraParams);
        }

        private void setCameraParams()
        {
            
            switch (gRPCManager.Instance.gRPCRobotParams.RobotGeneration)
            {
                default:
                    break;   
                case (RobotGenerationCode.V2023):
                {
                    needCameraParameterUpdate = true;    
                    break;
                }
            }
        }

        public void SetImageTransparent()
        {
            alpha = 0.5f;
            needColorUpdate = true;
        }

        public void SetImageOpaque()
        {
            alpha = 1.0f;
            needColorUpdate = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (needCameraParameterUpdate)
            {

                string ip_address = PlayerPrefs.GetString("robot_ip");
                string addressData = ip_address + ":" + PlayerPrefs.GetString("server_data_port");
                Channel channelData = new Channel(addressData, ChannelCredentials.Insecure);
                configService = new ConfigService.ConfigServiceClient(channelData);

                
                float[] camera_parameters = new float[9];
                var i = 0;
                foreach (var param in configService.GetReachyConfig(new Google.Protobuf.WellKnownTypes.Empty()).CameraParameters)
                {
                    if (i < 9)
                    {
                        camera_parameters[i] = param;
                    }
                    i ++;
                }

                cx = camera_parameters[0];
                cy = camera_parameters[1];
                fx = camera_parameters[2];
                fy = camera_parameters[3];
                k1 = camera_parameters[4];
                k2 = camera_parameters[5];
                k3 = camera_parameters[6];
                p1 = camera_parameters[7];
                p2 = camera_parameters[8];

                rend = GetComponent<Renderer> ();
                rend.material.SetFloat("_fx", fx);
                rend.material.SetFloat("_fy", fy);
                rend.material.SetFloat("_cx", cx);
                rend.material.SetFloat("_cy", cy);
                rend.material.SetFloat("_k1", k1);
                rend.material.SetFloat("_k2", k2);
                rend.material.SetFloat("_k3", k3);
                rend.material.SetFloat("_p1", p1);
                rend.material.SetFloat("_p2", p2);

                needCameraParameterUpdate = false;
            }

            if (needColorUpdate)
            {
                rend = GetComponent<Renderer> ();
                Color color = new Color(1, 1, 1, alpha);
                rend.material.SetColor("_Color", color);
                needColorUpdate = false;
            }
        }
    }
}