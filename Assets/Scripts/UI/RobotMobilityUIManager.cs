using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Reachy;


namespace TeleopReachy
{
    public class RobotMobilityUIManager : MonoBehaviour
    {
        [SerializeField]
        private ReachyController reachyController;

        private UserMobilityInput userMobilityInput = null;
        private RobotStatus robotStatus;
        private RobotConfig robotConfig;
        private Transform userTracker;

        private ConnectionStatus connectionStatus;


        private Vector2 directionLeft;
        private Vector2 directionRight;

        private bool isStatic;
        private bool isRotating;

        private bool ShowMobilityUIListenerSet = false;
        private bool HideMobilityUIListenerSet = false;

        [SerializeField]
        private Transform indicator;

        [SerializeField]
        private Transform arrow;

        [SerializeField]
        private Transform arrowMobilityCommand;

        [SerializeField]
        private Transform circleMobilityCommand;

        [SerializeField]
        private Transform arrowRightRotationCommand;

        [SerializeField]
        private Transform arrowLeftRotationCommand;

        private void OnEnable()
        {
            EventManager.StartListening(EventNames.TeleoperationSceneLoaded, Init);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventNames.TeleoperationSceneLoaded, Init);
        }

        void Start()
        {
            connectionStatus = gRPCManager.Instance.ConnectionStatus;
            connectionStatus.event_OnConnectionStatusHasChanged.AddListener(Init);
        }

        private void Init()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotConfig = RobotDataManager.Instance.RobotConfig;

            UpdateMobilityUI(robotConfig.HasMobilePlatform());

            if (robotConfig.HasMobilePlatform())
                robotStatus.event_OnSwitchMobilityOn.AddListener(UpdateMobilityUI);
        }

        void UpdateMobilityUI(bool on)
        {
            if (!on)
            {
                ShowMobilityUIListenerSet = false;
                HideMobilityUIListenerSet = false;
                robotStatus.event_OnStartTeleoperation.RemoveListener(ShowMobilityUI);
                robotStatus.event_OnStopTeleoperation.RemoveListener(HideMobilityUI);
            }
            else
            {
                userMobilityInput = UserInputManager.Instance.UserMobilityInput;
                userTracker = UserTrackerManager.Instance.transform;

                if (ShowMobilityUIListenerSet == false)
                {
                    robotStatus.event_OnStartTeleoperation.AddListener(ShowMobilityUI);
                    ShowMobilityUIListenerSet = true;
                }
                if (HideMobilityUIListenerSet == false)
                {
                    robotStatus.event_OnStopTeleoperation.AddListener(HideMobilityUI);
                    HideMobilityUIListenerSet = true;
                }
            }

            HideMobilityUI();
        }

        void Update()
        {
            //not initializd yet.
            if (userMobilityInput == null)
                return;

            float orbita_yaw = -reachyController.headOrientation[2];
            if (orbita_yaw > 180)
            {
                orbita_yaw = orbita_yaw - 360;
            }
            float x_pos = Mathf.Abs(orbita_yaw * 4) < 360 ? orbita_yaw * 4 : (orbita_yaw > 0 ? 360 - Mathf.Abs(360 - Mathf.Abs(orbita_yaw * 4)) : -360 + Mathf.Abs(-360 + Mathf.Abs(orbita_yaw * 4)));
            arrow.parent.localPosition = new Vector3(-x_pos, 0, 0);

            arrow.localEulerAngles = new Vector3(0, 0, orbita_yaw);

            if (userMobilityInput.CanGetUserMobilityInputs())
            {
                directionLeft = userMobilityInput.GetMobileBaseDirection();
                directionRight = userMobilityInput.GetAngleDirection();

                float rotation = -directionRight[0];

                arrowRightRotationCommand.gameObject.SetActive(rotation < 0);
                arrowLeftRotationCommand.gameObject.SetActive(rotation > 0);


                if (directionLeft[0] == 0 && directionLeft[1] == 0)
                {
                    circleMobilityCommand.localEulerAngles = new Vector3(0, 0, orbita_yaw + 90);
                    IsRobotStatic(true);
                }
                else
                {
                    IsRobotStatic(false);
                    float phi = Mathf.Atan2(directionLeft[1], directionLeft[0]);
                    circleMobilityCommand.localEulerAngles = new Vector3(0, 0, orbita_yaw) + new Vector3(0, 0, Mathf.Rad2Deg * phi);
                }
            }
            else
            {
                IsRobotStatic(true);
            }
        }

        private void IsRobotStatic(bool isStatic)
        {
            arrowMobilityCommand.gameObject.SetActive(!isStatic);
        }

        public void HideMobilityUI()
        {
            transform.ActivateChildren(false);
        }

        private void ShowMobilityUI()
        {
            transform.ActivateChildren(true);
            IsRobotStatic(true);
            arrowRightRotationCommand.gameObject.SetActive(false);
            arrowLeftRotationCommand.gameObject.SetActive(false);
        }
    }
}