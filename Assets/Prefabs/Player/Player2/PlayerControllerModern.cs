using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using TMPro;

public class PlayerMovementModern : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    
    
    [SerializeField] float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] Camera playerCamera;
    
    private void Start()
    {
        playerCamera = Camera.main;
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        
        CheckForInteractable();
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }
        
        
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
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
                     // Mesajı göster
                    if (hit.transform.GetComponent<NPCController>())
                    {
                        NPCController npc = hit.transform.GetComponent<NPCController>();
                        if (npc.currentState == NPCState.KeyGiver)
                        {
                            interactPrompt.enabled = true;
                            interactPrompt.text = "Recieve The Key\nPress [E]\n";
                            
                        }
                    }

                    if (hit.transform.GetComponent<Key>())
                    {
                        interactPrompt.enabled = true;
                        interactPrompt.text = "Give The Key \n" + hit.transform.GetComponent<Key>().genderOfKey + "\nPress [E]\n";
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
}