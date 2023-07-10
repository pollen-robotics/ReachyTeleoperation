using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TeleopReachy
{
    public class HeadsetRemovedInMirrorManager : MonoBehaviour
    {
        [SerializeField]
        private Transform headsetRemovedMessage;

        private bool wasUserPresent;

        // Start is called before the first frame update
        void Start()
        {
            headsetRemovedMessage.gameObject.SetActive(false);
            EventManager.StartListening(EventNames.HeadsetReset, ShowResetPosition);
        }

        void ShowResetPosition()
        {
            headsetRemovedMessage.gameObject.SetActive(true);
        }

        public void HideResetPosition()
        {
            headsetRemovedMessage.gameObject.SetActive(false);
        }
    }
}
