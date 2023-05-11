using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeleopReachy
{
    public class SetGroundLevel : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            EventManager.StartListening(EventNames.MirrorSceneLoaded, UpdateGroundLevel);
        }

        // Update is called once per frame
        void UpdateGroundLevel()
        {
            Vector3 userTrackerPosition = UserTrackerManager.Instance.transform.position; // - transform.forward * 0.1f;
            transform.position = new Vector3(transform.position.x, userTrackerPosition.y - 1.25f, transform.position.z);
        }
    }
}
