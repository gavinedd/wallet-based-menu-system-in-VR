using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TeleportObject : MonoBehaviour
{
    public XRNode controller;
    public InputFeatureUsage<bool> closeButton = CommonUsages.triggerButton;
    public InputFeatureUsage<bool> openButton = CommonUsages.gripButton;
    public GameObject objectToTeleport;
    public Transform handTransform;

    private bool isObjectTeleported = false;

    private void Update()
    {
        // Check if the open button is pressed on the specified controller
        InputDevice device = InputDevices.GetDeviceAtXRNode(controller);
        if (device.TryGetFeatureValue(openButton, out bool openButtonValue) && openButtonValue)
        {
            // If the object isn't already in the hand, teleport it to the hand's position and orientation
            if (!isObjectTeleported)
            {
                // Convert the local position of the handTransform to a world position
                Vector3 worldPosition = handTransform.TransformPoint(Vector3.zero);
                objectToTeleport.transform.position = worldPosition;
                objectToTeleport.transform.rotation = handTransform.rotation;

                // Set the flag to true to indicate that the object is teleported
                isObjectTeleported = true;
            }
        }
        // Check if the close button is pressed on the specified controller
        else if (device.TryGetFeatureValue(closeButton, out bool closeButtonValue) && closeButtonValue)
        {
            // If the object is in the hand, make it disappear and set isOpen to false
            if (isObjectTeleported)
            {
                //objectToTeleport.SetActive(false);

                // Set the flag to false to indicate that the object is no longer teleported
                isObjectTeleported = false;
            }
        }
        // Check if the open button is not pressed and the object is teleported, reset isObjectTeleported to false
        else if (!openButtonValue && isObjectTeleported)
        {
            isObjectTeleported = false;
        }
    }
}
