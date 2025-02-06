using System;
using UnityEngine;
using TMPro;
using NPC;
using UnityEditor;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer; // Layer 6 ve 8'i içerdiğinden emin olun
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Transform _holdPosition;
    private GameObject lastHighlightedObject;
    [SerializeField]private int originalLayer = -1; // Orijinal katmanı kalıcı olarak sakla

    public bool isHoldingSomething = false;
    public GameObject _currentHeldObject;
    
    
    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        
        //new
        HandleRaycast();
        HandleInteractionInput();
        
        ///

        
    }
    
    private void HandleRaycast()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer);
        
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green, 0.1f);

        UpdateHighlight(hit, hitSomething);
        UpdateUI(hit);
    }
    
    private void UpdateHighlight(RaycastHit hit, bool hitSomething)
    {
        if (_currentHeldObject != null) return;

        // Layer reset mekanizması düzeltildi
        if (lastHighlightedObject != null && (!hitSomething || hit.transform.gameObject != lastHighlightedObject))
        {
            ResetObjectHighlight();
        }

        if (hitSomething)
        {
            HandleNewObjectHighlight(hit.transform.gameObject);
        }
        else
        {
            interactPrompt.enabled = false;
        }
    }

    
    private void ResetObjectHighlight()
    {
        if (lastHighlightedObject != null)
        {
            lastHighlightedObject.layer = originalLayer;
            lastHighlightedObject = null;
            originalLayer = -1;
        }
    }
    
    private void HandleNewObjectHighlight(GameObject newObject)
    {
        if (newObject.layer == 8) return;

        // Yeni objeyi işaretle
        lastHighlightedObject = newObject;
        originalLayer = newObject.layer;
        newObject.layer = 8;
    }
    
    private void UpdateUI(RaycastHit hit)
    {
        if (_currentHeldObject != null || !hit.transform)
        {
            interactPrompt.enabled = false;
            return;
        }

        IInteractable interactable = hit.transform.GetComponent<IInteractable>();
        interactPrompt.enabled = interactable != null;
        interactPrompt.text = interactable?.GetInteractionText();
    }
    
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && _currentHeldObject == null)
        {
            TryInteract();
        }
    }
    
    private void TryInteract()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            interactable?.Interact();

            IHoldable holdable = hit.transform.GetComponent<IHoldable>();
            if (holdable != null)
            {
                _currentHeldObject = hit.transform.gameObject;
                holdable.OnPickup(_holdPosition);
                ResetObjectHighlight(); // EKLENDİ
            }
        }
    }
    

    
}