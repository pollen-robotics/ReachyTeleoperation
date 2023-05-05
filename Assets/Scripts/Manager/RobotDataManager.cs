using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeleopReachy
{
    public class RobotDataManager : Singleton<RobotDataManager>
    {
        public RobotJointCommands RobotJointCommands { get; private set; }
        public RobotStatus RobotStatus { get; private set; }
        public RobotConfig RobotConfig { get; private set; }
        public RobotMobilityCommands RobotMobilityCommands { get; private set; }
        public RobotPingWatcher RobotPingWatcher { get; private set; }
        public ErrorManager ErrorManager { get; private set; }

        protected override void Init()
        {
            RobotJointCommands = GetComponent<RobotJointCommands>();
            RobotStatus = GetComponent<RobotStatus>();
            RobotMobilityCommands = GetComponent<RobotMobilityCommands>();
            RobotConfig = GetComponent<RobotConfig>();
            RobotPingWatcher = GetComponent<RobotPingWatcher>();
            ErrorManager = GetComponent<ErrorManager>();
        }
    }
}
