using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;


namespace TeleopReachy
{
    public class ControllersAInputUIManager : MonoBehaviour
    {
        private ControllersManager.SupportedDevices currentDevice;

        private ControllersManager controllersManager;

        [SerializeField]
        private Texture oculusAInputImage; 
        
        [SerializeField]
        private Texture viveAInputImage;

        [SerializeField]
        private Texture valveAInputImage;

        private List<Texture> controllersImages;

        private void OnEnable()
        {
            controllersManager = ControllersManager.Instance;
            controllersImages = new List<Texture> {oculusAInputImage, viveAInputImage, valveAInputImage}; 
            transform.GetComponent<RawImage>().texture = controllersImages[(int)controllersManager.controllerDeviceType];
        }
    }
}
