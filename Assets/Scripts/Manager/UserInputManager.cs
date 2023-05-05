using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeleopReachy
{
    public class UserInputManager : Singleton<UserInputManager>
    {
        public UserMovementsInput UserMovementsInput { get; private set; }
        public UserEmotionInput UserEmotionInput { get; private set; }
        public UserMobilityInput UserMobilityInput { get; private set; }

        protected override void Init()
        {
            UserMovementsInput = GetComponent<UserMovementsInput>();
            UserEmotionInput = GetComponent<UserEmotionInput>();
            UserMobilityInput = GetComponent<UserMobilityInput>();
        }
    }
}

