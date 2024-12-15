using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] float sensitivity = 10f;
    private float rotationX = 0f;
    [SerializeField] GameObject player;
   
    void Update()
    {
        HandleCameraRotation();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        
        player.transform.Rotate(Vector3.up * mouseX);
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -70f, 70f);
        
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
