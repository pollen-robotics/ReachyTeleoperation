using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Grpc.Core;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Reachy.Sdk.Mobility;
using Reachy.Sdk.Config;
namespace TeleopReachy
{
    public class EyeScript : MonoBehaviour
    {
        private bool needColorUpdate = false;

        Renderer rend;

        float alpha = 1.0f;

        public void SetImageTransparent()
        {
            alpha = 0.5f;
            needColorUpdate = true;
        }

        public void SetImageOpaque()
        {
            alpha = 1.0f;
            needColorUpdate = true;
        }

        void Update()
        {
            if (needColorUpdate)
            {
                rend = GetComponent<Renderer> ();
                Color color = new Color(1, 1, 1, alpha);
                rend.material.SetColor("_Color", color);
                needColorUpdate = false;
            }
        }
    }
}