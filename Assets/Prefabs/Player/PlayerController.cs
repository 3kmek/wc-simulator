using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    Camera playerCamera;
    void Start()
    {
        playerCamera = Camera.main; // Ana kamerayı al
        Cursor.lockState = CursorLockMode.Locked; // Mouse'u oyun ekranına kilitle
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        #region Basic Controller

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * Time.deltaTime * speed;
        transform.Translate(movement);

        #endregion
    }
}
