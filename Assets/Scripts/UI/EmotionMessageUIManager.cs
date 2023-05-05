using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class EmotionMessageUIManager : MonoBehaviour
    {
        private RobotStatus robotStatus;

        private Coroutine limitDisplayInTime;

        [SerializeField]
        private Text infoMessage;

        private OnlineMenuManager onlineMenuManager;

        [SerializeField]
        private Texture sadImage;

        [SerializeField]
        private Texture happyImage;

        [SerializeField]
        private Texture angryImage;

        [SerializeField]
        private Texture confusedImage;

        [SerializeField]
        private RawImage image;



        private Dictionary<Emotion, Texture> emotionImages;

        void Start()
        {
            transform.ActivateChildren(false);
            emotionImages = new Dictionary<Emotion, Texture>();
            emotionImages.Add(Emotion.Sad, sadImage);
            emotionImages.Add(Emotion.Happy, happyImage);
            emotionImages.Add(Emotion.Angry, angryImage);
            emotionImages.Add(Emotion.Confused, confusedImage);
            EventManager.StartListening(EventNames.MirrorSceneLoaded, Init);
        }

        void Init()
        {
            onlineMenuManager = GameObject.Find("CanvaOnlineMenu").GetComponent<OnlineMenuManager>();
            onlineMenuManager.event_OnAskEmotion.AddListener(ShowMessage);
        }


        void ShowMessage(Emotion emotion)
        {
            switch (emotion)
            {
                case Emotion.Sad:
                    infoMessage.text = "Emotion sad is playing";
                    image.texture = emotionImages[Emotion.Sad];
                    break;

                case Emotion.Happy:
                    infoMessage.text = "Emotion happy is playing";
                    image.texture = emotionImages[Emotion.Happy];
                    break;

                case Emotion.Angry:
                    infoMessage.text = "Emotion angry is playing";
                    image.texture = emotionImages[Emotion.Angry];
                    break;

                case Emotion.Confused:
                    infoMessage.text = "Emotion confused is playing";
                    image.texture = emotionImages[Emotion.Confused];
                    break;
            }

            if (limitDisplayInTime != null) StopCoroutine(limitDisplayInTime);
            limitDisplayInTime = StartCoroutine(DisplayLimitedInTime());
        }

        IEnumerator DisplayLimitedInTime()
        {
            transform.ActivateChildren(true);
            yield return new WaitForSeconds(3);
            transform.ActivateChildren(false);
        }
    }
}