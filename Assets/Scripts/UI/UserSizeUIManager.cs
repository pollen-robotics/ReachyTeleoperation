using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class UserSizeUIManager : MonoBehaviour
    {
        [SerializeField]
        private Text valueTextMeters;

        //[SerializeField]
        //private Text valueTextFeet;

        public void UpdateValue(float value)
        {
            valueTextMeters.text = "Size: " + (int)(Math.Round(value * 100)) + " cm";
            //valueTextFeet.text = (value * 3.28f).ToString("F2") + "ft";
        }
    }
}
