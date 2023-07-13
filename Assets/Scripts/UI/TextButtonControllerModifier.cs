using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class TextButtonControllerModifier : MonoBehaviour
    {
        private Text textToChange;
        public ControllersManager controllers;

        private const string primLeftButton = "<primLeft>";
        private const string primRightButton = "<primRight>";
        private const string miniPrimRightButton = "<miniPrimRight>";

        // Start is called before the first frame update
        void Start()
        {
            textToChange = GetComponent<Text>();
            controllers = ActiveControllerManager.Instance.ControllersManager;
            textToChange.text = ChangeTextAccordingToController(textToChange.text);
        }

        public string ChangeTextAccordingToController(string stringToChange)
        {
            switch (controllers.controllerDeviceType)
            {
                case ControllersManager.SupportedDevices.Oculus:
                    {
                        stringToChange = stringToChange.Replace(primRightButton, "A");
                        stringToChange = stringToChange.Replace(miniPrimRightButton, "A");
                        stringToChange = stringToChange.Replace(primLeftButton, "X");
                        break;
                    }
                case ControllersManager.SupportedDevices.HTCVive:
                    {
                        stringToChange = stringToChange.Replace(primRightButton, "Right menu ☰");
                        stringToChange = stringToChange.Replace(miniPrimRightButton, "☰");
                        stringToChange = stringToChange.Replace(primLeftButton, "Left menu ☰");
                        break;
                    }
                case ControllersManager.SupportedDevices.ValveIndex:
                    {
                        stringToChange = stringToChange.Replace(primRightButton, "Right A");
                        stringToChange = stringToChange.Replace(miniPrimRightButton, "A");
                        stringToChange = stringToChange.Replace(primLeftButton, "Left A");
                        break;
                    }
            }
            return stringToChange;
        }
        public string GetPrimRightButtonName()
        {
            return primRightButton;
        }
    }

}
