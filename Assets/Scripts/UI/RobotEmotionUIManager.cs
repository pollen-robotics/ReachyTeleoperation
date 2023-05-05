using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace TeleopReachy
{
    public class RobotEmotionUIManager : MonoBehaviour
    {
        private RobotJointCommands robotCommands;
        private RobotStatus robotStatus;
        private UserEmotionInput userEmotionInput;
        private ReachySimulatedCommands robotSimulatedCommands;

        void Awake()
        {
            //EventManager.StartListening(EventNames.TeleoperationSceneLoaded, Init);
            EventManager.StartListening(EventNames.MirrorSceneLoaded, Init);
        }

        private void Init()
        {
            robotCommands = RobotDataManager.Instance.RobotJointCommands;
            robotStatus = RobotDataManager.Instance.RobotStatus;
            userEmotionInput = UserInputManager.Instance.UserEmotionInput;
            robotCommands.event_OnEmotionOver.AddListener(RemoveEmotionShown);
            robotSimulatedCommands = ReachySimulatedManager.Instance.ReachySimulatedCommands;
            robotSimulatedCommands.event_OnEmotionOver.AddListener(RemoveEmotionShown);
        }

        public void ShowSelectedEmotion(Emotion emotion)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<RawImage>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
            transform.GetChild((int)emotion).GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
            // transform.GetChild((int)e.Emotion).GetChild(0).gameObject.SetActive(true);
        }

        public void HighlightSelectedEmotion(Emotion emotion)
        {
            HighlightNoEmotion();
            transform.GetChild((int)emotion).localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }


        public void HighlightNoEmotion()
        {
            foreach (Transform child in transform)
            {
                child.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        void RemoveEmotionShown(Emotion emotion)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
            }
            Debug.Log($"[RobotEmotionUIManager]: RemoveEmotionShown {emotion}");
            // transform.GetChild((int)e.Emotion).GetChild(0).gameObject.SetActive(false);
        }
    }
}