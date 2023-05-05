using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class RightArmButtonManager : MonoBehaviour
    {
        [SerializeField]
        public Button rightArmButton;

        private RobotConfig robotConfig;
        private RobotStatus robotStatus;

        private bool needUpdateButton = false;
        private bool isInteractable = false;
        private ColorBlock buttonColor;
        private string buttonText;

        void Awake()
        {
            rightArmButton.onClick.AddListener(SwitchButtonMode);

            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;

            robotConfig.event_OnConfigChanged.AddListener(CheckRightArmPresence);

            rightArmButton.interactable = false;

            CheckRightArmPresence();
        }

        void SwitchButtonMode()
        {
            robotStatus.SetRightArmOn(!robotStatus.IsRightArmOn());
            
            if(robotStatus.IsRightArmOn())
            {
                rightArmButton.colors = ColorsManager.colorsActivated;
                rightArmButton.transform.GetChild(0).GetComponent<Text>().text = "Right arm ON";
            }
            else
            {
                rightArmButton.colors = ColorsManager.colorsDeactivated;
                rightArmButton.transform.GetChild(0).GetComponent<Text>().text = "Right arm OFF";
            }
        }

        void Update()
        {
            if(needUpdateButton)
            {
                rightArmButton.interactable = isInteractable;
                rightArmButton.colors = buttonColor;
                rightArmButton.transform.GetChild(0).GetComponent<Text>().text = buttonText;
                needUpdateButton = false;
            }
        }

        void CheckRightArmPresence()
        {
            if(robotConfig.HasRightArm())
            {
                isInteractable = true;
                if(robotStatus.IsRightArmOn())
                {
                    buttonColor = ColorsManager.colorsActivated;
                    buttonText = "Right arm ON";
                }
                else
                {
                    buttonColor = ColorsManager.colorsDeactivated;
                    buttonText = "Right arm OFF";
                }
            }
            else
            {
                buttonColor = ColorsManager.colorsDeactivated;
                buttonText = "Right arm OFF";
                isInteractable = false;
            }
            needUpdateButton = true;
        }
    }
}