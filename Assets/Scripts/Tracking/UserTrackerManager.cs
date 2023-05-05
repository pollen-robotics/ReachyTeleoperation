using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeleopReachy
{
    public class UserTrackerManager : Singleton<UserTrackerManager>
    {
        public HandsTracker HandsTracker { get; private set; }
        public HeadTracker HeadTracker { get; private set; }

        protected override void Init()
        {
            HeadTracker = transform.GetChild(0).GetComponent<HeadTracker>();
            HandsTracker = transform.GetChild(1).GetComponent<HandsTracker>();
        }
    }
}
