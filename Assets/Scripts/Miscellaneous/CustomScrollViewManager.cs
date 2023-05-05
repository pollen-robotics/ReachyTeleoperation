using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;


namespace TeleopReachy
{
    public class CustomScrollViewManager : MonoBehaviour
    {

        private CustomHandedInputSelector inputSelector;
        
        void Start()
        {
            inputSelector = CustomHandedInputSelector.Instance;
        }

        void Update()
        {
            if(inputSelector != null)
            {
                Vector2 joystickInput;
                float scrollValue;
                inputSelector.GetActiveController().TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out joystickInput);

                scrollValue = joystickInput[1];

                if(scrollValue != 0)
                {
                    OnScroll(new PointerEventData(EventSystem.current), scrollValue);
                }
            }
        }

        void OnScroll(PointerEventData eventData, float value)
        {
            eventData.delta = new Vector2(0.0f, value*2);
            eventData.scrollDelta = new Vector2(0.0f, value*2);
            transform.GetComponent<ScrollRect>().OnBeginDrag(eventData);
            transform.GetComponent<ScrollRect>().OnScroll(eventData);
        }

        void OnStopScrolling(PointerEventData eventData)
        {
            eventData.delta = new Vector2(0.0f, 0.0f);
            eventData.scrollDelta = new Vector2(0.0f, 0.0f);
            transform.GetComponent<ScrollRect>().OnEndDrag(eventData);
        }
    }
}