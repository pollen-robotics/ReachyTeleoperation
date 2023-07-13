using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Reachy.Sdk.Joint;


namespace TeleopReachy
{
    public class ErrorMessageUIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform motorsErrorPanel;
        [SerializeField]
        private Transform batteryErrorPanel;
        [SerializeField]
        private Transform pingErrorPanel;

        private RobotStatus robotStatus;
        private ErrorManager errorManager;

        private int nbMotorsWarning = 0;
        private int nbMotorsError = 0;
        private float previousBatteryLevel = 0;

        private Coroutine motorsErrorPanelDisplay;
        private Coroutine motorsWarningValue;
        private Coroutine motorsErrorValue;
        private Coroutine batteryErrorPanelDisplay;
        private Coroutine pingErrorPanelDisplay;

        private bool needMotorsUpdate;
        private bool needPingUpdate;
        private bool needBatteryUpdate;

        private bool wasErrorTemperature;
        private bool wasWarningTemperature;

        private string warningPingText;
        private string warningBatteryText;
        private Color32 warningBatteryColor;

        void Start()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStopTeleoperation.AddListener(HideWarningMessage);
            robotStatus.event_OnStartTeleoperation.AddListener(ReinitializeValues);

            errorManager = RobotDataManager.Instance.ErrorManager;
            errorManager.event_OnWarningMotorsTemperatures.AddListener(WarningMotorTemperature);
            errorManager.event_OnWarningHighLatency.AddListener(WarningHighLatency);
            errorManager.event_OnWarningUnstablePing.AddListener(WarningUnstablePing);
            errorManager.event_OnWarningLowBattery.AddListener(WarningLowBattery);

            errorManager.event_OnErrorMotorsTemperatures.AddListener(ErrorMotorTemperature);
            errorManager.event_OnErrorLowBattery.AddListener(ErrorLowBattery);

            HideWarningMessage();
        }
        
        void Update()
        {
            if(needBatteryUpdate)
            {
                if (batteryErrorPanelDisplay != null) StopCoroutine(batteryErrorPanelDisplay);
                batteryErrorPanel.ActivateChildren(true);
                batteryErrorPanel.GetChild(1).GetComponent<Text>().text = warningBatteryText;
                batteryErrorPanel.GetChild(0).GetComponent<Image>().color = warningBatteryColor;
                batteryErrorPanelDisplay = StartCoroutine(HidePanelAfterSeconds(3, batteryErrorPanel));

                needBatteryUpdate = false;
            }

            if(needPingUpdate)
            {
                if (pingErrorPanelDisplay != null) StopCoroutine(pingErrorPanelDisplay);
                pingErrorPanel.ActivateChildren(true);
                pingErrorPanel.GetChild(1).GetComponent<Text>().text = warningPingText;
                pingErrorPanelDisplay = StartCoroutine(HidePanelAfterSeconds(3, pingErrorPanel));

                needPingUpdate = false;
            }

            if(needMotorsUpdate)
            {
                if (motorsWarningValue != null && wasWarningTemperature) StopCoroutine(motorsWarningValue);
                if (motorsErrorValue != null && wasErrorTemperature) StopCoroutine(motorsErrorValue);
                if (motorsErrorPanelDisplay != null) StopCoroutine(motorsErrorPanelDisplay);
                motorsErrorPanel.ActivateChildren(true);
                if(wasWarningTemperature)
                {
                    string warningText = nbMotorsWarning > 1 ? nbMotorsWarning + " Motors are heating up" : "1 Motor is heating up";
                    motorsErrorPanel.GetChild(1).GetComponent<Text>().text = warningText;
                }
                if(wasErrorTemperature)
                {
                    string errorText = nbMotorsError > 1 ? nbMotorsError + " Motors in critical error" : "1 Motor in critical error";
                    motorsErrorPanel.GetChild(3).GetComponent<Text>().text = errorText;
                    motorsErrorPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_red;
                }
                if(wasWarningTemperature) motorsWarningValue = StartCoroutine(ReinitializeMotorsWarningValue(3));
                if(wasErrorTemperature) motorsErrorValue = StartCoroutine(ReinitializeMotorsErrorValue(3));
                motorsErrorPanelDisplay = StartCoroutine(HidePanelAfterSeconds(3, motorsErrorPanel));

                wasWarningTemperature = false;
                wasErrorTemperature = false;
                needMotorsUpdate = false;
            }
        }

        void ReinitializeValues()
        {
            nbMotorsWarning = 0;
            previousBatteryLevel = 0;
        }

        void WarningMotorTemperature(List<JointId> motors)
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                if(motors.Count > nbMotorsWarning)
                {
                    nbMotorsWarning = motors.Count;
                    wasWarningTemperature = true;
                    needMotorsUpdate = true;
                }
            }
        }

        IEnumerator ReinitializeMotorsWarningValue(int seconds)
        {
            yield return new WaitForSeconds(3);
            string warningText = "0 Motor is heating up";
            motorsErrorPanel.GetChild(1).GetComponent<Text>().text = warningText;
        }

        void ErrorMotorTemperature(List<JointId> motors)
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                nbMotorsError = motors.Count;
                wasErrorTemperature = true;
                needMotorsUpdate = true;
            }
        }

        IEnumerator ReinitializeMotorsErrorValue(int seconds)
        {
            yield return new WaitForSeconds(3);
            string warningText = "0 Motor in critical error";
            motorsErrorPanel.GetChild(3).GetComponent<Text>().text = warningText;
            motorsErrorPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_black;
        }

        void WarningUnstablePing()
        {
            SetPingWarningMessage("Unstable network");
        }

        void WarningHighLatency()
        {
            SetPingWarningMessage("Low speed network");
        }

        private void SetPingWarningMessage(string warningText)
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                warningPingText = warningText;
                needPingUpdate = true;
            }
        }

        void WarningLowBattery(float batteryLevel)
        {
            if(previousBatteryLevel == 0 || (previousBatteryLevel - batteryLevel > 0.2f))
            {
                SetErrorBatteryMessage("Low battery", ColorsManager.error_black);
                previousBatteryLevel = batteryLevel;
            }
        }

        void ErrorLowBattery(float batteryLevel)
        {
            SetErrorBatteryMessage("No battery", ColorsManager.error_red);
            previousBatteryLevel = batteryLevel;
        }

        private void SetErrorBatteryMessage(string errorText, Color32 color)
        {
            if (robotStatus.IsRobotTeleoperationActive())
            {
                warningBatteryText = errorText;
                warningBatteryColor = color;
                needBatteryUpdate = true;
            }
        }

        void HideWarningMessage()
        {
            if (batteryErrorPanelDisplay != null) StopCoroutine(batteryErrorPanelDisplay);
            if (pingErrorPanelDisplay != null) StopCoroutine(pingErrorPanelDisplay);
            if (motorsErrorPanelDisplay != null) StopCoroutine(motorsErrorPanelDisplay);
            motorsErrorPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_black;
            batteryErrorPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_black;
            pingErrorPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_black;
            motorsErrorPanel.ActivateChildren(false);
            batteryErrorPanel.ActivateChildren(false);
            pingErrorPanel.ActivateChildren(false);
        }

        void HideWarningMessage(object sender, EventArgs e)
        {
            HideWarningMessage();
        }

        IEnumerator HidePanelAfterSeconds(int seconds, Transform masterPanel)
        {
            yield return new WaitForSeconds(seconds);
            masterPanel.GetChild(0).GetComponent<Image>().color = ColorsManager.error_black;
            masterPanel.ActivateChildren(false);
        }
    }
}
