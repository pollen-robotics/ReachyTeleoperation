using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class VRKeyboard : MonoBehaviour
    {
#if UNITY_ANDROID
        private TouchScreenKeyboard keyboard = null;
        private InputField inputfield_toedit = null;
#endif
        public void OpenKeyboard(InputField inputf)
        {
#if UNITY_ANDROID
                EventManager.TriggerEvent(EventNames.HideXRay);
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NamePhonePad);
                inputfield_toedit = inputf;
#endif
        }

        public void OpenKeyboardIP(InputField inputf)
        {
#if UNITY_ANDROID 
                EventManager.TriggerEvent(EventNames.HideXRay);
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.URL);
                inputfield_toedit = inputf;
#endif
        }

        void Update()
        {
#if UNITY_ANDROID
            if (keyboard != null)
            {
                if (TouchScreenKeyboard.visible == false)
                {
                    if (keyboard.status == TouchScreenKeyboard.Status.Done)
                    {
                        inputfield_toedit.text = keyboard.text;
                        keyboard = null;
                        inputfield_toedit = null;
                        EventManager.TriggerEvent(EventNames.ShowXRay);
                    }
                }
                else
                {
                    inputfield_toedit.text = keyboard.text;
                }
            }
#endif
        }
    }
}