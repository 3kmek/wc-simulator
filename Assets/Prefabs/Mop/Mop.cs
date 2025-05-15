using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;

public class Mop : MonoBehaviour, IHoldable, IInteractable
{
    [SerializeField] private Transform holder;
    [SerializeField] private GameObject player;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private BoxCollider collider1, collider2;

    [SerializeField] private Vector3 offset;
    [SerializeField] private CleaningManager cleaningManager;


    [SerializeField] public int ShitAmountOfMop = 0;
    [SerializeField] public BoxCollider interactBox;
    [SerializeField] public Animator animator;
    [SerializeField] public AnimationClip anim;

    [SerializeField] public int MopCapacity;
    [SerializeField] public int MopCurrentFilth;
    
    
    [SerializeField] private Renderer rend;
    [SerializeField] private Texture defaultTexture;
    [SerializeField] private Texture filthTexture;
    
    [SerializeField] private bool didTouch = false;
    
    private bool isAnimating = false;

    [SerializeField] private GameObject mopHolderObject;
    [SerializeField] Vector3 mopHolderPos;
    [SerializeField] Vector3 mopHolderRot;
    
    private bool holdingAnimDone = false;
    // Start is called before the first frame update
    void Start()
    {
        Installation();
    }

    private void Update()
    {
        HandleTexture();
        HandleWarningText();
    }

    void HandleTexture()
    {
        // Texture değiş
        if (MopCurrentFilth == MopCapacity)
        {
            //rend.material.SetTexture("_MainTex", filthTexture);;
            rend.material.mainTexture = filthTexture;
        }
        else
        {
            //rend.material.SetTexture("Texture2D_4450AB74", defaultTexture);
            rend.material.mainTexture = defaultTexture;
        }
    }

    void HandleWarningText()
    {
        if (MopCurrentFilth == MopCapacity && InventorySystem.Instance.Slot == this.gameObject)
        {
            UITextManager.Instance.warningPrompt.text = "Mop is very dirty! You need to clean it up.";
            UITextManager.Instance.warningPrompt.enabled = true;
        }

        else
        {
            UITextManager.Instance.warningPrompt.enabled = false;
        }
    }


    void Installation()
    {
        
        rend = gameObject.GetComponent<Renderer>();
        defaultTexture = rend.material.mainTexture;
        rb = GetComponent<Rigidbody>();
        collider1 = GetComponent<BoxCollider>();
        holder = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetChild(0);
        cleaningManager = GameObject.FindGameObjectWithTag("CleaningManager").GetComponent<CleaningManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        interactBox = transform.GetChild(0).GetComponent<BoxCollider>();
        animator = transform.GetComponent<Animator>();
        mopHolderPos = mopHolderObject.transform.position;
        mopHolderRot = mopHolderObject.transform.rotation.eulerAngles;
    }

    public void OnPickup(Transform holdPosition)
    {
        animator.enabled = true;
        rb.isKinematic = true;
        collider1.enabled = false;
        collider2.enabled = false;
        
        transform.parent.SetParent(null);
        transform.parent.SetParent(holder);
        transform.parent.position = holder.position;
        transform.parent.localPosition = new Vector3(0.540000021f,1.00999999f,1.01999998f);;
        
        transform.parent.rotation = holder.rotation;
        transform.parent.localRotation = new Quaternion(-0.232002452f, 0, 0, 0.972715259f);
        
        holdingAnimDone = true;
        
        CleaningManager.Instance.currentMop = this.GetComponent<Mop>();
        
        player.GetComponent<PlayerInteraction>()._currentHeldObject = this.gameObject;
        player.GetComponent<PlayerInteraction>().isHoldingSomething = true;
    }

    public void OnDrop()
    {
        if (holdingAnimDone)
        {
            animator.enabled = false;
            transform.parent.SetParent(null);
            transform.parent.SetParent(mopHolderObject.transform);
            rb.isKinematic = true;
            collider1.enabled = true;
            collider2.enabled = true;
            IsHolding = false;
        
            
        
            
            holdingAnimDone = false;
            
            player.GetComponent<PlayerInteraction>()._currentHeldObject = null;
            player.GetComponent<PlayerInteraction>().isHoldingSomething = false;

            transform.DOMove(mopHolderPos, 0.5f).OnComplete(() =>
            {
                transform.DORotate(mopHolderRot, 0.5f);
            });
        }
    }

    public bool IsHolding { get; private set; }
    public void Use()
    {
        // play anim
        
        
        
        if (animator != null && isAnimating == false)
        {
            
            
            animator.Play("MopAnim", 0, 0f);
            
            StartCoroutine(ResetMopAnim());
            
            
            isAnimating = true;
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (didTouch == false && (isAnimating == true) && (other.GetComponent<WC.Poop>() != null || other.GetComponent<Dirt>() != null) && MopCurrentFilth < MopCapacity)
        {
            didTouch = true;
            other.transform.DOScale(Vector3.zero, 1f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    Destroy(other.gameObject);
                });

           
            MopCurrentFilth++;
            
            
            // Text koy
            
            
        }
        
    }

    private IEnumerator ResetMopAnim()
    {
        yield return new WaitForSeconds(anim.length); // animasyon süresi kadar bekle
        isAnimating = false;
        didTouch = false;
    }

    public string GetInteractionText() => "Hold the Mop\nPress [E]";

    public void Interact()
    {
        
    }
}
