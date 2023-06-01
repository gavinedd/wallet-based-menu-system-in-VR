using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WalletOpener : MonoBehaviour
{
    private bool isOpen;
    private bool isHeld;
    private Vector3 initialHandRotation;
    private Vector3 currentHandRotation;
    public GameObject frontCover;
    public GameObject backCover;
    private float angleDifference;
    public WalletMenu menu;

    private InputDevice targetDevice;
    public InputDeviceCharacteristics controllerCharacteristics;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isHeld = false;
        angleDifference = 0;
        getDevices();
        menu.Visibility = 1;
        menu.Visibility = 0;
    }

    // Update is called once per frame
    void Update()
    {
        menu.Visibility = 0;
        if (!targetDevice.isValid)
        {
            getDevices();
        }
        else
        {
            if (isHeld)
            {
                backCover.transform.rotation = frontCover.transform.rotation;
                //Debug.Log("angle diff: " + angleDifference);
                if (0 < angleDifference && angleDifference < 160)
                {
                    backCover.transform.Rotate(new Vector3(1.7f * angleDifference, 0, 0));
                    menu.Visibility = 1.7f * angleDifference / 160;
                }
            }
            else if (!isHeld)
            {
                angleDifference = Mathf.Max(angleDifference - 0.3f, 0);

                backCover.transform.rotation = frontCover.transform.rotation;
                if (0 < angleDifference && angleDifference < 160)
                {
                    backCover.transform.Rotate(new Vector3(1.7f * angleDifference, 0, 0));
                    menu.Visibility = 1.7f * angleDifference / 160;
                }
            }
            else
            {
                backCover.transform.rotation = frontCover.transform.rotation;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            isHeld = true;
            if (!targetDevice.isValid)
            {
                getDevices();
            }
            else
            {
                targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
                if (!isOpen && triggerValue > 0.5)
                {
                    initialHandRotation = other.transform.rotation.eulerAngles;
                    currentHandRotation = other.transform.rotation.eulerAngles;
                    angleDifference = 0;
                    isOpen = true;
                }
                else if (triggerValue > 0.5)
                {
                    currentHandRotation = other.transform.rotation.eulerAngles;
                    Vector3 angle = currentHandRotation - initialHandRotation;
                    angleDifference = (angle.x + angle.y + angle.z + 1080) % 360;
                }
                else
                {
                    angleDifference = Mathf.Max(angleDifference - 0.2f, 0);
                    isOpen = false;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            isHeld = false;
        }
    }

    void getDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }
}
