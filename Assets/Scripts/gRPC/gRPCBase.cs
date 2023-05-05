using UnityEngine;
using System.Collections;
using Grpc.Core;


namespace TeleopReachy
{
    public class gRPCBase : MonoBehaviour
    {
        protected Channel channel = null;

        protected bool isRobotInRoom = false;

        public string rpcException;

        private const float REFRESH_REQ_SEC = 2;

        private RobotStatus robotStatus = null;

        private bool wasSuspended = false;

        protected string ip_address;

        protected void InitChannel(string port)
        {
            ip_address = PlayerPrefs.GetString("robot_ip");
            if (ip_address != Robot.VIRTUAL_ROBOT_IP)
            {
                robotStatus = RobotDataManager.Instance.RobotStatus;
                string address = ip_address + ":" + PlayerPrefs.GetString(port);
                channel = new Channel(address, ChannelCredentials.Insecure);
                wasSuspended = false;
                StartCoroutine(ChannelMonitor());
            }
        }

        IEnumerator ChannelMonitor()
        {
            while (true)
            {
                switch (channel.State)
                {
                    case ChannelState.Ready:
                        {
                            if (wasSuspended)
                            {
                                RecoverFromNetWorkIssue();
                                wasSuspended = false;
                            }
                            break;
                        }
                    case ChannelState.TransientFailure:
                        {
                            NotifyDisconnection();
                            wasSuspended = true;
                            break;
                        }
                    case ChannelState.Shutdown:
                        {
                            Debug.LogError("major issue!");
                            break;
                        }
                }
                yield return new WaitForSeconds(REFRESH_REQ_SEC);
            }
        }

        protected virtual void RecoverFromNetWorkIssue()
        {

        }

        protected virtual void NotifyDisconnection()
        {

        }
    }
}