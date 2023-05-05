using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class UserSizeSliderManager : MonoBehaviour
    {
        [SerializeField]
        private Slider userSizeSlider;

        public void UpdateUserSize()
        {
            UserSize.Instance.UpdateUserSize(userSizeSlider.value);
        }

        public void IncrementValue()
        {
            userSizeSlider.value += 0.01f;
            if (userSizeSlider.value > userSizeSlider.maxValue)
                userSizeSlider.value = userSizeSlider.maxValue;
            UserSize.Instance.UpdateUserSize(userSizeSlider.value);
        }

        public void DecrementValue()
        {
            userSizeSlider.value -= 0.01f;
            if (userSizeSlider.value < userSizeSlider.minValue)
                userSizeSlider.value = userSizeSlider.minValue;
            UserSize.Instance.UpdateUserSize(userSizeSlider.value);
        }
    }
}

