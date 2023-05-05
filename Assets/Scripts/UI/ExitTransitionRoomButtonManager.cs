using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class ExitTransitionRoomButtonManager : MonoBehaviour
    {
        [SerializeField]
        private Transform buttonsPanel;

        [SerializeField]
        private Transform validationText;

        [SerializeField]
        private Transform usualText;

        [SerializeField]
        private Transform usualIcon;

        private Vector3 closedButtonsPosition = new Vector3(-188, -130, 0);
        private Vector3 openButtonsPosition = new Vector3(-64, -130, 0);

        private bool needUpdateButtons;

        private float _timeElapsed;

        private Vector3 lerpStartingPosition;
        private Vector3 lerpGoalPosition;


        void Update()
        {
            if(needUpdateButtons)
            {
                _timeElapsed += Time.deltaTime;
                if(_timeElapsed >= 0.3f)
                {
                    _timeElapsed = 0;
                    buttonsPanel.localPosition = lerpGoalPosition;
                    needUpdateButtons = false;
                }
                else
                {
                    float fTime = _timeElapsed / 0.3f;
                    buttonsPanel.localPosition = Vector3.Lerp(lerpStartingPosition, lerpGoalPosition, fTime);
                }
            }
        }

        public void ShowValidationButtons()
        {
            lerpStartingPosition = buttonsPanel.localPosition;
            _timeElapsed = 0;
            lerpGoalPosition = openButtonsPosition;
            needUpdateButtons = true;

            ActivateValidationText(true);
        }

        public void HideValidationButtons()
        {
            lerpStartingPosition = buttonsPanel.localPosition;
            _timeElapsed = 0;
            lerpGoalPosition = closedButtonsPosition;
            needUpdateButtons = true;

            ActivateValidationText(false);
        }

        private void ActivateValidationText(bool activate)
        {
            validationText.GetComponent<Text>().enabled = activate;
            usualIcon.GetComponent<RawImage>().enabled = !activate;
            usualText.GetComponent<Text>().enabled = !activate;
        }
    }
}