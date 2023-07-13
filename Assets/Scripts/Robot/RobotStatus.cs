using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TeleopReachy
{
    public class RobotStatus : MonoBehaviour
    {
        private bool isRobotTeleoperationActive;

        private bool areRobotMovementsSuspended;

        private bool isRobotCompliant;

        private bool isMobilityInCloseLoop; // true if mobile base in close-loop, false if mobile base in idle

        private bool isMobilityInBreakMode;

        private bool isMobilityOn; // true if operator want to have control of the mobile base, false otherwise

        private bool isLeftArmOn = true; // true if operator want to have control of the left arm, false otherwise

        private bool isRightArmOn = true; // true if operator want to have control of the right arm, false otherwise

        private bool isHeadOn = true; // true if operator want to have control of the head, false otherwise

        private bool isMobilityActive; // true if panel must be shown, false otherwise

        private bool areEmotionsActive;

        private bool isEmotionPlaying;

        private bool statusChanged;

        private bool hasMotorsSpeedLimited;

        private bool isGraspingLockActivated;

        private bool isLeftGripperClosed = false;
        private bool isRightGripperClosed = false;

        public bool IsRobotPositionLocked { get; private set; }

        public UnityEvent event_OnStartTeleoperation;
        public UnityEvent event_OnStopTeleoperation;
        public UnityEvent event_OnSuspendTeleoperation;
        public UnityEvent event_OnResumeTeleoperation;

        public UnityEvent event_OnInitializeRobotStateRequested;
        public UnityEvent event_OnRobotStiffRequested;
        public UnityEvent<bool> event_OnGraspingLock;
        public UnityEvent event_OnRobotSmoothlyCompliantRequested;
        public UnityEvent event_OnRobotCompliantRequested;
        public UnityEvent event_OnRobotFullyCompliant;

        public UnityEvent<bool> event_OnSwitchMobilityOn;

        public void LeftGripperClosed(bool isclosed)
        {
            isLeftGripperClosed = isclosed;
        }

        public void RightGripperClosed(bool isclosed)
        {
            isRightGripperClosed = isclosed;
        }

        public bool IsRightGripperClosed()
        {
            return isRightGripperClosed;
        }

        public bool IsLeftGripperClosed()
        {
            return isLeftGripperClosed;
        }

        public bool IsRobotTeleoperationActive()
        {
            return isRobotTeleoperationActive;
        }

        public bool AreRobotMovementsSuspended()
        {
            return areRobotMovementsSuspended;
        }

        public bool IsRobotCompliant()
        {
            return isRobotCompliant;
        }

        public bool AreEmotionsActive()
        {
            return areEmotionsActive;
        }

        public bool IsMobilityOn()
        {
            return isMobilityOn;
        }

        public bool IsLeftArmOn()
        {
            return isLeftArmOn;
        }

        public bool IsRightArmOn()
        {
            return isRightArmOn;
        }

        public bool IsHeadOn()
        {
            return isHeadOn;
        }

        public bool IsMobilityInCloseLoop()
        {
            return isMobilityInCloseLoop;
        }

        public bool IsMobilityInBreakMode()
        {
            return isMobilityInBreakMode;
        }

        public bool IsEmotionPlaying()
        {
            return isEmotionPlaying;
        }

        public bool IsMobilityActive()
        {
            return isMobilityActive;
        }

        public bool HasMotorsSpeedLimited()
        {
            return hasMotorsSpeedLimited;
        }

        public bool IsGraspingLockActivated()
        {
            return isGraspingLockActivated;
        }

        public void SetGraspingLockActivated(bool isActivated, bool displayPopup = true)
        {
            isGraspingLockActivated = isActivated;
            if (displayPopup)
                event_OnGraspingLock.Invoke(isActivated);
        }

        public void SetEmotionPlaying(bool isPlaying)
        {
            isEmotionPlaying = isPlaying;
        }

        public void SetMobilityActive(bool isActive)
        {
            isMobilityActive = isActive;
        }

        public void SetMobilityInBreakMode(bool inBreakMode)
        {
            isMobilityInBreakMode = inBreakMode;
        }

        public void SetEmotionsActive(bool isActive)
        {
            areEmotionsActive = isActive;
        }

        public void SetMobilityOn(bool isOn)
        {
            isMobilityOn = isOn;
            event_OnSwitchMobilityOn.Invoke(isOn);
        }

        public void SetLeftArmOn(bool isOn)
        {
            isLeftArmOn = isOn;
        }

        public void SetRightArmOn(bool isOn)
        {
            isRightArmOn = isOn;
        }

        public void SetHeadOn(bool isOn)
        {
            isHeadOn = isOn;
        }

        public void SetMobilityInCloseLoop(bool isCloseLoop)
        {
            isMobilityInCloseLoop = isCloseLoop;
        }

        public void InitializeRobotState()
        {
            event_OnInitializeRobotStateRequested.Invoke();
        }

        public void LockRobotPosition()
        {
            IsRobotPositionLocked = true;
        }

        public void StartRobotTeleoperation()
        {
            Debug.Log("[RobotStatus]: Start teleoperation");
            isRobotTeleoperationActive = true;
            IsRobotPositionLocked = false;
            event_OnStartTeleoperation.Invoke();
        }

        public void StopRobotTeleoperation()
        {
            Debug.Log("[RobotStatus]: Stop teleoperation");
            isRobotTeleoperationActive = false;
            event_OnStopTeleoperation.Invoke();
        }

        public void SetMotorsSpeedLimited(bool isLimited)
        {
            hasMotorsSpeedLimited = isLimited;
        }

        public void SetRobotCompliant(bool isCompliant)
        {
            isRobotCompliant = isCompliant;
            if (isRobotCompliant)
            {
                event_OnRobotFullyCompliant.Invoke();
            }
        }

        public void SuspendRobotTeleoperation()
        {
            areRobotMovementsSuspended = true;
            event_OnSuspendTeleoperation.Invoke();
        }

        public void ResumeRobotTeleoperation()
        {
            areRobotMovementsSuspended = false;
            event_OnResumeTeleoperation.Invoke();
        }

        public void TurnRobotStiff()
        {
            Debug.Log("[RobotStatus]: Turn Robot Stiff");
            event_OnRobotStiffRequested.Invoke();
        }

        public void TurnRobotCompliant()
        {
            Debug.Log("[RobotStatus]: Turn Robot Compliant");
            event_OnRobotCompliantRequested.Invoke();
        }

        public void TurnRobotSmoothlyCompliant()
        {
            Debug.Log("[RobotStatus]: Turn Robot Smoothly Compliant");
            event_OnRobotSmoothlyCompliantRequested.Invoke();
        }

        public override string ToString()
        {
            return string.Format(@"isRobotTeleoperationActive = {0},
             areRobotMovementsSuspended= {1},
             isRobotCompliant= {2},
             isMobilityInCloseLoop= {3},
             isMobilityInBreakMode= {4},
             isMobilityOn= {5},
             isLeftArmOn= {6},
             isRightArmOn= {7},
             isHeadOn= {8},
             isMobilityActive= {9},
             areEmotionsActive= {10},
             isEmotionPlaying= {11},
             statusChanged= {12},
             hasMotorsSpeedLimited= {13}",
             isRobotTeleoperationActive, areRobotMovementsSuspended, isRobotCompliant,
              isMobilityInCloseLoop, isMobilityInBreakMode, isMobilityOn, isLeftArmOn, isRightArmOn, isHeadOn,
               isMobilityActive, areEmotionsActive, isEmotionPlaying, statusChanged, isGraspingLockActivated);
        }
    }
}