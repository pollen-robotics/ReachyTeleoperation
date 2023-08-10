using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Reachy.Sdk.Joint;

namespace TeleopReachy
{
    public class RobotStatePanelUIManager : MonoBehaviour
    {
        private gRPCDataController dataController;
        private ConnectionStatus connectionStatus;

        private RobotStatus robotStatus;

        private List<GameObject> motors;

        private bool isStatePanelStatusActive;

        void Awake()
        {
            if (Robot.IsCurrentRobotVirtual())
            {
                isStatePanelStatusActive = false;
                UpdateStatePanelStatus();
                return;
            }

            dataController = gRPCManager.Instance.gRPCDataController;
            dataController.event_OnStateUpdateTemperature.AddListener(UpdateTemperatures);

            connectionStatus = gRPCManager.Instance.ConnectionStatus;
            connectionStatus.event_OnConnectionStatusHasChanged.AddListener(CheckTemperatureInfo);

            motors = new List<GameObject>();
            foreach (Transform child in transform.GetChild(1))
            {
                motors.Add(child.gameObject);
            }

            CheckTemperatureInfo();

            isStatePanelStatusActive = true;
        }

        private void UpdateTemperatures(Dictionary<JointId, float> Temperatures)
        {
            /*foreach(KeyValuePair<JointId, float> motor in Temperatures)
            {
                string motorName = motor.Key.Name + "_temperature";
                GameObject currentMotor = motors.Find(m => m.name == motorName);
                currentMotor.transform.GetComponent<Text>().text = motor.Key.Name + ": " + Mathf.Round(motor.Value).ToString();
                if(motor.Value >= ErrorManager.THRESHOLD_ERROR_MOTOR_TEMPERATURE)
                {
                    currentMotor.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    if(motor.Value >= ErrorManager.THRESHOLD_WARNING_MOTOR_TEMPERATURE)
                    {
                        currentMotor.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else 
                    {
                        currentMotor.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }  */  
        }

        private void CheckTemperatureInfo()
        {
            if(connectionStatus.AreRobotServicesRestarting())
            {
                transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "Waiting for temperatures...";
                transform.GetChild(2).GetChild(1).GetComponent<Text>().color = ColorsManager.blue;
                isStatePanelStatusActive = true;
            }
            else
            {
                if(!connectionStatus.IsServerConnected() || !connectionStatus.IsRobotInDataRoom())
                {
                    transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "No temperature information";
                    transform.GetChild(2).GetChild(1).GetComponent<Text>().color = ColorsManager.red;
                    isStatePanelStatusActive = true;
                }
                else
                {
                    isStatePanelStatusActive = false;
                }
            }
            UpdateStatePanelStatus();
        }

        private void UpdateStatePanelStatus()
        {
            transform.GetChild(2).gameObject.SetActive(isStatePanelStatusActive);
        }
    }
}
