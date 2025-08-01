using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCanvas : MonoBehaviour
{
    private Transform camera;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        HandleLookAtCamera();
    }

    void HandleLookAtCamera()
    {
        Vector3 directionToCameraPosition = camera.position - transform.position;
        transform.rotation = Quaternion.LookRotation(directionToCameraPosition) * Quaternion.Euler(0, 180, 0);
    }
}
