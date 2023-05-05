using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace TeleopReachy
{
    public class OfflineMenuManager : Singleton<OfflineMenuManager>
    {
        public enum OfflineMenuItem
        {
            LockAndHome, Cancel, Home
        }

        private ControllersManager controllers;

        private RobotStatus robotStatus;
        private RobotConfig robotConfig;

        public OfflineMenuItem selectedItem;

        private bool isOfflineMenuActive;

        private bool rightPrimaryButtonPreviouslyPressed;
        private bool leftPrimaryButtonPreviouslyPressed;

        public float indicatorTimer = 0.0f;
        private const float minIndicatorTimer = 0.0f;

        public UnityEvent event_OnAskForOfflineMenu;
        public UnityEvent event_OnLeaveOfflineMenu;

        // Start is called before the first frame update
        void Start()
        {
            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStopTeleoperation.AddListener(DeactivateOfflineMenu);

            controllers = ControllersManager.Instance;

            selectedItem = OfflineMenuItem.Cancel;

            isOfflineMenuActive = false;
        }

        // Update is called once per frame
        void Update()
        {
            bool rightPrimaryButtonPressed = false;
            controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out rightPrimaryButtonPressed);

            bool leftPrimaryButtonPressed = false;
            controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftPrimaryButtonPressed);

            if (robotStatus.IsRobotTeleoperationActive() && !robotStatus.AreRobotMovementsSuspended())
            {
                if (rightPrimaryButtonPressed && !rightPrimaryButtonPreviouslyPressed)
                {
                    selectedItem = OfflineMenuItem.Home;
                    if (!isOfflineMenuActive)
                    {
                        event_OnAskForOfflineMenu.Invoke();
                        isOfflineMenuActive = true;
                    }
                }

                if (isOfflineMenuActive)
                {
                    if (rightPrimaryButtonPressed && rightPrimaryButtonPreviouslyPressed)
                    {
                        indicatorTimer += Time.deltaTime / 2;
                        if (indicatorTimer >= 1.0f)
                        {
                            ExitOffLineMenu();
                            if (selectedItem == OfflineMenuItem.LockAndHome)
                                robotStatus.LockRobotPosition();
                            EventManager.TriggerEvent(EventNames.BackToMirrorScene);

                        }

                        if (leftPrimaryButtonPressed && !leftPrimaryButtonPreviouslyPressed)
                        {
                            selectedItem = OfflineMenuItem.LockAndHome;
                        }
                        else if (!leftPrimaryButtonPressed && leftPrimaryButtonPreviouslyPressed)
                        {
                            selectedItem = OfflineMenuItem.Home;
                        }

                    }
                    else if (!rightPrimaryButtonPressed && rightPrimaryButtonPreviouslyPressed)
                    {
                        ExitOffLineMenu();
                    }
                }
            }

            rightPrimaryButtonPreviouslyPressed = rightPrimaryButtonPressed;
            leftPrimaryButtonPreviouslyPressed = leftPrimaryButtonPressed;
        }

        void ExitOffLineMenu()
        {
            indicatorTimer = minIndicatorTimer;
            event_OnLeaveOfflineMenu.Invoke();
            DeactivateOfflineMenu();
        }

        void DeactivateOfflineMenu()
        {
            isOfflineMenuActive = false;
        }
    }
}
