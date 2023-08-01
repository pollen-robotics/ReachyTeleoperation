using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TeleopReachy
{
    public class ConnectionStatus : MonoBehaviour
    {
        //private bool isServerConnected;
        private bool isRobotConfigReady;
        private bool isRobotInDataRoom;
        private bool isRobotInVideoRoom;
        private bool isRobotInMobileRoom;
        private bool isRobotInRestartRoom;
        private bool hasRobotJustLeftDataRoom;

        private bool isRobotReady;

        private bool areRobotServicesRestarting;

        private gRPCDataController dataController;
        // private gRPCVideoController videoController;
        private gRPCMobileBaseController mobileController;


        public WebRTCVideoReceiver webRTCVideoController;
        public UnityEvent event_OnConnectionStatusHasChanged;
        public UnityEvent event_OnRobotReady;

        public UnityEvent event_OnRobotUnready;

        private bool statusChanged;

        private RobotConfig robotConfig;

        private Coroutine waitForConnection;

        void Start()
        {
            dataController = gRPCManager.Instance.gRPCDataController;
            // videoController = gRPCManager.Instance.gRPCVideoController;
            mobileController = gRPCManager.Instance.gRPCMobileBaseController;

            robotConfig = RobotDataManager.Instance.RobotConfig;

            robotConfig.event_OnConfigChanged.AddListener(RobotConfigurationChanged);

            //isServerConnected = false;
            isRobotConfigReady = false;
            isRobotInDataRoom = false;
            isRobotInVideoRoom = true;
            isRobotInMobileRoom = false;
            isRobotInRestartRoom = false;

            isRobotReady = false;

            hasRobotJustLeftDataRoom = false;

            areRobotServicesRestarting = true;

            statusChanged = false;

            if (webRTCVideoController != null) webRTCVideoController.event_OnVideoRoomStatusHasChanged.AddListener(VideoControllerStatusHasChanged);
            if (dataController != null) dataController.event_DataControllerStatusHasChanged.AddListener(DataControllerStatusHasChanged);
            if (mobileController != null) mobileController.event_OnMobileRoomStatusHasChanged.AddListener(MobileControllerStatusHasChanged);

            waitForConnection = StartCoroutine(WaitForConnection());
        }

        public bool IsRobotInDataRoom()
        {
            return isRobotInDataRoom;
        }

        public bool HasRobotJustLeftDataRoom()
        {
            return hasRobotJustLeftDataRoom;
        }

        public bool IsRobotConfigReady()
        {
            return isRobotConfigReady;
        }

        public bool IsRobotInVideoRoom()
        {
            return isRobotInVideoRoom;
        }

        public bool IsRobotInRestartRoom()
        {
            return isRobotInRestartRoom;
        }

        public bool IsRobotInMobileRoom()
        {
            return isRobotInMobileRoom;
        }

        public bool IsRobotReady()
        {
            return isRobotReady;
        }

        public bool IsServerConnected()
        {
            //return isServerConnected;
            //Todo
            return true;
        }

        public bool AreRobotServicesRestarting()
        {
            return areRobotServicesRestarting;
        }

        public void SetRobotServicesRestarting(bool areRestarting)
        {
            areRobotServicesRestarting = areRestarting;
        }


        void VideoControllerStatusHasChanged(bool isRobotInRoom)
        {
            Debug.Log("[ConnectionStatus] VideoControllerStatusHasChanged");
            isRobotInVideoRoom = isRobotInRoom;
            statusChanged = true;
        }


        void DataControllerStatusHasChanged(bool isRobotInRoom)
        {
            Debug.Log("[ConnectionStatus] DataControllerStatusHasChanged");
            isRobotInDataRoom = isRobotInRoom;
            if (isRobotInDataRoom)
            {
                hasRobotJustLeftDataRoom = false;
            }
            else
            {
                hasRobotJustLeftDataRoom = true;
            }
            statusChanged = true;
        }

        void MobileControllerStatusHasChanged(bool isRobotInRoom)
        {
            isRobotInMobileRoom = isRobotInRoom;
            statusChanged = true;
        }

        /*void RestartServiceStatusHasChanged(object sender, RobotRoomStatusEventArgs e)
        {
            isServerConnected = RobotRoomStatusEventArgs.isServerConnected;
            isRobotInRestartRoom = e.isRobotInRoom;
            statusChanged = true;
        }*/

        /*void RestartServiceRequestedByUser(object sender, EventArgs e)
        {
            isRobotInDataRoom = false;
            isRobotInVideoRoom = false;
            statusChanged = true;
        }*/

        void RobotConfigurationChanged()
        {
            Debug.Log("[ConnectionStatus] Config Changed");
            isRobotConfigReady = robotConfig.GotReachyConfig();
            statusChanged = true;
        }

        void Update()
        {
            if (statusChanged)
            {
                statusChanged = false;
                if (isRobotInDataRoom && isRobotConfigReady && ((robotConfig.HasHead() && isRobotInVideoRoom) || !robotConfig.HasHead()))
                {
                    if (!isRobotReady)
                    {
                        isRobotReady = true;
                        if (waitForConnection != null) StopCoroutine(waitForConnection);
                        areRobotServicesRestarting = false;
                        event_OnRobotReady.Invoke();
                    }
                }
                else
                {
                    isRobotReady = false;
                    event_OnRobotUnready.Invoke();
                }
                if (!isRobotInDataRoom)
                {
                    hasRobotJustLeftDataRoom = false;
                }
                event_OnConnectionStatusHasChanged.Invoke();
            }
        }

        IEnumerator WaitForConnection()
        {
            yield return new WaitForSeconds(5);
            areRobotServicesRestarting = false;
            statusChanged = true;
        }
    }
}