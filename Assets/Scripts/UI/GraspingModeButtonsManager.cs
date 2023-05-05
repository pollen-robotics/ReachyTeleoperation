using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class GraspingModeButtonsManager : MonoBehaviour
    {
        public Button modeFullControlButton;
        public Button modeLockedButton;
        
        private RobotConfig robotConfig;
        private RobotStatus robotStatus;

        private bool needUpdateButton;
        private bool isInteractable = false;
        private ColorBlock buttonColorFullControl;
        private ColorBlock buttonColorLocked;

        void Awake()
        {
            modeFullControlButton.onClick.AddListener(delegate { SwitchToGraspingLockMode(false); });
            modeLockedButton.onClick.AddListener(delegate { SwitchToGraspingLockMode(true); });

            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;

            robotConfig.event_OnConfigChanged.AddListener(CheckGripperPresence);
            robotStatus.event_OnStopTeleoperation.AddListener(UpdateGraspingButtons);

            modeFullControlButton.interactable = isInteractable;
            modeLockedButton.interactable = isInteractable;

            needUpdateButton = false;

            CheckGripperPresence();
        }

        void SwitchToGraspingLockMode(bool isGraspingLockOn)
        {
            robotStatus.SetGraspingLockActivated(isGraspingLockOn);
            needUpdateButton = true;
        }

        void Update()
        {
            if(needUpdateButton)
            {
                modeFullControlButton.interactable = isInteractable;
                modeLockedButton.interactable = isInteractable;
                if (robotStatus.IsGraspingLockActivated())
                {
                    modeFullControlButton.colors = ColorsManager.colorsDeactivated;
                    modeLockedButton.colors = ColorsManager.colorsActivated;

                }
                else
                {
                    modeFullControlButton.colors = ColorsManager.colorsActivated;
                    modeLockedButton.colors = ColorsManager.colorsDeactivated;
                }
                needUpdateButton = false;
            }
        }

        void UpdateGraspingButtons()
        {
            needUpdateButton = true;
        }

        void CheckGripperPresence()
        {
            if (robotConfig.HasRightGripper() || robotConfig.HasLeftGripper())
            {
                isInteractable = true;
            }
            else
            {
                isInteractable = false;
            }
            needUpdateButton = true;
        }
    }
}