using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Voyeur : MonoBehaviour, IInteractable
{
    FirstPersonController player;

    [SerializeField] Transform camHolder;
    
    public MeshRenderer meshRenderer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetInteractionText()
    {
        return "Spy the customer [R]";
    }

    public void Interact()
    {
        player.EnterVoyeurMode(camHolder);
        
        
    }
}
