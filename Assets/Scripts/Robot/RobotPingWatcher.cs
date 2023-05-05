using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TeleopReachy
{
    public class RobotPingWatcher : MonoBehaviour
    {
        private const int QUEUE_SIZE = 5;

        private const float REFRESH_REQ_SEC = 1;

        public const int THRESHOLD_LOW_QUALITY_PING = 45;

        private float mean_ping = 0;

        private bool isUnstable = false;

        Coroutine pingCheck = null;

        // Start is called before the first frame update
        void Start()
        {
            string robot_ip = PlayerPrefs.GetString("robot_ip");
            if(robot_ip != "localhost" && robot_ip != Robot.VIRTUAL_ROBOT_IP) pingCheck = StartCoroutine(MeanPing(robot_ip));
        }

        void OnDestroy()
        {
            if (pingCheck != null)
                StopCoroutine(pingCheck);
        }

        IEnumerator MeanPing(string ip)
        {
            Queue<int> lastPingTimes = new Queue<int>(QUEUE_SIZE);
            while (true)
            {
                Ping p = new Ping(ip);

                yield return new WaitForSeconds(REFRESH_REQ_SEC);

                if(p.isDone) lastPingTimes.Enqueue(p.time);
                else lastPingTimes.Enqueue((int)REFRESH_REQ_SEC * 1000);

                if (lastPingTimes.Count > QUEUE_SIZE) lastPingTimes.Dequeue();

                float mean = 0;
                isUnstable = false;
                foreach (int obj in lastPingTimes)
                {
                    if (obj > THRESHOLD_LOW_QUALITY_PING)
                        isUnstable = true;
                    mean += obj;
                }
                mean_ping = mean / lastPingTimes.Count;
            }
        }

        public float GetPing()
        {
            return mean_ping;
        }

        public bool GetIsUnstablePing()
        {
            return isUnstable;
        }
    }
}