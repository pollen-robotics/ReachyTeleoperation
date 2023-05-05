using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;


namespace TeleopReachy
{
    public class CanvaValidateLeaveOnLockedPositionManager : MonoBehaviour
    {
        [SerializeField]
        private Button validateLeaveRoomButton;

        [SerializeField]
        private Button cancelLeaveRoomButton;

        [SerializeField]
        private Transform loader;

        [SerializeField]
        private GameObject beforeValidateElements;

        [SerializeField]
        private GameObject afterValidateElements;

        [SerializeField]
        private ExitTransitionRoomButtonManager exitButtonsManager;

        private RobotStatus robotStatus;

        Coroutine rotateLoader;

        void Awake()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;

            validateLeaveRoomButton.onClick.AddListener(QuitTransitionRoom);
            cancelLeaveRoomButton.onClick.AddListener(Cancel);
        }

        void OnDestroy()
        {
            if (rotateLoader != null) StopCoroutine(rotateLoader);
        }

        void QuitTransitionRoom()
        {
            robotStatus.SetLeftArmOn(false);
            robotStatus.SetRightArmOn(false);
            robotStatus.SetHeadOn(false);
            robotStatus.TurnRobotSmoothlyCompliant();
            beforeValidateElements.SetActive(false);
            afterValidateElements.SetActive(true);
            rotateLoader = StartCoroutine(RotateLoader(3));
            TransitionRoomManager.Instance.BackToConnectionScene();
        }

        private IEnumerator RotateLoader(float duration)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                loader.transform.Rotate(0, 0, -7, Space.Self);
                yield return null;
            }
        }

        void Cancel()
        {
            transform.ActivateChildren(false);
            exitButtonsManager.HideValidationButtons();
        }
    }
}