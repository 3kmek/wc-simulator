using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] float sensitivity = 10f;
    private float rotationX, rotationY = 0f;
    [SerializeField] GameObject player;
    [SerializeField] private Transform orientation;
    
    public bool isCursorVisible = true;

    private void Start()
    {
        
    }

    void Update()
    {
        HandleCameraRotation();

        HandleCursor();
    }

    void HandleCursor()
    {
        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        rotationY += mouseX;
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -70f, 70f);
        
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0f);
        
        
        //old
        //transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        //player.transform.Rotate(Vector3.up * mouseX);
    }
}
