using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachy;

namespace TeleopReachy
{
    public class SetRobotAtUserTrackerPosition : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            Vector3 userTrackerPosition = UserTrackerManager.Instance.transform.position; // - transform.forward * 0.1f;
            Quaternion userTrackerRotation = UserTrackerManager.Instance.transform.localRotation;
            Vector3 userTrackerEulerAngles = userTrackerRotation.eulerAngles;

            transform.rotation = Quaternion.Euler(-90, userTrackerEulerAngles.y, 0);
            transform.position = userTrackerPosition;
        }
    }
}
