using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System;

namespace TeleopReachy
{
    public class CustomHandedInputSelector : MonoBehaviour
    {
        // Controllers
        public GameObject controllerLeft;
        public GameObject controllerRight;

        private GameObject activeController;

        public Transform robotDataManager;

        private RobotStatus robotStatus;

        private bool allowActiveControllerChange;

        private List<UnityEngine.XR.InputDevice> leftHandDevices;
        private List<UnityEngine.XR.InputDevice> rightHandDevices;
        private UnityEngine.XR.InputDevice leftHandDevice;
        private UnityEngine.XR.InputDevice rightHandDevice;

        private ControllersVibrations hapticController;

        public static CustomHandedInputSelector Instance { get; private set; }
        // Start is called before the first frame update
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            activeController = controllerRight;

            hapticController = gameObject.GetComponent<ControllersVibrations>();

            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);

            if (inputDevices.Count > 0)
            {
                UpdateDevicesList();
            }
            UnityEngine.XR.InputDevices.deviceConnected += CheckDevice;

            if (robotDataManager != null)
            {
                robotStatus = RobotDataManager.Instance.RobotStatus;
                robotStatus.event_OnStartTeleoperation.AddListener(HideControllerLaser);
                robotStatus.event_OnStopTeleoperation.AddListener(ShowControllerLaser);
            }

            EnableSteamLaserPointer(controllerLeft, false);
            allowActiveControllerChange = true;
        }

        void Update()
        {
            if (allowActiveControllerChange)
            {
                bool triggerLeft;
                if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerLeft) && triggerLeft)
                {
                    if (activeController != controllerLeft)
                    {
                        EnableSteamLaserPointer(controllerLeft, true);
                        EnableSteamLaserPointer(controllerRight, false);
                        activeController = controllerLeft;
                        hapticController.SetActiveController(leftHandDevice);
                    }
                }

                bool triggerRight;
                if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerRight) && triggerRight)
                {
                    if (activeController != controllerRight)
                    {
                        EnableSteamLaserPointer(controllerRight, true);
                        EnableSteamLaserPointer(controllerLeft, false);
                        activeController = controllerRight;
                        hapticController.SetActiveController(rightHandDevice);
                    }
                }
            }
        }

        private void CheckDevice(UnityEngine.XR.InputDevice device)
        {
            UpdateDevicesList();
        }

        private void UpdateDevicesList()
        {
            leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

            rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

            if (leftHandDevices.Count == 1)
            {
                leftHandDevice = leftHandDevices[0];
            }
            if (rightHandDevices.Count == 1)
            {
                rightHandDevice = rightHandDevices[0];
            }

            if (activeController == controllerRight) hapticController.SetActiveController(rightHandDevice);
            else hapticController.SetActiveController(leftHandDevice);
        }

        private void ShowControllerLaser()
        {
            EnableSteamLaserPointer(activeController, true);
            allowActiveControllerChange = true;
        }

        private void HideControllerLaser()
        {
            allowActiveControllerChange = false;
            EnableSteamLaserPointer(activeController, false);
        }

        private void EnableSteamLaserPointer(GameObject controller, bool enable)
        {
            XRRayInteractor xrRay = controller.GetComponent<XRRayInteractor>();
            xrRay.enabled = enable;
        }

        public UnityEngine.XR.InputDevice GetActiveController()
        {
            if (activeController == controllerRight) return rightHandDevice;
            else return leftHandDevice;
        }
    }
}