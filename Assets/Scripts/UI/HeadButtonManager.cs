using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class HeadButtonManager : MonoBehaviour
    {
        [SerializeField]
        private Button headButton;

        private RobotConfig robotConfig;
        private RobotStatus robotStatus;

        private bool needUpdateButton = false;
        private bool isInteractable = false;
        private ColorBlock buttonColor;
        private string buttonText;

        void Awake()
        {
            headButton.onClick.AddListener(SwitchButtonMode);

            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;

            robotConfig.event_OnConfigChanged.AddListener(CheckHeadPresence);

            headButton.interactable = false;

            CheckHeadPresence();
        }

        void SwitchButtonMode()
        {
            robotStatus.SetHeadOn(!robotStatus.IsHeadOn());
            
            if(robotStatus.IsHeadOn())
            {
                headButton.colors = ColorsManager.colorsActivated;
                headButton.transform.GetChild(0).GetComponent<Text>().text = "Head ON";
            }
            else
            {
                headButton.colors = ColorsManager.colorsDeactivated;
                headButton.transform.GetChild(0).GetComponent<Text>().text = "Head OFF";
            }
        }

        void Update()
        {
            if(needUpdateButton)
            {
                headButton.interactable = isInteractable;
                headButton.colors = buttonColor;
                headButton.transform.GetChild(0).GetComponent<Text>().text = buttonText;
                needUpdateButton = false;
            }
        }

        void CheckHeadPresence()
        {
            if(robotConfig.HasHead())
            {
                isInteractable = true;
                if(robotStatus.IsHeadOn())
                {
                    buttonColor = ColorsManager.colorsActivated;
                    buttonText = "Head ON";
                }
                else
                {
                    buttonColor = ColorsManager.colorsDeactivated;
                    buttonText = "Head OFF";
                }
            }
            else
            {
                buttonColor = ColorsManager.colorsDeactivated;
                buttonText = "Head OFF";
                isInteractable = false;
            }
            needUpdateButton = true;
        }
    }
}