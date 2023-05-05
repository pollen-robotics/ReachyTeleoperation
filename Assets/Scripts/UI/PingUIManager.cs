using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TeleopReachy
{
    public class PingUIManager : MonoBehaviour
    {
        [SerializeField]
        private Text pingValue;
        [SerializeField]
        private Text pingQualityText;
        [SerializeField]
        private RawImage pingIcon;

        private RobotPingWatcher pingWatcher;
        private ErrorManager errorManager;

        private Coroutine warningEnd;

        private bool hasWarningActivated;

        // Start is called before the first frame update
        void Start()
        {
            pingWatcher = RobotDataManager.Instance.RobotPingWatcher;

            errorManager = RobotDataManager.Instance.ErrorManager;
            errorManager.event_OnWarningHighLatency.AddListener(WarningHighLatency);
            errorManager.event_OnWarningUnstablePing.AddListener(WarningUnstablePing);

            hasWarningActivated = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(pingValue != null) pingValue.text = "Ping : " + (int)pingWatcher.GetPing() + " ms";

            if (!hasWarningActivated)
            {
                if(pingQualityText != null)
                {
                    pingQualityText.text = "Good network connection";
                    pingQualityText.color = ColorsManager.blue;
                }
                if(pingIcon != null)
                {
                    pingIcon.color = ColorsManager.blue;
                }
            }
        }

        void WarningUnstablePing()
        {
            SetPingMessage("Unstable network connection", ColorsManager.orange);
        }

        void WarningHighLatency()
        {
            SetPingMessage("Low speed network connection", ColorsManager.purple);
        }

        private void SetPingMessage(string message, Color32 color)
        {
            if (warningEnd != null) StopCoroutine(warningEnd);
            hasWarningActivated = true;
            if(pingQualityText != null)
            {
                pingQualityText.text = message;
                pingQualityText.color = color;
            }
            if(pingIcon != null)
            {
                pingIcon.color = color;
            }
            warningEnd = StartCoroutine(KeepOneSecond());
        }

        IEnumerator KeepOneSecond()
        {
            yield return new WaitForSeconds(1);
            hasWarningActivated = false;
        }
    }
}
