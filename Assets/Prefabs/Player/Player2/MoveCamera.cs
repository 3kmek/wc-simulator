using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = cameraTransform.position;
    }
}
