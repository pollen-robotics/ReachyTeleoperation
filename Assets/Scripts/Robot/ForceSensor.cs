using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSensor : MonoBehaviour
{
    public float currentForce;
    private float gain = 10000;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 0.01)
        {
            currentForce = collision.impulse.magnitude * gain;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.impulse.magnitude > 0.01)
        {
            currentForce = collision.impulse.magnitude * gain;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        currentForce = collision.impulse.magnitude * gain;
    }
}
