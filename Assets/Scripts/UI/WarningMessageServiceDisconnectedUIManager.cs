using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace TeleopReachy
{
    public class WarningMessageServiceDisconnectedUIManager : MonoBehaviour
    {
        private ConnectionStatus connectionStatus;
        private RobotStatus robotStatus;
        private RobotConfig robotConfig;

        [SerializeField]
        private Text warningMessage;

        private string messageToDisplay;

        private bool needUpdateWarningMessage;
        private bool wantWarningMessageDisplayed;

        private bool onlyMobileServicesAffected;
        private Coroutine limitDisplayInTime;

        private UserMobilityInput userMobilityInput;

        void Start()
        {
            connectionStatus = gRPCManager.Instance.ConnectionStatus;
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotConfig = RobotDataManager.Instance.RobotConfig;

            EventManager.StartListening(EventNames.TeleoperationSceneLoaded, ListenToUserMobilityInput);

            connectionStatus.event_OnConnectionStatusHasChanged.AddListener(CheckNewStatus);
            connectionStatus.event_OnRobotReady.AddListener(HideWarningMessage);
            connectionStatus.event_OnRobotUnready.AddListener(ShowWarningMessage);
            robotStatus.event_OnStopTeleoperation.AddListener(HideWarningMessage);

            needUpdateWarningMessage = false;
            wantWarningMessageDisplayed = false;

            transform.ActivateChildren(false);
        }

        void ListenToUserMobilityInput()
        {
            userMobilityInput = UserInputManager.Instance.UserMobilityInput;
            userMobilityInput.event_OnTriedToSendCommands.AddListener(DisplayMessageForMobility);
        }


        void CheckNewStatus()
        {
            onlyMobileServicesAffected = false;
            if(!connectionStatus.IsRobotInDataRoom())
            {
                if(!connectionStatus.IsRobotInVideoRoom())
                {
                    messageToDisplay = "Robot services have been disconnected";
                }
                else
                {
                    messageToDisplay = "Motor control has been disconnected";
                }
            }
            else
            {
                if(!connectionStatus.IsRobotInVideoRoom())
                {
                    messageToDisplay = "Video stream has been disconnected";
                }
                else
                {
                    if(robotConfig.HasMobilePlatform() && !connectionStatus.IsRobotInMobileRoom())
                    {
                        onlyMobileServicesAffected = true;
                        messageToDisplay = "Mobile services have been disconnected";
                    }
                    else{
                        wantWarningMessageDisplayed = false;
                    }
                }
            }
            needUpdateWarningMessage = true;
        }

        void DisplayMessageForMobility()
        {
            if(!wantWarningMessageDisplayed)
            {
                onlyMobileServicesAffected = true;
                if(robotConfig.HasMobilePlatform())
                {
                    if(!robotStatus.IsMobilityActive())
                    {
                        messageToDisplay = "Mobile services have been disconnected";
                    }
                    else
                    {
                        if(!robotStatus.IsMobilityOn()) messageToDisplay = "Mobile base has been disabled in options";
                    }
                    ShowWarningMessage();
                }            
            }
        }

        void ShowWarningMessage()
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                wantWarningMessageDisplayed = true;
                needUpdateWarningMessage = true;
            }
        }

        void Update()
        {
            if(needUpdateWarningMessage)
            {
                warningMessage.text = messageToDisplay;
                if(onlyMobileServicesAffected)
                {
                    if(wantWarningMessageDisplayed) 
                    {
                        if(limitDisplayInTime != null) StopCoroutine(limitDisplayInTime);
                        limitDisplayInTime = StartCoroutine(DisplayLimitedInTime());
                    }
                }
                else
                {
                    if(limitDisplayInTime != null) StopCoroutine(limitDisplayInTime);
                    if(wantWarningMessageDisplayed) transform.ActivateChildren(true);
                    else { transform.ActivateChildren(false); }
                }
                needUpdateWarningMessage = false;
            }
        }

        IEnumerator DisplayLimitedInTime()
        {
            transform.ActivateChildren(true);
            yield return new WaitForSeconds(3);
            transform.ActivateChildren(false);
            wantWarningMessageDisplayed = false;
        }

        void HideWarningMessage()
        {
            wantWarningMessageDisplayed = false;
            needUpdateWarningMessage = true;
        }
    }
}