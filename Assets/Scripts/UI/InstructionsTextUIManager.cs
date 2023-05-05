using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TeleopReachy
{
    public class InstructionsTextUIManager : MonoBehaviour
    {
        private Text instructions;

        [SerializeField]
        private Text instructionsDetails;

        private ConnectionStatus connectionStatus;
        private TransitionRoomManager transitionRoomManager;

        private bool needUpdateText;

        private string instructionsText;
        private string instructionsDetailsText;

        // Start is called before the first frame update
        void Start()
        {
            instructions = transform.GetComponent<Text>();

            transitionRoomManager = TransitionRoomManager.Instance;

            if (Robot.IsCurrentRobotVirtual())
            {
                instructionsText = "Have fun with virtual Reachy!";
            }
            else
            {
                instructionsText = "Please face the mirror then press Ready";
                instructionsDetailsText = "";
                transitionRoomManager.event_OnReadyForTeleop.AddListener(IndicateToPressA);
                transitionRoomManager.event_OnAbortTeleop.AddListener(IndicateRobotNotReady);
            }

            needUpdateText = true;
        }

        void Update()
        {
            if (needUpdateText)
            {
                instructions.text = instructionsText;
                if (instructionsDetails != null) instructionsDetails.text = instructionsDetailsText;
                needUpdateText = false;
            }
        }

        void IndicateToPressA()
        {
            instructionsText = "Press and hold A to start";
            instructionsDetailsText = "";
            needUpdateText = true;
        }

        void IndicateRobotNotReady()
        {
            instructionsText = "Reachy is not ready for teleop.";
            instructionsDetailsText = "Some required services are missing. Check the status panel for more details!";
            needUpdateText = true;
        }
    }
}

