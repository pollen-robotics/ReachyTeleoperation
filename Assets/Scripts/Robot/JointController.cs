using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    private float targetPosition;
    private bool changed;
    private ArticulationBody articulation;

    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    void FixedUpdate()
    {
        if (changed)
        {
            var drive = articulation.xDrive;
            drive.target = targetPosition;
            articulation.xDrive = drive;

            changed = false;
        }
    }

    public void RotateTo(float newTargetPosition)
    {
        targetPosition = newTargetPosition;
        changed = true;
    }

    public float GetPresentPosition()
    {
        return Mathf.Rad2Deg * articulation.jointPosition[0];
    }
}
