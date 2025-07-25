using System;
using UnityEngine;
using TMPro;
using NPC;
using UnityEditor;
using UnityEngine.Serialization;
using System.Collections;


public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer; // Layer 6 ve 8'i içerdiğinden emin olun
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Transform _holdPosition;
    private GameObject lastHighlightedObject;
    [SerializeField]private int originalLayer = -1; // Orijinal katmanı kalıcı olarak sakla

    public bool isHoldingSomething = false;
    public GameObject _currentHeldObject;
    public bool IsInteractWhileHolding = false;
    
    [Header("Texts")]
    [SerializeField] public TextMeshProUGUI noToiletAvaibleText;
    
    
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
        UIBuildSystem.Instance.SetShowDemolishText();
    }
    
    private void UpdateHighlight(RaycastHit hit, bool hitSomething)
    {
        //if (_currentHeldObject != null) return;

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
            UITextManager.Instance.interactPrompt.enabled = false;
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

        if (newObject.GetComponent<NPCController>() != null)
        {
            lastHighlightedObject = newObject.transform.GetChild(0).gameObject;
            originalLayer = newObject.transform.GetChild(0).gameObject.layer;
            newObject.transform.GetChild(0).gameObject.layer = 8;
        }
        else
        {
            // Yeni objeyi işaretle
            lastHighlightedObject = newObject;
            originalLayer = newObject.layer;
            newObject.layer = 8;
        }
        
    }
    
    private void UpdateUI(RaycastHit hit)
    {
        if (!hit.transform)
        {
            UITextManager.Instance.interactPrompt.enabled = false;
            return;
        }

        IInteractable interactable = hit.transform.GetComponent<IInteractable>();
        UITextManager.Instance.interactPrompt.enabled = interactable != null;
        UITextManager.Instance.interactPrompt.text = interactable?.GetInteractionText();
    }
    
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && _currentHeldObject == null)
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.E) && _currentHeldObject != null)
        {
            TryInteractWhileHolding();
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
                UITextManager.Instance.warningPrompt.enabled = false;
                _currentHeldObject = hit.transform.gameObject;
                holdable.OnPickup(_holdPosition);
                IsInteractWhileHolding = true;
                ResetObjectHighlight(); // EKLENDİ
                
                
            }
        }
    }
    private void TryInteractWhileHolding()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            var heldPoop = _currentHeldObject?.GetComponent<WC.Poop>();

            if (interactable != null && heldPoop != null)
            {
                interactable.Interact();
                ResetObjectHighlight();
                UITextManager.Instance.warningPrompt.enabled = false;

                // Coroutine iptal et
                if (warningCoroutine != null)
                    StopCoroutine(warningCoroutine);
            }
            else if (interactable != null && heldPoop == null)
            {
                UITextManager.Instance.warningPrompt.text = "Your hands are full!";
                UITextManager.Instance.warningPrompt.enabled = true;

                // Zaten çalışan bir coroutine varsa durdur
                if (warningCoroutine != null)
                    StopCoroutine(warningCoroutine);

                warningCoroutine = StartCoroutine(DisableWarningPromptAfterDelay(1f));
            }
        }
    }
    
    private Coroutine warningCoroutine;

    private IEnumerator DisableWarningPromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UITextManager.Instance.warningPrompt.enabled = false;
        warningCoroutine = null;
    }



    

    
}