using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace TeleopReachy
{
    public class MirrorPanelUIManager : Singleton<MirrorPanelUIManager>
    {
        [SerializeField]
        private Transform statusPanel;

        [SerializeField]
        private Transform advancedOptions;

        [SerializeField]
        private Transform helpPanel;

        private bool isStatusPanelOpen;
        private bool isAdvancedOptionsOpen;
        private bool isHelpPanelOpen;

        private float timeElapsedStatusPanel;
        private float timeElapsedAdvancedOptions;

        private float timeElapsedHelpPanel;

        private bool needUpdateStatusPanel;
        private bool needUpdateAdvancedOptions;
        private bool needUpdateHelpPanel;

        private readonly Vector3 closedStatusPanelPosition = new Vector3(745, -14, 0);
        private readonly Vector3 openStatusPanelPosition = new Vector3(50, -14, 0);

        private readonly Vector3 closedHelpPanelPosition = new Vector3(745, -300, 0);
        private readonly Vector3 openHelpPanelPosition = new Vector3(50, -300, 0);

        private readonly Vector3 closedAdvancedOptionsPosition = new Vector3(-605, 514, 0);
        private readonly Vector3 openAdvancedOptionsPosition = new Vector3(-50, 514, 0);

        private Vector3 lerpStatusPanelStartingPosition;
        private Vector3 lerpStatusPanelGoalPosition;

        private Vector3 lerpHelpPanelStartingPosition;
        private Vector3 lerpHelpPanelGoalPosition;

        private Vector3 lerpAdvancedOptionsStartingPosition;
        private Vector3 lerpAdvancedOptionsGoalPosition;

        // Start is called before the first frame update
        void Start()
        {
            isStatusPanelOpen = false;
            isAdvancedOptionsOpen = false;
            isHelpPanelOpen = false;

            needUpdateStatusPanel = false;
            needUpdateAdvancedOptions = false;
            needUpdateHelpPanel = false;
        }

        private void OpenClosePanel(ref Vector3 lerpPanelStartingPosition, ref Transform panel, ref float timeElapsedPanel, ref Vector3 lerpPanelGoalPosition,
                                    Vector3 closedPanelPosition, Vector3 openPanelPosition, ref bool isPanelOpen, ref bool needUpdatePanel)
        {
            lerpPanelStartingPosition = panel.localPosition;
            timeElapsedPanel = 0;
            if (isPanelOpen)
            {
                lerpPanelGoalPosition = closedPanelPosition;
            }
            else
            {
                lerpPanelGoalPosition = openPanelPosition;
            }
            isPanelOpen = !isPanelOpen;
            needUpdatePanel = true;
        }

        public void OpenCloseStatusPanel()
        {
            OpenClosePanel(ref lerpStatusPanelStartingPosition, ref statusPanel, ref timeElapsedStatusPanel, ref lerpStatusPanelGoalPosition,
                           closedStatusPanelPosition, openStatusPanelPosition, ref isStatusPanelOpen, ref needUpdateStatusPanel);
        }

        public void OpenCloseHelpPanel()
        {
            OpenClosePanel(ref lerpHelpPanelStartingPosition, ref helpPanel, ref timeElapsedHelpPanel, ref lerpHelpPanelGoalPosition,
                           closedHelpPanelPosition, openHelpPanelPosition, ref isHelpPanelOpen, ref needUpdateHelpPanel);
        }

        public void OpenCloseAdvancedOptions()
        {
            OpenClosePanel(ref lerpAdvancedOptionsStartingPosition, ref advancedOptions, ref timeElapsedAdvancedOptions, ref lerpAdvancedOptionsGoalPosition,
                           closedAdvancedOptionsPosition, openAdvancedOptionsPosition, ref isAdvancedOptionsOpen, ref needUpdateAdvancedOptions);
        }

        private void NeedUpdatePanel(ref float timeElapsedPanel, ref Transform panel, ref bool needUpdatePanel, Vector3 lerpPanelGoalPosition, Vector3 lerpPanelStartingPosition)
        {
            timeElapsedPanel += Time.deltaTime;
            if (timeElapsedPanel >= 1)
            {
                timeElapsedPanel = 0;
                panel.localPosition = lerpPanelGoalPosition;
                needUpdatePanel = false;
            }
            else
            {
                float fTime = timeElapsedPanel / 1;
                panel.localPosition = Vector3.Lerp(lerpPanelStartingPosition, lerpPanelGoalPosition, fTime);
            }
        }

        void Update()
        {
            if (needUpdateStatusPanel)
            {
                NeedUpdatePanel(ref timeElapsedStatusPanel, ref statusPanel, ref needUpdateStatusPanel, lerpStatusPanelGoalPosition, lerpStatusPanelStartingPosition);
            }

            if (needUpdateHelpPanel)
            {
                NeedUpdatePanel(ref timeElapsedHelpPanel, ref helpPanel, ref needUpdateHelpPanel, lerpHelpPanelGoalPosition, lerpHelpPanelStartingPosition);
            }

            if (needUpdateAdvancedOptions)
            {
                NeedUpdatePanel(ref timeElapsedAdvancedOptions, ref advancedOptions, ref needUpdateAdvancedOptions, lerpAdvancedOptionsGoalPosition, lerpAdvancedOptionsStartingPosition);
            }
        }
    }
}

