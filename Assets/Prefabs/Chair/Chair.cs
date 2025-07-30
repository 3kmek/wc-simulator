using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour, IInteractable
{
    private GameObject player;
    private FirstPersonController firstPersonController;
    [SerializeField] private Transform camHolder;
    
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        firstPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    
    void Update()
    {
        
    }

    public string GetInteractionText()
    {
        return "Sit [E]";
    }

    public void Interact()
    {
        if (!firstPersonController.IsPlayerSitting)
        {
            firstPersonController.EnterSittingMode(camHolder);
        }
    }
}
