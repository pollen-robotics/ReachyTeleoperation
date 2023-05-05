using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace TeleopReachy
{
    public static class ChangeRenderer
    {
        // Turn on or off the renderer of a gameObject and of all its children
        public static void switchRenderer(this Transform t, bool enabled)
        {
            foreach (Transform child in t)
            {
                switchRenderer(child, enabled);
            }
            Renderer r = t.gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                r.enabled = enabled;
            }
        }
    }

    public static class ChangeCollider
    {
        // Turn on or off the collider of a gameObject and of all its children
        public static void switchCollider(this Transform t, bool enabled)
        {
            foreach (Transform child in t)
            {
                switchCollider(child, enabled);
            }
            Collider c = t.gameObject.GetComponent<Collider>();
            if (c != null)
            {
                c.enabled = enabled;
            }
        }
    }

    public static class ChangeLayer
    {
        // Send a gameObject to another layer for rendering
        public static void sendToLayer(this Transform t, int nlayer)
        {
            foreach (Transform child in t)
            {
                sendToLayer(child, nlayer);
            }
            t.gameObject.layer = nlayer;
        }
    }

    public static class ChangeActiveChildStatus
    {
        // Activate of deactivate all children of a gameObject (but not the gameObject)
        public static void ActivateChildren(this Transform t, bool enabled)
        {
            foreach (Transform child in t)
            {
                child.gameObject.SetActive(enabled);
            }
        }
    }
}