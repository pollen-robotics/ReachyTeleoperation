using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;


namespace TeleopReachy
{
    public class UserMobilityInput : MonoBehaviour
    {
        public ControllersManager controllers;

        private RobotMobilityCommands mobilityCommands;
        private RobotStatus robotStatus;

        private Vector2 mobileBaseTranslation;
        private Vector2 mobileBaseRotation;

        private Vector2 direction;

        private bool mobilityInputsSuspended;
        private bool mobilityInputsDisableTry;

        private const float maxSpeedFactor = 0.5f;

        public UnityEvent event_OnTriedToSendCommands;
        public UnityEvent<bool> event_DejaVu;

        private void OnEnable()
        {
            EventManager.StartListening(EventNames.TeleoperationSceneLoaded, Init);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventNames.TeleoperationSceneLoaded, Init);
        }

        private void Init()
        {
            mobilityCommands = RobotDataManager.Instance.RobotMobilityCommands;
            robotStatus = RobotDataManager.Instance.RobotStatus;
        }

        void Awake()
        {
            controllers = ActiveControllerManager.Instance.ControllersManager;
        }

        void Start()
        {
            mobilityInputsSuspended = false;
            mobilityInputsDisableTry = false;
        }

        void Update()
        {
            // For joystick commands
            controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out mobileBaseRotation);
            controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out mobileBaseTranslation);

            bool leftPrimaryButtonPressed;
            bool rightPrimaryButtonPressed;
            bool rightSecondaryButtonPressed;

            controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftPrimaryButtonPressed);
            controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out rightPrimaryButtonPressed);

            controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out rightSecondaryButtonPressed);
            if (controllers.controllerDeviceType == ControllersManager.SupportedDevices.HTCVive)
            {
                controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out rightSecondaryButtonPressed);
            }

            if (robotStatus != null && robotStatus.IsRobotTeleoperationActive() && robotStatus.IsMobilityActive() && robotStatus.IsMobilityOn() && !robotStatus.AreRobotMovementsSuspended())
            {
                if (!leftPrimaryButtonPressed && !rightPrimaryButtonPressed)
                {
                    mobilityInputsSuspended = false;
                    float r = Mathf.Sqrt(Mathf.Pow(mobileBaseTranslation[0], 2) + Mathf.Pow(mobileBaseTranslation[1], 2));
                    float phi = Mathf.Atan2(mobileBaseTranslation[1], mobileBaseTranslation[0]);

                    if (Mathf.Abs(phi) < (Mathf.PI / 16)) mobileBaseTranslation[1] = 0;
                    if ((phi > (Mathf.PI / 2 - Mathf.PI / 16)) && (phi < (Mathf.PI / 2 + Mathf.PI / 16))) mobileBaseTranslation[0] = 0;
                    if (Mathf.Abs(phi) > (Mathf.PI - Mathf.PI / 16)) mobileBaseTranslation[1] = 0;
                    if ((phi > (-Mathf.PI / 2 - Mathf.PI / 16)) && (phi < (-Mathf.PI / 2 + Mathf.PI / 16))) mobileBaseTranslation[0] = 0;

                    direction = new Vector2(mobileBaseTranslation[0], mobileBaseTranslation[1]);

                    float translationSpeed = maxSpeedFactor;
                    if (rightSecondaryButtonPressed)
                        translationSpeed = 1.0f;

                    mobilityCommands.SendMobileBaseDirection(new Vector3(direction[1] * translationSpeed, -direction[0] * translationSpeed, -mobileBaseRotation[0] * 1.5f));
                }
                else
                {
                    mobilityInputsSuspended = true;
                }
            }
            else
            {
                if(robotStatus.IsRobotTeleoperationActive() && (!robotStatus.IsMobilityActive() || !robotStatus.IsMobilityOn()))
                {
                    if (!leftPrimaryButtonPressed && !rightPrimaryButtonPressed)
                    {
                        if((mobileBaseRotation != new Vector2(0, 0) || mobileBaseTranslation != new Vector2(0, 0))) 
                        {
                            if(!mobilityInputsDisableTry)
                            {
                                event_OnTriedToSendCommands.Invoke();
                                mobilityInputsDisableTry = true;
                            }
                        }
                        else { mobilityInputsDisableTry = false; }
                    }
                }
            }
        }

        public bool CanGetUserMobilityInputs()
        {
            return !mobilityInputsSuspended;
        }

        public Vector2 GetMobileBaseDirection()
        {
            return direction;
        }

        public Vector2 GetAngleDirection()
        {
            return mobileBaseRotation;
        }
    }
}