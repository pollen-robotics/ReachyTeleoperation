using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using Reachy.Sdk.Kinematics;

namespace TeleopReachy
{
    public class TrackedHandManager : MonoBehaviour
    {
        public ArmSide side_id;

        void Start()
        {
            ControllersManager.Instance.event_OnDevicesUpdate.AddListener(DefineTrackedHandOrientation);
        }

        private void DefineTrackedHandOrientation()
        {
            if(ControllersManager.Instance.controllerDeviceType == ControllersManager.SupportedDevices.Oculus)
            {
                transform.localPosition = new Vector3(0, -0.05f, 0);
                UnityEngine.Quaternion targetRotation = new UnityEngine.Quaternion();
                if(side_id == ArmSide.Left) targetRotation.eulerAngles = new Vector3(-60, 30, 0);
                else targetRotation.eulerAngles = new Vector3(-60, -30, 0);
                transform.rotation = targetRotation;
            }

            if(ControllersManager.Instance.controllerDeviceType == ControllersManager.SupportedDevices.ValveIndex)
            {
                transform.localPosition = new Vector3(0, 0, 0);
                UnityEngine.Quaternion targetRotation = new UnityEngine.Quaternion();
                if(side_id == ArmSide.Left) targetRotation.eulerAngles = new Vector3(0, 0, 20);
                else targetRotation.eulerAngles = new Vector3(0, 0, -20);
                transform.rotation = targetRotation;
            }

            if(ControllersManager.Instance.controllerDeviceType == ControllersManager.SupportedDevices.HTCVive)
            {
                transform.localPosition = new Vector3(0, 0, 0);
                UnityEngine.Quaternion targetRotation = new UnityEngine.Quaternion();
                if(side_id == ArmSide.Left) targetRotation.eulerAngles = new Vector3(0, 0, 0);
                else targetRotation.eulerAngles = new Vector3(0, 0, 0);
                transform.rotation = targetRotation;
            }
        }
    }
}
