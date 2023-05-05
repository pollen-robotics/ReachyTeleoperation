using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

using Grpc.Core;

using Reachy.Sdk.Joint;
using Reachy.Sdk.Camera;
using Reachy.Sdk.Fan;

namespace Reachy
{
    [System.Serializable]
    public class Motor
    {
        public string name;
        public int uid;
        public GameObject gameObject;
        public float targetPosition;
        public float presentPosition;
        public float offset;
        public bool isDirect;
        public bool isCompliant;
    }

    [System.Serializable]
    public class Sensor
    {
        public string name;
        public GameObject sensorObject;
        public float currentState;
    }

    [System.Serializable]
    public class Fan
    {
        public string name;
        public bool state;
    }

    [System.Serializable]
    public struct SerializableMotor
    {
        public string name;
        public int uid;
        public float present_position;
        public float goal_position;
        public bool isCompliant;
    }

    [System.Serializable]
    public struct SerializableSensor
    {
        public string name;
        public float sensor_state;
    }

    [System.Serializable]
    public struct SerializableFan
    {
        public string name;
        public bool fan_state;
    }

    [System.Serializable]
    public struct MotorCommand
    {
        public string name;
        public float goal_position;
    }

    [System.Serializable]
    public struct SerializableCommands
    {
        public List<MotorCommand> motors;
    }

    public class ReachyController : MonoBehaviour
    {
        public Motor[] motors;
        public Fan[] fans;
        public Sensor[] sensors;
        public GameObject head;

        private Dictionary<string, Motor> name2motor;
        private Dictionary<string, Sensor> name2sensor;
        private Dictionary<string, Fan> name2fan;

        UnityEngine.Quaternion baseHeadRot;
        UnityEngine.Quaternion targetHeadRot;
        public Vector3 headOrientation;
        float headRotDuration;

        void Awake()
        {
            name2motor = new Dictionary<string, Motor>();

            name2sensor = new Dictionary<string, Sensor>();

            name2fan = new Dictionary<string, Fan>();

            for (int i = 0; i < motors.Length; i++)
            {
                Motor m = motors[i];
                m.uid = i;
                name2motor[m.name] = m;
            }

            for (int i = 0; i < sensors.Length; i++)
            {
                Sensor s = sensors[i];
                name2sensor[s.name] = s;
            }

            for (int i = 0; i < fans.Length; i++)
            {
                Fan f = fans[i];
                name2fan[f.name] = f;
            }

            headOrientation = new Vector3(0, 0, 0);
            baseHeadRot = head.transform.localRotation;
        }

        void Update()
        {
            for (int i = 0; i < motors.Length; i++)
            {
                Motor m = motors[i];

                if (!m.name.StartsWith("neck"))
                {
                    JointController joint = m.gameObject.GetComponent<JointController>();
                    joint.RotateTo(m.targetPosition);

                    m.presentPosition = joint.GetPresentPosition();
                }
                else
                {
                    m.presentPosition = m.targetPosition;
                }
            }

            for (int i = 0; i < sensors.Length; i++)
            {
                Sensor s = sensors[i];

                ForceSensor fSensor = s.sensorObject.GetComponent<ForceSensor>();
                s.currentState = fSensor.currentForce;
            }

            UpdateHeadOrientation();
        }

        void SetMotorTargetPosition(string motorName, float targetPosition)
        {
            targetPosition += name2motor[motorName].offset;
            if (!name2motor[motorName].isDirect)
            {
                targetPosition *= -1;
            }
            name2motor[motorName].targetPosition = targetPosition;
        }

        void SetMotorCompliancy(string motorName, bool compliancy)
        {
            name2motor[motorName].isCompliant = compliancy;
        }

        void SetFanState(string fanName, bool targetState)
        {
            if (fanName != "neck_fan")
            {
                name2fan[fanName].state = targetState;
            }
        }

        public void HandleCommand(Dictionary<JointId, float> commands)
        {
            bool containNeckCommand = false;
            foreach (KeyValuePair<JointId, float> kvp in commands)
            {
                string motorName;
                switch (kvp.Key.IdCase)
                {
                    case JointId.IdOneofCase.Name:
                        motorName = kvp.Key.Name;
                        break;
                    case JointId.IdOneofCase.Uid:
                        motorName = motors[kvp.Key.Uid].name;
                        break;
                    default:
                        motorName = kvp.Key.Name;
                        break;
                }
                if (!name2motor[motorName].isCompliant)
                {
                    SetMotorTargetPosition(motorName, kvp.Value);
                }


                if (motorName == "neck_roll")
                {
                    containNeckCommand = true;
                    headOrientation[0] = kvp.Value;
                }
                if (motorName == "neck_pitch")
                {
                    containNeckCommand = true;
                    headOrientation[1] = kvp.Value;
                }
                if (motorName == "neck_yaw")
                {
                    containNeckCommand = true;
                    headOrientation[2] = kvp.Value;
                }
            }

            if (containNeckCommand)
            {
                //UnityEngine.Quaternion.Euler(); not properly working. Manually creating rotation
                UnityEngine.Quaternion euler_request = UnityEngine.Quaternion.Euler(Vector3.forward * headOrientation[2]) * UnityEngine.Quaternion.Euler(Vector3.up * -headOrientation[0]) * UnityEngine.Quaternion.Euler(Vector3.right * headOrientation[1]);
                HandleHeadOrientation(euler_request);
            }
        }

