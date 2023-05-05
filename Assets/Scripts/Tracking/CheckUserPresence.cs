using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TeleopReachy
{
    public class CheckUserPresence : MonoBehaviour
    {
        private InputDevice headDevice;

        private bool wasUserPresent;

        // Start is called before the first frame update
        void Start()
        {
            headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            wasUserPresent = true;
        }

        void Update()
        {
            if (headDevice.isValid)
            {
                bool userPresent = false;
                bool presenceFeatureSupported = headDevice.TryGetFeatureValue(CommonUsages.userPresence, out userPresent);
                if (presenceFeatureSupported)
                {
                    if (!userPresent && wasUserPresent) 
                    {
                        EventManager.TriggerEvent(EventNames.HeadsetRemoved);
                    }
                    if (userPresent && !wasUserPresent) 
                    {
                        EventManager.TriggerEvent(EventNames.HeadsetReset);
                    }
                    wasUserPresent = userPresent;
                }
            }
        }
    }
}
