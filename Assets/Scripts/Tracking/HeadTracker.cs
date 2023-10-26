using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using Reachy;
using Reachy.Sdk.Kinematics;

namespace TeleopReachy
{
    public class HeadTracker : MonoBehaviour
    {
        private UnityEngine.Quaternion initialRotation;
        private HeadIKRequest headTarget;

        void Update()
        {
            UnityEngine.Quaternion headQuat = transform.localRotation;
            UnityEngine.Quaternion RotZeroQuat = transform.parent.rotation;
            headQuat = UnityEngine.Quaternion.Inverse(RotZeroQuat) * headQuat;

            headQuat *= UnityEngine.Quaternion.Euler(Vector3.right * -10);

            // Amplify rotation
            headQuat = UnityEngine.Quaternion.LerpUnclamped(UnityEngine.Quaternion.identity, headQuat, 1.5f);

            headTarget = new HeadIKRequest
            {
                Q = new Reachy.Sdk.Kinematics.Quaternion
                {
                    W = headQuat.w,
                    X = -headQuat.z,
                    Y = headQuat.x,
                    Z = -headQuat.y,
                }
            };
        }

        public HeadIKRequest GetHeadTarget()
        {
            return headTarget;
        }
    }
}