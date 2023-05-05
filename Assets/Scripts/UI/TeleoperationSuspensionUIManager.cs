using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeleopReachy
{
    public class TeleoperationSuspensionUIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform loaderA;

        private bool isLoaderActive = false;

        private RobotStatus robotStatus;
        private TeleoperationSuspensionManager suspensionManager;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.StartListening(EventNames.HeadsetRemoved, DisplaySuspensionWarning);

            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStopTeleoperation.AddListener(HideSuspensionWarning);

            suspensionManager = TeleoperationSuspensionManager.Instance;

            HideSuspensionWarning();
        }

        // Update is called once per frame
        void DisplaySuspensionWarning()
        {
            if(robotStatus.IsRobotTeleoperationActive())
            {
                isLoaderActive = true;
                transform.ActivateChildren(true);
            }
        }

        void HideSuspensionWarning()
        {
            loaderA.GetComponent<UnityEngine.UI.Image>().fillAmount = suspensionManager.indicatorTimer;
            isLoaderActive = false;
            transform.ActivateChildren(false);
        }


        // Update is called once per frame
        void Update()
        {
            if (isLoaderActive)
            {
                loaderA.GetComponent<UnityEngine.UI.Image>().fillAmount = suspensionManager.indicatorTimer;
            }
        }
    }
}

