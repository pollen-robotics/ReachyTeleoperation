using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachy;

namespace TeleopReachy
{
    [ExecuteAlways]
    public class CustomPostProcessing : MonoBehaviour
    {
        public Material material;
        private UserMobilityInput userMobilityInput = null;
        private bool set = false;
        private bool activated = false;

        // void Start()
        // {
        //     // userMobilityInput = UserInputManager.Instance.UserMobilityInput;
        //     // userMobilityInput.event_DejaVu.AddListener(anime_lines);
        // }

        void anime_lines(bool ac)
        {
            activated = ac;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (userMobilityInput == null)
            {
                if (UserInputManager.Instance != null)
                {
                    userMobilityInput = UserInputManager.Instance.UserMobilityInput;
                    if (userMobilityInput != null)
                    {
                        if (!set)
                        {
                            userMobilityInput.event_DejaVu.AddListener(anime_lines);   
                            set = true;
                        }
                    }
                }
            }
            if (activated)
                Graphics.Blit(source, destination, material);
            else
                Graphics.Blit(source, destination);
        }
    }
}