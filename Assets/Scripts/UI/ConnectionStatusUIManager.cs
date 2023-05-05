using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace TeleopReachy
{
    public class ConnectionStatusUIManager : MonoBehaviour
    {
        [SerializeField]
        private Text connectionStatusHelpText;

        [SerializeField]
        private Text connectionStatusLabel;

        [SerializeField]
        private Image connectionStatusImage;

        private ConnectionStatus connectionStatus;
        private RobotConfig robotConfig;

        private Color32 connectionStatusColor;
        private string connectionStatusText;
        private string connectionStatusHelp;

        void Awake()
        {
            if (Robot.IsCurrentRobotVirtual())
            {
                connectionStatusText = "Using virtual Reachy";
                connectionStatusColor = ColorsManager.green;
                UpdateConnectionStatus();
            }
            else
            {

                connectionStatus = gRPCManager.Instance.ConnectionStatus;
                //connectionStatus.OnConnectionStatusHasChanged += CheckConnectionStatus;
                connectionStatus.event_OnConnectionStatusHasChanged.AddListener(CheckConnectionStatus);

                connectionStatusText = "Trying to connect to server...";
                connectionStatusColor = ColorsManager.blue;
                connectionStatusHelp = "Looking for the server...\n" +
                    "Please wait.\n";

                robotConfig = RobotDataManager.Instance.RobotConfig;

                CheckConnectionStatus();
            }
        }

        private void UpdateConnectionStatus()
        {
            if (connectionStatusImage != null) connectionStatusImage.color = connectionStatusColor;
            if (connectionStatusLabel != null)
            {
                connectionStatusLabel.text = connectionStatusText;
                connectionStatusLabel.color = connectionStatusColor;
            }

            if (connectionStatusHelpText != null)
            {
                connectionStatusHelpText.text = connectionStatusHelp;
            }
        }

        private void CheckConnectionStatus()
        {
            Debug.Log("[ConnectionStatusUIManager]: CheckConnectionStatus " + transform.parent);
            if (connectionStatus.IsServerConnected())
            {
                Debug.Log("[ConnectionStatusUIManager]: isRobotInDataRoom: " + connectionStatus.IsRobotInDataRoom());
                Debug.Log("[ConnectionStatusUIManager]: IsRobotInVideoRoom: " + connectionStatus.IsRobotInVideoRoom());
                if (connectionStatus.IsRobotInDataRoom() && connectionStatus.IsRobotInVideoRoom())
                {
                    if (connectionStatus.IsRobotInRestartRoom())
                    {
                        connectionStatusText = "Connected to Reachy";
                        connectionStatusColor = ColorsManager.green;
                        connectionStatusHelp = "Everything seems to work fine. Enjoy!";
                    }
                    else
                    {
                        connectionStatusText = "Connected to Reachy. No restart available";
                        connectionStatusColor = ColorsManager.yellow;
                        connectionStatusHelp = "Restart service is not available, but you can still teleoperate the robot. Enjoy!";
                    }
                    if (robotConfig.HasMobilePlatform() && !connectionStatus.IsRobotInMobileRoom())
                    {
                        connectionStatusText = "Connected to Reachy. Mobile base unavailable";
                        connectionStatusColor = ColorsManager.purple;
                        connectionStatusHelp = "Mobile base services are not available, but you can still teleoperate the robot. Enjoy!";
                    }
                }
                else
                {
                    if (connectionStatus.AreRobotServicesRestarting())
                    {
                        connectionStatusText = "Trying to connect...";
                        connectionStatusColor = ColorsManager.blue;
                        connectionStatusHelp = "Restarting the robot services...\n" +
                            "Please wait.\n";
                    }
                    else
                    {
                        connectionStatusText = "Robot connection failed";
                        connectionStatusColor = ColorsManager.orange;
                        connectionStatusHelp = "Some required robot services are not available.\n" +
                            "Teleoperation is not possible.\n";
                    }
                }
            }
            else
            {
                connectionStatusText = "Unable to connect to remote server";
                connectionStatusColor = ColorsManager.red;
                connectionStatusHelp = "The server is not responding, or you may not be connected to the right address.\n";
            }
            UpdateConnectionStatus();
        }
    }
}