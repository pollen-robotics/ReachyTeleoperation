using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeleopReachy
{
    public enum Emotion
    {
        Happy, Sad, Confused, Angry, NoEmotion
    }

    public class UserEmotionInput : MonoBehaviour
    {
        public OnlineMenuManager onlineMenuManager;

        public ControllersManager controllers;

        private RobotConfig robotConfig;
        private RobotJointCommands robotCommands;

        private ReachySimulatedCommands robotSimulatedCommands;
        private RobotStatus robotStatus;


        private void OnEnable()
        {
            /*EventManager.StartListening(EventNames.TeleoperationSceneLoaded, Init);*/
            EventManager.StartListening(EventNames.MirrorSceneLoaded, Init);
        }

        private void OnDisable()
        {
            /*EventManager.StopListening(EventNames.TeleoperationSceneLoaded, Init);*/
            EventManager.StopListening(EventNames.MirrorSceneLoaded, Init);
        }

        private void Init()
        {
            robotCommands = RobotDataManager.Instance.RobotJointCommands;
            robotCommands.event_OnEmotionOver.AddListener(EmotionIsOver);
            robotSimulatedCommands = ReachySimulatedManager.Instance.ReachySimulatedCommands;
            robotSimulatedCommands.event_OnEmotionOver.AddListener(EmotionIsOver);
            robotStatus = RobotDataManager.Instance.RobotStatus;
            robotConfig = RobotDataManager.Instance.RobotConfig;
            onlineMenuManager.event_OnAskEmotion.AddListener(AskToPlayEmotion);
        }


        private void AskToPlayEmotion(Emotion emotion)
        {
            if (robotConfig.HasHead() && robotStatus.IsRobotTeleoperationActive() && robotStatus.AreEmotionsActive() && !robotStatus.IsEmotionPlaying() && !robotStatus.AreRobotMovementsSuspended())
            {
                robotStatus.SetEmotionPlaying(true);
                switch (emotion)
                {
                    case Emotion.Sad:
                        robotCommands.ReachySad();
                        break;

                    case Emotion.Happy:
                        robotCommands.ReachyHappy();
                        break;

                    case Emotion.Angry:
                        robotCommands.ReachyAngry();
                        break;

                    case Emotion.Confused:
                        robotCommands.ReachyConfused();
                        break;
                }
            }
            else
            {
                robotStatus.SetEmotionPlaying(true);
                switch (emotion)
                {
                    case Emotion.Sad:
                        robotSimulatedCommands.ReachySad();
                        break;

                    case Emotion.Happy:
                        robotSimulatedCommands.ReachyHappy();
                        break;
                    case Emotion.Angry:
                        robotSimulatedCommands.ReachyAngry();
                        break;

                    case Emotion.Confused:
                        robotSimulatedCommands.ReachyConfused();
                        break;
                }
            }
        }

        public void EmotionIsOver(Emotion emotion)
        {
            robotStatus.SetEmotionPlaying(false);
        }
    }
}