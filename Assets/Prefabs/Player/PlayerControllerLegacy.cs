using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControllerLegacy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] float interactRange = 3f;
    [SerializeField] Camera playerCamera;
    
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    void Start()
    {
        playerCamera = Camera.main; // Ana kamerayı al
        Cursor.lockState = CursorLockMode.Locked; // Mouse'u oyun ekranına kilitle
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        
        CheckForInteractable();
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }
    }
    
    void CheckForInteractable()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera atanmamış!");
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (interactPrompt != null)
                {
                    interactPrompt.enabled = true; // Mesajı göster
                    interactPrompt.text = "Give The Key \n(Woman Section)\nPress [E]\n"; // İstediğiniz mesajı ayarlayın
                }
            }
            else
            {
                if (interactPrompt != null)
                {
                    interactPrompt.enabled = false; // Mesajı gizle
                }
            }
        }
        else
        {
            if (interactPrompt != null)
            {
                interactPrompt.enabled = false; // Mesajı gizle
            }
        }

        // Raycast çizgisi görünür olması için
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green, 0.1f);
    }

    void InteractWithObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                Debug.Log("Etkileşim yapılabilir bir nesneye bakmıyorsunuz.");
            }
        }
        else
        {
            Debug.Log("Hiçbir nesneye çarpmadı.");
        }

        // Raycast çizgisi görünür olması için
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green, 2f);
        
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
