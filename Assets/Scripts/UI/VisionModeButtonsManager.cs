using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class VisionModeButtonsManager : MonoBehaviour
    {
        public Button mode2DButton;
        public Button mode3DButton;

        public GameObject info3d = null;

        private RobotConfig robotConfig;
        private RobotStatus robotStatus;

        private bool needUpdateButton;
        private bool isInteractable2D = false;
        private bool isInteractable3D = false;
        private ColorBlock buttonColor2D;
        private ColorBlock buttonColor3D;

        void Awake()
        {
            mode2DButton.onClick.AddListener(delegate { SwitchTo2DMode(true); });
            mode3DButton.onClick.AddListener(delegate { SwitchTo2DMode(false); });

            robotConfig = RobotDataManager.Instance.RobotConfig;
            robotStatus = RobotDataManager.Instance.RobotStatus;

            robotConfig.event_OnConfigChanged.AddListener(CheckHeadPresence);

            mode2DButton.interactable = false;
            mode3DButton.interactable = false;
            info3d.SetActive(false);

            CheckHeadPresence();
        }

        void SwitchTo2DMode(bool is2DOn)
        {
            robotStatus.Set2DVisionModeOn(is2DOn);
            needUpdateButton = true;
        }

        void Update()
        {
            if (needUpdateButton)
            {
                mode2DButton.interactable = isInteractable2D;
                mode3DButton.interactable = isInteractable3D;
                info3d.SetActive(!isInteractable3D);
                if (robotStatus.Is2DVisionModeOn())
                {
                    mode2DButton.colors = ColorsManager.colorsActivated;
                    mode3DButton.colors = ColorsManager.colorsDeactivated;
                }
                else
                {
                    mode2DButton.colors = ColorsManager.colorsDeactivated;
                    mode3DButton.colors = ColorsManager.colorsActivated;
                }
                needUpdateButton = false;
            }
        }

        void CheckHeadPresence()
        {
            if (robotConfig.HasHead())
            {
                isInteractable2D = true;
                isInteractable3D = true; // TODO remove this
                if (robotConfig.RobotGeneration == RobotGenerationCode.V2023)
                    isInteractable3D = true;
            }
            else
            {
                isInteractable2D = false;
                isInteractable3D = false;
            }
            needUpdateButton = true;
        }
    }
}