        public void HandleCompliancy(Dictionary<JointId, bool> commands)
        {
            foreach (KeyValuePair<JointId, bool> kvp in commands)
            {
                string motorName;
                switch (kvp.Key.IdCase)
                {
                    case JointId.IdOneofCase.Name:
                        motorName = kvp.Key.Name;
                        break;
                    case JointId.IdOneofCase.Uid:
                        motorName = motors[kvp.Key.Uid].name;
                        break;
                    default:
                        motorName = kvp.Key.Name;
                        break;
                }
                SetMotorCompliancy(motorName, kvp.Value);
            }
        }

        public void HandleFanCommand(Dictionary<FanId, bool> commands)
        {
            foreach (KeyValuePair<FanId, bool> kvp in commands)
            {
                string fanName;
                switch (kvp.Key.IdCase)
                {
                    case FanId.IdOneofCase.Name:
                        fanName = kvp.Key.Name;
                        break;
                    case FanId.IdOneofCase.Uid:
                        fanName = fans[kvp.Key.Uid].name;
                        break;
                    default:
                        fanName = kvp.Key.Name;
                        break;
                }
                SetFanState(fanName, kvp.Value);
            }
        }

        public List<SerializableMotor> GetCurrentMotorsState(Dictionary<JointId, JointField> request)
        {
            List<SerializableMotor> motorsList = new List<SerializableMotor>();
            foreach (KeyValuePair<JointId, JointField> kvp in request)
            {
                Motor m;
                float position;
                float target_position;
                bool compliancy;
                switch (kvp.Key.IdCase)
                {
                    case JointId.IdOneofCase.Name:
                        m = name2motor[kvp.Key.Name];
                        position = m.presentPosition;
                        target_position = m.targetPosition;
                        compliancy = m.isCompliant;
                        if (!name2motor[kvp.Key.Name].isDirect)
                        {
                            position *= -1;
                            target_position *= -1;
                        }
                        position -= name2motor[kvp.Key.Name].offset;
                        target_position -= name2motor[kvp.Key.Name].offset;
                        break;
                    case JointId.IdOneofCase.Uid:
                        m = motors[kvp.Key.Uid];
                        position = m.presentPosition;
                        target_position = m.targetPosition;
                        compliancy = m.isCompliant;
                        if (!motors[kvp.Key.Uid].isDirect)
                        {
                            position *= -1;
                            target_position *= -1;
                        }
                        position -= motors[kvp.Key.Uid].offset;
                        target_position -= motors[kvp.Key.Uid].offset;
                        break;
                    default:
                        m = name2motor[kvp.Key.Name];
                        position = m.presentPosition;
                        target_position = m.targetPosition;
                        compliancy = m.isCompliant;
                        if (!name2motor[kvp.Key.Name].isDirect)
                        {
                            position *= -1;
                            target_position *= -1;
                        }
                        position -= name2motor[kvp.Key.Name].offset;
                        target_position -= name2motor[kvp.Key.Name].offset;
                        break;
                }
                motorsList.Add(new SerializableMotor() { name = m.name, uid = m.uid, present_position = Mathf.Deg2Rad * position, goal_position = Mathf.Deg2Rad * target_position, isCompliant = compliancy });
            }
            return motorsList;
        }

        public List<SerializableSensor> GetCurrentSensorsState(Google.Protobuf.Collections.RepeatedField<Reachy.Sdk.Joint.SensorId> request)
        {
            List<Sensor> sensorRequest = new List<Sensor>();

            foreach (var sensor in request)
            {
                switch (sensor.IdCase)
                {
                    case SensorId.IdOneofCase.Name:
                        sensorRequest.Add(name2sensor[sensor.Name]);
                        break;
                    case SensorId.IdOneofCase.Uid:
                        sensorRequest.Add(sensors[sensor.Uid]);
                        break;
                }
            }

            List<SerializableSensor> sensorsList = new List<SerializableSensor>();

            foreach (var sensor in sensorRequest)
            {
                float state = sensor.currentState;
                sensorsList.Add(new SerializableSensor() { name = sensor.name, sensor_state = state });
            }

            return sensorsList;
        }

        public List<SerializableFan> GetCurrentFansState(Google.Protobuf.Collections.RepeatedField<Reachy.Sdk.Fan.FanId> request)
        {
            List<Fan> fanRequest = new List<Fan>();

            foreach (var fan in request)
            {
                switch (fan.IdCase)
                {
                    case FanId.IdOneofCase.Name:
                        fanRequest.Add(name2fan[fan.Name]);
                        break;
                    case FanId.IdOneofCase.Uid:
                        fanRequest.Add(fans[fan.Uid]);
                        break;
                }
            }

            List<SerializableFan> fansList = new List<SerializableFan>();

            foreach (var fan in fanRequest)
            {
                bool state = fan.state;
                fansList.Add(new SerializableFan() { name = fan.name, fan_state = state });
            }

            return fansList;
        }

        public void HandleHeadOrientation(UnityEngine.Quaternion q)
        {
            targetHeadRot = q;
        }

        void UpdateHeadOrientation()
        {
            head.transform.localRotation = targetHeadRot;
        }
    }
}