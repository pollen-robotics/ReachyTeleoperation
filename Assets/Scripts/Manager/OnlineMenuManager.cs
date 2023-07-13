using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace TeleopReachy
{
    public class OnlineMenuManager : MonoBehaviour
    {
        private enum OnlineMenuItem
        {
            GraspingLock, Cancel, Happy, Confused, Sad, Angry
        }

        public ControllersManager controllers;

        public Transform Headset;

        public Transform CancelIcon;
        public Transform GraspingLockIcon;

        private RobotStatus robotStatus;
        private RobotConfig robotConfig;

        private OnlineMenuItem selectedItem;
        private Emotion selectedEmotion;

        private bool isOnlineMenuOpen;

        private bool leftPrimaryButtonPreviouslyPressed;

        private Vector2 rightJoystickDirection;

        private int nbEnum;

        public UnityEvent<Emotion> event_OnAskEmotion;

        public UnityEvent<Emotion> event_OnEmotionSelected;

        public UnityEvent event_OnNoEmotionSelected;

        private void OnEnable()
        {
            EventManager.StartListening(EventNames.MirrorSceneLoaded, Init);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventNames.MirrorSceneLoaded, Init);
        }

        private void Init()
        {
            Headset = HeadsetPermanenTrackerWorldManager.Instance.transform;

            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotStatus.event_OnStopTeleoperation.AddListener(HideOnlineMenu);
            robotStatus.SetGraspingLockActivated(false, false);
            robotConfig = RobotDataManager.Instance.RobotConfig;

        }

        // Start is called before the first frame update
        void Start()
        {
            HideOnlineMenu();

            selectedItem = OnlineMenuItem.Cancel;
            selectedEmotion = Emotion.NoEmotion;

            nbEnum = Enum.GetNames(typeof(OnlineMenuItem)).Length;
        }

        // Update is called once per frame
        void Update()
        {
            bool leftPrimaryButtonPressed = false;
            bool rightPrimaryButtonPressed;

            if (robotStatus != null && !robotStatus.AreRobotMovementsSuspended())
            {
                if (controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out rightPrimaryButtonPressed) && !rightPrimaryButtonPressed)
                {
                    if (controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftPrimaryButtonPressed) && !leftPrimaryButtonPreviouslyPressed)
                    {
                        selectedItem = OnlineMenuItem.Cancel;
                    }

                    if (controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftPrimaryButtonPressed) && leftPrimaryButtonPreviouslyPressed)
                    {
                        if (!isOnlineMenuOpen)
                        {
                            ShowOnlineMenu();
                        }
                        transform.rotation = Headset.rotation;

                        controllers.rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightJoystickDirection);

                        float r = Mathf.Sqrt(Mathf.Pow(rightJoystickDirection[0], 2) + Mathf.Pow(rightJoystickDirection[1], 2));
                        if (r < 0.5)
                        {
                            selectedItem = OnlineMenuItem.Cancel;
                        }
                        else
                        {
                            float phi = Mathf.Atan2(rightJoystickDirection[1], rightJoystickDirection[0]);

                            if (Maths.isApproxEqual(phi, -2f, 0.2f))
                                selectedItem = OnlineMenuItem.GraspingLock;
                            else if (Maths.isApproxEqual(phi, 0f, 0.5f))
                                selectedItem = OnlineMenuItem.Angry;
                            else if (Maths.isApproxEqual(phi, 1f, 0.5f))
                                selectedItem = OnlineMenuItem.Sad;
                            else if (Maths.isApproxEqual(phi, 2f, 0.5f))
                                selectedItem = OnlineMenuItem.Confused;
                            else if (Maths.isApproxEqual(phi, 3.14f, 0.5f))
                                selectedItem = OnlineMenuItem.Happy;
                        }
                        if (robotConfig.IsVirtual() || robotConfig.HasHead())
                        {
                            switch (selectedItem)
                            {
                                case OnlineMenuItem.Angry:
                                    {
                                        if (selectedEmotion != Emotion.Angry)
                                        {
                                            event_OnEmotionSelected.Invoke(Emotion.Angry);
                                        }
                                        selectedEmotion = Emotion.Angry;
                                        RemoveHighlightCancel();
                                        RemoveHighlightGraspingLock();
                                        break;
                                    }
                                case OnlineMenuItem.Sad:
                                    {
                                        if (selectedEmotion != Emotion.Sad)
                                        {
                                            event_OnEmotionSelected.Invoke(Emotion.Sad);
                                        }
                                        selectedEmotion = Emotion.Sad;
                                        RemoveHighlightCancel();
                                        RemoveHighlightGraspingLock();
                                        break;
                                    }
                                case OnlineMenuItem.Confused:
                                    {
                                        if (selectedEmotion != Emotion.Confused)
                                        {
                                            event_OnEmotionSelected.Invoke(Emotion.Confused);
                                        }
                                        selectedEmotion = Emotion.Confused;
                                        RemoveHighlightCancel();
                                        RemoveHighlightGraspingLock();
                                        break;
                                    }
                                case OnlineMenuItem.Happy:
                                    {
                                        if (selectedEmotion != Emotion.Happy)
                                        {
                                            event_OnEmotionSelected.Invoke(Emotion.Happy);
                                        }
                                        selectedEmotion = Emotion.Happy;
                                        RemoveHighlightCancel();
                                        RemoveHighlightGraspingLock();
                                        break;
                                    }
                                case OnlineMenuItem.Cancel:
                                    {
                                        event_OnNoEmotionSelected.Invoke();
                                        selectedEmotion = Emotion.NoEmotion;
                                        RemoveHighlightGraspingLock();
                                        HighlightCancel();
                                        break;
                                    }
                                case OnlineMenuItem.GraspingLock:
                                    {
                                        event_OnNoEmotionSelected.Invoke();
                                        selectedEmotion = Emotion.NoEmotion;
                                        HighlightGraspingLock();
                                        RemoveHighlightCancel();
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        if (isOnlineMenuOpen) HideOnlineMenu();
                    }

                    if (controllers.leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftPrimaryButtonPressed) && !leftPrimaryButtonPressed && leftPrimaryButtonPreviouslyPressed)
                    {
                        switch (selectedItem)
                        {
                            case OnlineMenuItem.Happy:
                            case OnlineMenuItem.Angry:
                            case OnlineMenuItem.Confused:
                            case OnlineMenuItem.Sad:
                                {
                                    if (!robotStatus.IsEmotionPlaying())
                                    {
                                        event_OnAskEmotion.Invoke(selectedEmotion);
                                    }
                                    break;
                                }
                            case OnlineMenuItem.GraspingLock:
                                {
                                    LockGrasp();
                                    break;
                                }
                        }
                    }

                    leftPrimaryButtonPreviouslyPressed = leftPrimaryButtonPressed;
                }
            }
        }

        void HighlightCancel()
        {
            CancelIcon.localScale = new Vector3(1.5f * 0.3f, 1.5f * 0.3f, 1.5f);
        }

        void RemoveHighlightCancel()
        {
            CancelIcon.localScale = new Vector3(1.0f * 0.3f, 1.0f * 0.3f, 1.0f);
        }

        void HighlightGraspingLock()
        {
            GraspingLockIcon.localScale = new Vector3(1.5f * 0.3f, 1.5f * 0.3f, 1.5f);
        }

        void RemoveHighlightGraspingLock()
        {
            GraspingLockIcon.localScale = new Vector3(1.0f * 0.3f, 1.0f * 0.3f, 1.0f);
        }

        void LockGrasp()
        {
            robotStatus.SetGraspingLockActivated(!robotStatus.IsGraspingLockActivated());
        }

        void ShowOnlineMenu()
        {
            if (robotConfig.IsVirtual() || robotConfig.HasHead())
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }

            bool locked = robotStatus.IsGraspingLockActivated();
            GraspingLockIcon.GetChild(0).gameObject.SetActive(locked);
            GraspingLockIcon.GetChild(1).gameObject.SetActive(!locked);
            isOnlineMenuOpen = true;
        }

        void HideOnlineMenu()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            isOnlineMenuOpen = false;
        }
    }
}
