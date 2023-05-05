using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


namespace TeleopReachy
{
    public class RobotButtonManager : Selectable
    {
        private Robot reachy;

        static RobotButtonManager activeRobotButton;

        public UnityEvent<RobotButtonInfo> event_OnDeletionRequested;
        public UnityEvent<RobotButtonInfo> event_OnModificationRequested;
        public UnityEvent<Robot> event_OnSelectedRobotButtonChanged;

        public Text textUID;
        public Text textIP;

        public GameObject modify_button;
        public GameObject delete_button;
        public GameObject raw_image;

        public GameObject imageSelected;

        public void SetRobot(Robot robot)
        {
            reachy = robot;
            textUID.text = reachy.uid;
            textIP.text = reachy.ip;
        }

        public void SelectRobotButton()
        {
            if (activeRobotButton != null) activeRobotButton.transform.GetChild(7).gameObject.SetActive(false);
            imageSelected.SetActive(true);
            activeRobotButton = gameObject.GetComponent<RobotButtonManager>();
            event_OnSelectedRobotButtonChanged.Invoke(reachy);
        }

        public RobotButtonManager GetSelectedRobotButton()
        {
            return activeRobotButton;
        }

        public Robot GetRobot()
        {
            return reachy;
        }

        public void DeleteRobotButton()
        {
            reachy = null;
            GameObject.Destroy(this.gameObject);
        }

        public void RequestButtonDeletion()
        {
            event_OnDeletionRequested.Invoke(new RobotButtonInfo(reachy, gameObject));
        }

        public void RequestButtonModification()
        {
            event_OnModificationRequested.Invoke(new RobotButtonInfo(reachy, gameObject));
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            ActiveControllerManager.Instance.ControllersVibrations.OnUIEnterVibration();
        }

        public void DisplayModifItems()
        {
            if (!reachy.IsVirtualRobot())
            {
                modify_button.SetActive(true);
                delete_button.SetActive(true);
            }
            raw_image.SetActive(true);
        }

        public void HideModifItems()
        {
            if (!reachy.IsVirtualRobot())
            {
                modify_button.SetActive(false);
                delete_button.SetActive(false);
            }
            raw_image.SetActive(false);
        }
    }

    public struct RobotButtonInfo
    {
        public RobotButtonInfo(Robot reachy, GameObject robotbutton)
        {
            robot = reachy;
            button = robotbutton;
        }

        public Robot robot { get; }
        public GameObject button { get; }
    }
}