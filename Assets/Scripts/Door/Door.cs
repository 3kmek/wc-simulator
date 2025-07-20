using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, IInteractable
{
    public Cubicle Cubicle;
    
    
    [SerializeField] private GameObject voyeurVFX;
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactableLayer;

    [SerializeField] private Transform doorCenter;
    
    public Animator Animator;

    public bool isNPCLookingAndClose;

    [SerializeField] public bool isDoorOpen;
    [SerializeField] public GameObject OccupiedObstacle;
    private BoxCollider boxCollider;
    public WC.Toilet toilet;

    
    
    
    
    // Start is called before the first frame update
    
    
    
    public string GetInteractionText()
    {
        if (isDoorOpen)
        {
            return "Close the door [E]";
        }
        if (!isDoorOpen)
        {
            return "Open the door [E]";
        }
        else
        {
            return "";
        }

        
    }

    

    public void Interact()
    {
        if (!isDoorOpen)
        {
            OpenDoor();
            
        }

        else if (isDoorOpen)
        {
            CloseDoor();
        }
    }

    void Start()
    {
        toilet = transform.parent.GetChild(0).gameObject.GetComponent<WC.Toilet>();
        isDoorOpen = false;
        Animator = transform.parent.GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        OpenDoor();
        isDoorOpen = true;
        Cubicle.isCubicleBusy = false;
    }

    public void OpenDoor()
    {
        if (true)
        {
            isDoorOpen = true;
            Animator.Play("Door_Open");
            StartCoroutine(SetTriggerAfterClip("Door_Open", true));
            transform.parent.transform.GetChild(1).gameObject.SetActive(false);
            GetComponent<NavMeshObstacle>().enabled = false;
            
        }
    }

    public void CloseDoor()
    {
        if (true)
        {
            Animator.Play("Door_Close");
            StartCoroutine(SetTriggerAfterClip("Door_Close", false));
            isDoorOpen = false;
            transform.parent.transform.GetChild(1).gameObject.SetActive(true);
            GetComponent<NavMeshObstacle>().enabled = true;
        }
        else if (toilet.isNPCAssigned)
        {
            
        }
        
    }
    
    IEnumerator SetTriggerAfterClip(string clipName, bool state)
    {
        float len = GetClipLength(clipName);
        yield return new WaitForSeconds(len);
        boxCollider.isTrigger = state;
    }
    
    float GetClipLength(string clipName)
    {
        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
        
    }

    
    
    private void HandleRaycast()
    {
        
        Ray ray = new Ray(doorCenter.position, new Vector3(0,0,-doorCenter.position.normalized.z));
        
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer);
        
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green, 0.1f);
        if (!isDoorOpen)
        {
            if (hitSomething && hit.transform.CompareTag("Player"))
            {
            
                voyeurVFX.SetActive(true);
            }
            else
            {
            
                voyeurVFX.SetActive(false);
            }
        }
        
        
        if (hitSomething && hit.transform.GetComponent<NPCController>())
        {
            if (hit.transform.GetComponent<NPCController>().Cubicle.GetComponent<Cubicle>() == Cubicle )
            {
                isNPCLookingAndClose = true;
                //OpenDoor();
            }
            if (hit.transform.GetComponent<NPCController>().currentState == NPCState.PerformingAction)
            {
                isNPCLookingAndClose = false;
                //CloseDoor();
            }

            if (hit.transform.GetComponent<NPCController>().currentState == NPCState.PerformDone)
            {
                //OpenDoor();
            }
            
            

        }
        else
        {
            
            
        }
        
    }
}
