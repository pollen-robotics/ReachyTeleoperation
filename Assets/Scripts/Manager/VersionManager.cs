using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Grpc.Core;
using Reachy.Sdk.Joint;
using Reachy;

namespace TeleopReachy
{
    public class VersionManager : MonoBehaviour
    {
        public Text versionText;
        public string introText;

        void Start()
        {
            versionText.text = introText + Application.version;
        }
    }
}