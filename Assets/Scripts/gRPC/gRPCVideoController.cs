using UnityEngine;
using UnityEngine.Events;
using Grpc.Core;
using Reachy.Sdk.Camera;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace TeleopReachy
{
    public class gRPCVideoController : gRPCBase
    {
        private CameraService.CameraServiceClient client = null;

        private Texture2D leftTexture;
        private Texture2D rightTexture;

        [SerializeField]
        private Material leftEyeTexture;

        [SerializeField]
        private Texture defaultTexture;

        private bool firstConnection;

        private bool needUpdateEyeImage;

        private RobotConfig robotConfig;
        public UnityEvent<bool> event_OnVideoRoomStatusHasChanged;

        private float previous_time = -1;
        Queue<float> previous_elapsed;
        private const int QUEUE_SIZE = 30;

        private float mean_fps = 0;

        // Start is called before the first frame update
        void Awake()
        {
            firstConnection = true;

            needUpdateEyeImage = false;

            previous_elapsed = new Queue<float>(QUEUE_SIZE);

            leftTexture = new Texture2D(2, 2);
            rightTexture = new Texture2D(2, 2);

            SetDefaultOverlayTexture();
        }

        void Start()
        {
            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotConfig.event_OnConfigChanged.AddListener(CheckConfig);
            InitChannel("server_camera_port");
            if (channel != null)
            {
                client = new CameraService.CameraServiceClient(channel);
            }
        }

        void CheckConfig()
        {
            if (firstConnection && robotConfig.HasHead() && client != null)
            {
                firstConnection = false;
                Task.Run(() => TryGetImage());
            }
        }

        async void TryGetImage()
        {
            try
            {
                await client.GetImageAsync(new ImageRequest { Camera = new Reachy.Sdk.Camera.Camera { Id = CameraId.Left }, });
                isRobotInRoom = true;
                event_OnVideoRoomStatusHasChanged.Invoke(isRobotInRoom);
                needUpdateEyeImage = true;
                previous_time = -1;
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed: " + e);
                isRobotInRoom = false;
                event_OnVideoRoomStatusHasChanged.Invoke(isRobotInRoom);
            }
        }

        protected override void RecoverFromNetWorkIssue()
        {
            Task.Run(() => TryGetImage());
        }

        protected override void NotifyDisconnection()
        {
            Debug.LogWarning("GRPC VideoController disconnected");
            isRobotInRoom = false;
            event_OnVideoRoomStatusHasChanged.Invoke(isRobotInRoom);
        }

        public async void GetImage(CameraId side)
        {

            try
            {
                if (needUpdateEyeImage)
                {
                    needUpdateEyeImage = false;
                    var reply = await client.GetImageAsync(new ImageRequest { Camera = new Reachy.Sdk.Camera.Camera { Id = side }, });
                    byte[] imageBytes = reply.Data.ToByteArray();

                    leftTexture.LoadImage(imageBytes);
                    rightTexture.LoadImage(imageBytes);

                    leftEyeTexture.SetTexture("_MainTex", leftTexture);
                    leftEyeTexture.SetTexture("_MainTexRight", rightTexture);

                    ComputeMeanFPS();
                    needUpdateEyeImage = true;
                }
            }
            catch (RpcException e)
            {
                Debug.LogWarning("RPC failed in GetImage : " + e);
                isRobotInRoom = false;
                event_OnVideoRoomStatusHasChanged.Invoke(isRobotInRoom);
                SetDefaultOverlayTexture();
            }
            catch (ArgumentNullException e)
            {
                //reply can be null when app is closing
                Debug.LogWarning("Null exception : " + e);
            }
        }

        public void SetDefaultOverlayTexture()
        {
            if (leftEyeTexture.mainTexture != defaultTexture)
            {
                leftEyeTexture.mainTexture = defaultTexture;
                leftEyeTexture.SetTexture("_MainTexRight", defaultTexture);
            }
        }

        private void ComputeMeanFPS()
        {
            float current_time = Time.time;
            float elapsed_time = current_time - previous_time;

            if (previous_time != -1)
            {
                previous_elapsed.Enqueue(elapsed_time);
                if (previous_elapsed.Count > QUEUE_SIZE)
                    previous_elapsed.Dequeue();
                float mean = 0;
                foreach (float obj in previous_elapsed)
                {
                    mean += obj;
                }
                mean /= previous_elapsed.Count;
                mean_fps = 1 / mean;
            }
            previous_time = current_time;
        }

        public float GetMeanFPS()
        {
            if (ip_address != Robot.VIRTUAL_ROBOT_IP)
            {
                if (previous_elapsed.Count > 1) return mean_fps;
                else return -1;
            }
            else return 30;
        }
    }
}