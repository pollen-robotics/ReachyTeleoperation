using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Reachy.Sdk.Joint;

namespace TeleopReachy
{
    public class ErrorManager : MonoBehaviour
    {
        private gRPCDataController dataController;
        private gRPCMobileBaseController mobileBaseController;

        private gRPCVideoController videoController;

        private RobotPingWatcher robotPing;

        private List<JointId> warningHotMotors;
        private List<JointId> errorOverheatingMotors;

        private Queue<float> pingsQueue;
        private const int PINGS_QUEUE_SIZE = 20;
        private const float THRESHOLD_WARNING_BATTERY_LEVEL = 24.5f;
        private const float THRESHOLD_ERROR_BATTERY_LEVEL = 23.1f;
        private const float FPS_MINIMUM = 15f;
        public const float THRESHOLD_ERROR_MOTOR_TEMPERATURE = 55.0f;
        public const float THRESHOLD_WARNING_MOTOR_TEMPERATURE = 50.0f;

        public UnityEvent event_OnWarningHighLatency;
        public UnityEvent event_OnWarningUnstablePing;
        public UnityEvent<float> event_OnWarningLowBattery;
        public UnityEvent<float> event_OnErrorLowBattery;
        public UnityEvent<List<JointId>> event_OnWarningMotorsTemperatures;
        public UnityEvent<List<JointId>> event_OnErrorMotorsTemperatures;

        public float previousBatteryLevel;

        // Start is called before the first frame update
        void Start()
        {
            dataController = gRPCManager.Instance.gRPCDataController;
            dataController.event_OnStateUpdateTemperature.AddListener(CheckTemperatures);

            mobileBaseController = gRPCManager.Instance.gRPCMobileBaseController;
            mobileBaseController.event_OnMobileBaseBatteryLevelUpdate.AddListener(CheckBatteryLevel);

            videoController = gRPCManager.Instance.gRPCVideoController;

            robotPing = RobotDataManager.Instance.RobotPingWatcher;
            pingsQueue = new Queue<float>();
        }

        void Update()
        {
            CheckPingQuality();
            CheckVideoQuality();
        }

        void CheckPingQuality()
        {
            if (robotPing.GetPing() > RobotPingWatcher.THRESHOLD_LOW_QUALITY_PING)
                event_OnWarningHighLatency.Invoke();
            else if (robotPing.GetIsUnstablePing())
                event_OnWarningUnstablePing.Invoke();
        }

        void CheckVideoQuality()
        {
            float fps = videoController.GetMeanFPS();
            if ((fps != -1) && (fps < FPS_MINIMUM)) {
                event_OnWarningHighLatency.Invoke();
            }
        }

        public void NotifyNetworkUnstability()
        {
            event_OnWarningUnstablePing.Invoke();
        }

        protected void CheckTemperatures(Dictionary<JointId, float> Temperatures)
        {
            warningHotMotors = new List<JointId>();
            errorOverheatingMotors = new List<JointId>();

            foreach (KeyValuePair<JointId, float> motor in Temperatures)
            {
                if (motor.Value >= THRESHOLD_ERROR_MOTOR_TEMPERATURE) errorOverheatingMotors.Add(motor.Key);
                else if (motor.Value >= THRESHOLD_WARNING_MOTOR_TEMPERATURE) warningHotMotors.Add(motor.Key);
            }

            if (warningHotMotors.Count > 0)
                event_OnWarningMotorsTemperatures.Invoke(warningHotMotors);
            if (errorOverheatingMotors.Count > 0)
                event_OnErrorMotorsTemperatures.Invoke(errorOverheatingMotors);
        }

        protected void CheckBatteryLevel(float batteryLevel)
        {
            previousBatteryLevel = batteryLevel;
            if (batteryLevel < THRESHOLD_ERROR_BATTERY_LEVEL)
                event_OnErrorLowBattery.Invoke(batteryLevel);
            else if (batteryLevel < THRESHOLD_WARNING_BATTERY_LEVEL)
                event_OnWarningLowBattery.Invoke(batteryLevel);
        }

        public void CheckBatteryStatus()
        {
            if (previousBatteryLevel < THRESHOLD_ERROR_BATTERY_LEVEL)
                event_OnErrorLowBattery.Invoke(previousBatteryLevel);
            else if (previousBatteryLevel < THRESHOLD_WARNING_BATTERY_LEVEL)
                event_OnWarningLowBattery.Invoke(previousBatteryLevel);
        }
    }
}
