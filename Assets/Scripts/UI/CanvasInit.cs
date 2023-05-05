using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeleopReachyXR
{
    public class CanvasInit : MonoBehaviour
    {
        [SerializeField]
        private float PlaneDistance;

        // Start is called before the first frame update
        void Start()
        {
            // Assigne la caméra de Basescene au canva courant
            transform.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            transform.GetComponent<Canvas>().worldCamera = Camera.main;

            if(PlaneDistance != 0) transform.GetComponent<Canvas>().planeDistance = PlaneDistance;
        }

    }
}
