using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;


namespace TeleopReachy
{
    public class RobotButtonModify : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            ActiveControllerManager.Instance.ControllersVibrations.OnUIEnterVibration();
        }
    }
}
