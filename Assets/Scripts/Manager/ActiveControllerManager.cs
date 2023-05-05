using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeleopReachy
{
    public class ActiveControllerManager : Singleton<ActiveControllerManager>
    {
        public ControllersManager ControllersManager { get; private set; }
        public ControllersVibrations ControllersVibrations { get; private set; }

        protected override void Init()
        {
            ControllersManager = GetComponent<ControllersManager>();
            ControllersVibrations = GetComponent<ControllersVibrations>();
        }
    }
}


