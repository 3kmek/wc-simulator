using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject voyeurVFX;
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactableLayer;

    [SerializeField] private Transform doorCenter;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
        
    }
    
    private void HandleRaycast()
    {
        
        Ray ray = new Ray(doorCenter.position, transform.forward);
        
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer);
        
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green, 0.1f);

        if (hitSomething)
        {
            voyeurVFX.SetActive(true);
        }
        else
        {
            voyeurVFX.SetActive(false);
        }
    }
}
