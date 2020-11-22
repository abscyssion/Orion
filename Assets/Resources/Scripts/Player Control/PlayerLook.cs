using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerLook : MonoBehaviour
{
    public Transform cameraObject;

    private const float mouseSensitivity = 250.0f;

    private const float fov = 90.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Locked;

            float mouseX = mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
            float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

            rotationX -= mouseY; rotationX = Mathf.Clamp(rotationX, -fov, fov);
            rotationY += mouseX;

            transform.eulerAngles = new Vector3(0.0f, rotationY, 0.0f);
            cameraObject.eulerAngles = new Vector3(rotationX, rotationY, 0.0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
