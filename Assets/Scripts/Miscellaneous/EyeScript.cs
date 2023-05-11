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
    public class EyeScript : MonoBehaviour
    {

        private gRPCRobotParams robotParams;
        private RobotStatus robotStatus;
        private Channel channel;
        private ConfigService.ConfigServiceClient configService;

        private bool needCameraParameterUpdate = false;
        private bool needColorUpdate = false;

        Renderer rend;

        float l_fx = 0;
        float l_fy = 0;
        float l_cx = 0;
        float l_cy = 0;
        float l_k1 = 0;
        float l_k2 = 0;
        float l_k3 = 0;
        float l_p1 = 0;
        float l_p2 = 0;

        float r_fx = 0;
        float r_fy = 0;
        float r_cx = 0;
        float r_cy = 0;
        float r_k1 = 0;
        float r_k2 = 0;
        float r_k3 = 0;
        float r_p1 = 0;
        float r_p2 = 0;

        public float rightTexOffsetX = 0;
        public float rightTexOffsetY = 0;

        float alpha = 1.0f;

        void Start()
        {        
            gRPCManager.Instance.gRPCRobotParams.event_OnRobotGenerationReceived.AddListener(setCameraParams);
            robotStatus = RobotDataManager.Instance.RobotStatus;
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

        public void switchVisionMode(bool mode2d)
        {
            if (mode2d)
            {
                rightTexOffsetX = 0f;
                rightTexOffsetY = 0f;
            }
            else
            {
                rightTexOffsetX = 1.03f;
                rightTexOffsetY = -0.08f;

            }
            needCameraParameterUpdate = true;
            Update();
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

        void Update()
        {
            if (needCameraParameterUpdate)
            {

                string ip_address = PlayerPrefs.GetString("robot_ip");
                string addressData = ip_address + ":" + PlayerPrefs.GetString("server_data_port");
                Channel channelData = new Channel(addressData, ChannelCredentials.Insecure);
                configService = new ConfigService.ConfigServiceClient(channelData);

                
                float[] camera_parameters = new float[18];
                var i = 0;
                foreach (var param in configService.GetReachyConfig(new Google.Protobuf.WellKnownTypes.Empty()).CameraParameters)
                {

                    camera_parameters[i] = param;
                    i ++;
                }

                l_cx = camera_parameters[0];
                l_cy = camera_parameters[1];
                l_fx = camera_parameters[2];
                l_fy = camera_parameters[3];
                l_k1 = camera_parameters[4];
                l_k2 = camera_parameters[5];
                l_k3 = camera_parameters[6];
                l_p1 = camera_parameters[7];
                l_p2 = camera_parameters[8];

                r_cx = camera_parameters[9];
                r_cy = camera_parameters[10];
                r_fx = camera_parameters[11];
                r_fy = camera_parameters[12];
                r_k1 = camera_parameters[13];
                r_k2 = camera_parameters[14];
                r_k3 = camera_parameters[15];
                r_p1 = camera_parameters[17];
                r_p2 = camera_parameters[17];

                rend = GetComponent<Renderer> ();
                rend.material.SetFloat("_l_fx", l_fx);
                rend.material.SetFloat("_l_fy", l_fy);
                rend.material.SetFloat("_l_cx", l_cx);
                rend.material.SetFloat("_l_cy", l_cy);
                rend.material.SetFloat("_l_k1", l_k1);
                rend.material.SetFloat("_l_k2", l_k2);
                rend.material.SetFloat("_l_k3", l_k3);
                rend.material.SetFloat("_l_p1", l_p1);
                rend.material.SetFloat("_l_p2", l_p2);

                rend.material.SetFloat("_r_fx", r_fx);
                rend.material.SetFloat("_r_fy", r_fy);
                rend.material.SetFloat("_r_cx", r_cx);
                rend.material.SetFloat("_r_cy", r_cy);
                rend.material.SetFloat("_r_k1", r_k1);
                rend.material.SetFloat("_r_k2", r_k2);
                rend.material.SetFloat("_r_k3", r_k3);
                rend.material.SetFloat("_r_p1", r_p1);
                rend.material.SetFloat("_r_p2", r_p2);

                rend.material.SetFloat("_rightTexOffsetX", rightTexOffsetX);
                rend.material.SetFloat("_rightTexOffsetY", rightTexOffsetY);

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