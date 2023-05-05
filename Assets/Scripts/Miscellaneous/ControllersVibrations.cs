using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using Reachy;

namespace TeleopReachy
{
    public class ControllersVibrations : MonoBehaviour
    {
        private UnityEngine.XR.InputDevice activeDevice;

        public void SetActiveController(UnityEngine.XR.InputDevice c)
        {
            activeDevice = c;
        }

        public void OnUIEnterVibration()
        {
            UIEnterVibration(0.01f);
        }

        public void OnUIEnterVibration(Button button)
        {
            if (button.interactable)
            {
                UIEnterVibration(0.01f);
            }
        }

        public void OnUIEnterVibration(InputField inputField)
        {
            if (inputField.interactable)
            {
                UIEnterVibration(0.01f);
            }
        }

        public void OnUIEnterVibration(Toggle toggle)
        {
            if (toggle.interactable)
            {
                UIEnterVibration(0.01f);
            }
        }

        private void UIEnterVibration(float duration)
        {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (activeDevice.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.2f;
                    activeDevice.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}