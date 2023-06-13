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
using System.Globalization;

namespace TeleopReachy
{
    public class EyeScript : MonoBehaviour
    {

        private gRPCRobotParams robotParams;
        private RobotStatus robotStatus;
        private Channel channel;
        private ConfigService.ConfigServiceClient configService;

        public Texture2D l_mapX;
        public Texture2D l_mapY;
        
        public Texture2D r_mapX;
        public Texture2D r_mapY;

        private bool needCameraParameterUpdate = false;
        private bool needColorUpdate = false;

        // get leftEyeTexture instance from grpcvideocontroller
        public Material leftEyeTexture;


        float alpha = 1.0f;

        void Start()
        {        
            
            leftEyeTexture = gRPCManager.Instance.gRPCVideoController.leftEyeTexture;

            l_mapX = new Texture2D(480, 640, TextureFormat.RGBAFloat, false);
            l_mapY = new Texture2D(480, 640, TextureFormat.RGBAFloat, false);
            r_mapX = new Texture2D(480, 640, TextureFormat.RGBAFloat, false);
            r_mapY = new Texture2D(480, 640, TextureFormat.RGBAFloat, false);

            TextAsset l_mapx_txt = Resources.Load<TextAsset>("L_mapx");
            TextAsset l_mapy_txt = Resources.Load<TextAsset>("L_mapy");
            TextAsset r_mapx_txt = Resources.Load<TextAsset>("R_mapx");
            TextAsset r_mapy_txt = Resources.Load<TextAsset>("R_mapy");

            var dataLines = l_mapx_txt.text.Split('\n');
            for(int j = 1; j < dataLines.Length; j++) {
                var data = dataLines[j].Split(',');
                for(int i = 1; i < data.Length; i++) {
                    string str_val = data[i];
                    float float_val = float.Parse(str_val.ToString(), CultureInfo.InvariantCulture);
                    Color c = new Color(float_val/data.Length, 0f, 0f);
                    l_mapX.SetPixel(i-1, j-1, c);
                }
            }

            dataLines = l_mapy_txt.text.Split('\n');
            for(int j = 1; j < dataLines.Length; j++) {
                var data = dataLines[j].Split(',');
                for(int i = 1; i < data.Length; i++) {
                    string str_val = data[i];
                    float float_val = float.Parse(str_val.ToString(), CultureInfo.InvariantCulture);
                    Color c = new Color(float_val/dataLines.Length, 0f, 0f);
                    l_mapY.SetPixel(i-1, j-1, c);
                }
            }

            dataLines = r_mapx_txt.text.Split('\n');
            for(int j = 1; j < dataLines.Length; j++) {
                var data = dataLines[j].Split(',');
                for(int i = 1; i < data.Length; i++) {
                    string str_val = data[i];
                    float float_val = float.Parse(str_val.ToString(), CultureInfo.InvariantCulture);
                    Color c = new Color(float_val/dataLines.Length, 0f, 0f);
                    r_mapX.SetPixel(i-1, j-1, c);
                }
            }

            dataLines = r_mapy_txt.text.Split('\n');
            for(int j = 1; j < dataLines.Length; j++) {
                var data = dataLines[j].Split(',');
                for(int i = 1; i < data.Length; i++) {
                    string str_val = data[i];
                    float float_val = float.Parse(str_val.ToString(), CultureInfo.InvariantCulture);
                    Color c = new Color(float_val/dataLines.Length, 0f, 0f);
                    r_mapY.SetPixel(i-1, j-1, c);
                }
            }
            
            l_mapX.Apply();
            l_mapY.Apply();
            r_mapX.Apply();
            r_mapY.Apply();

            gRPCManager.Instance.gRPCRobotParams.event_OnRobotGenerationReceived.AddListener(setCameraParams);
            robotStatus = RobotDataManager.Instance.RobotStatus;

            needCameraParameterUpdate = true;
            Update();
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
                // Ca fait buger le truc ?
                leftEyeTexture.SetTexture("_l_MapX", l_mapX);
                leftEyeTexture.SetTexture("_l_MapY", l_mapY);
                leftEyeTexture.SetTexture("_r_MapX", r_mapX);
                leftEyeTexture.SetTexture("_r_MapY", r_mapY);
                needCameraParameterUpdate = false;
            }

            if (needColorUpdate)
            {
                Color color = new Color(1, 1, 1, alpha);
                leftEyeTexture.SetColor("_Color", color);
                needColorUpdate = false;
            }
        }
    }
}