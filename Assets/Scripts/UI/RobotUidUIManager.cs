using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class RobotUidUIManager : MonoBehaviour
    {
        void Start()
        {        
            transform.GetComponent<Text>().text = "Robot IP address: " + PlayerPrefs.GetString("robot_ip");
        }
    }
}