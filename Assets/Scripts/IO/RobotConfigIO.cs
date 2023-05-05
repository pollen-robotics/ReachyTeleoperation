
using UnityEngine;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Collections.Generic;

namespace TeleopReachy
{

    [Serializable]
    public class Robot
    {
        public string uid;
        public string ip;

        public const string VIRTUAL_ROBOT = "Virtual";
        public const string VIRTUAL_ROBOT_IP = "none";

        public bool IsVirtualRobot()
        {
            return (uid == VIRTUAL_ROBOT && ip == VIRTUAL_ROBOT_IP);
        }

        public static bool IsCurrentRobotVirtual()
        {
            return (PlayerPrefs.GetString("robot_ip") == Robot.VIRTUAL_ROBOT_IP);
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "Connections")]
    public class SaveRobots
    {
        [XmlArrayItem("Robots")]
        public List<Robot> Robots;
    }

    public static class RobotConfigIO
    {
        public static void SaveRobots(string filename, List<Robot> robotsList)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SaveRobots));

            SaveRobots robotSave = new SaveRobots();
            robotSave.Robots = robotsList;

            using (TextWriter writer = new StreamWriter(filename))
            {
                ser.Serialize(writer, robotSave);
            }
        }

        public static List<Robot> LoadRobots(string filename)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveRobots));

            SaveRobots robotSave;
            if (File.Exists(filename))
            {
                using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                {
                    robotSave = (SaveRobots)serializer.Deserialize(fileStream);
                }
                return robotSave.Robots;
            }
            else
            {
                return new List<Robot>();
            }
        }
    }
}