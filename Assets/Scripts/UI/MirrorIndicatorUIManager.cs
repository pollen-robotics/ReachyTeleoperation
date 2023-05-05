using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace TeleopReachy
{ 
    public class MirrorIndicatorUIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform lockPositionIndicator;

        private RobotStatus robotStatus;


        void Start()
        {
            robotStatus = RobotDataManager.Instance.RobotStatus;
            TransitionRoomManager.Instance.event_OnReadyForTeleop.AddListener(ShowMirrorIndicator);
            transform.ActivateChildren(false);
        }

        void ShowMirrorIndicator()
        {
            transform.ActivateChildren(true);
            lockPositionIndicator.gameObject.SetActive(robotStatus.IsRobotPositionLocked);
        }

    }
}

