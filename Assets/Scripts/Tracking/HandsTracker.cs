using UnityEngine;
using Reachy.Sdk.Kinematics;
using UnityEngine.XR.Interaction.Toolkit;


namespace TeleopReachy
{
    [System.Serializable]
    public class HandController
    {
        private Transform VRHand;

        [SerializeField]
        [HideInInspector]
        public UnityEngine.Matrix4x4 handPose = UnityEngine.Matrix4x4.identity;
        [SerializeField]
        [HideInInspector]
        public Reachy.Sdk.Kinematics.Matrix4x4 target_pos;

        [SerializeField]
        [HideInInspector]
        public string handSide;

        [SerializeField]
        [HideInInspector]
        public float trigger = 0;

        public ArmSide side_id;

        public UnityEngine.XR.InputDevice device;

        public HandController(string side, UnityEngine.XR.InputDevice handDevice)
        {
            InitDevice(side);
            device = handDevice;
        }

        public HandController(string side)
        {
            InitDevice(side);
        }

        void InitDevice(string side)
        {
            handSide = side;

            if (handSide == "right")
            {
                side_id = ArmSide.Right;
                VRHand = GameObject.Find("TrackedRightHand").transform;
            }
            else
            {
                side_id = ArmSide.Left;
                VRHand = GameObject.Find("TrackedLeftHand").transform;
            }
        }

        public Transform GetVRHand()
        {
            return VRHand;
        }
    }

    public class HandsTracker : MonoBehaviour
    {
        public HandController rightHand;
        public HandController leftHand;

        public ControllersManager controllers;

        void Awake()
        {
            rightHand = new HandController("right", controllers.rightHandDevice);
            leftHand = new HandController("left", controllers.leftHandDevice);

            controllers.event_OnDevicesUpdate.AddListener(UpdateDevices);
        }

        void Start()
        {
            GetTransforms(rightHand);
            GetTransforms(leftHand);
        }

        private void UpdateDevices()
        {
            rightHand = new HandController("right", controllers.rightHandDevice);
            leftHand = new HandController("left", controllers.leftHandDevice);
        }

        void Update()
        {
            GetTransforms(rightHand);
            GetTransforms(leftHand);

            AdaptativeCloseHand(rightHand);
            AdaptativeCloseHand(leftHand);
        }

        private void GetTransforms(HandController hand)
        {
            // Position
            Vector3 positionHeadset = UnityEngine.Quaternion.Inverse(transform.parent.rotation) * (hand.GetVRHand().position - transform.parent.position);
            Vector3 positionReachy = new Vector3(positionHeadset.z, -positionHeadset.x, positionHeadset.y);
            Vector4 positionVect = new Vector4(positionReachy.x, positionReachy.y, positionReachy.z, 1);

            // Rotation
            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Inverse(transform.parent.rotation) * hand.GetVRHand().rotation;
            hand.handPose.SetTRS(new Vector3(0, 0, 0), rotation, new Vector3(1, 1, 1));

            // matrice de passage
            UnityEngine.Matrix4x4 mP = new UnityEngine.Matrix4x4(new Vector4(0, -1, 0, 0),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(1, 0, 0, 0),
                                            new Vector4(0, 0, 0, 1));
            hand.handPose = (mP * hand.handPose) * mP.inverse;

            hand.handPose.SetColumn(3, positionVect);

            hand.target_pos = new Reachy.Sdk.Kinematics.Matrix4x4
            {
                Data = { hand.handPose[0,0], hand.handPose[0,1], hand.handPose[0,2], hand.handPose[0,3],
                                                                                hand.handPose[1,0], hand.handPose[1,1], hand.handPose[1,2], hand.handPose[1,3],
                                                                                hand.handPose[2,0], hand.handPose[2,1], hand.handPose[2,2], hand.handPose[2,3],
                                                                                hand.handPose[3,0], hand.handPose[3,1], hand.handPose[3,2], hand.handPose[3,3] }
            };
        }

        private void AdaptativeCloseHand(HandController hand)
        {
            // Get value of how much trigger is pushed
            float trigger;
            if (hand.device.isValid)
            {
                hand.device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out trigger);
                hand.trigger = trigger;
            }
        }
    }
}