using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Hammer : MonoBehaviour, IInteractable, IHoldable
{
    [Header("Settings")]
    [SerializeField] private Vector3 _holdOffset = new Vector3(0, -0.5f, 0);
    
    
    [SerializeField] private Transform holder;
    [SerializeField] private GameObject player;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private MeshCollider hammerCollider;
    [SerializeField] private Vector3 tableHolderPos, tableHolderRot; 
    
    public bool IsHolding { get; private set; }
    public void Use()
    {
       //if (holdingAnimDone)
        //{
         //   buildManager.SelectItem();
       // }
    }
    
    private bool holdingAnimDone = false;
    private BuildManager buildManager;
    // Start is called before the first frame update
    
    void Start()
    {
        tableHolderPos = transform.position;
        tableHolderRot = transform.rotation.eulerAngles;
        Installation();
    }


    void Installation()
    {
        rb = GetComponent<Rigidbody>();
        hammerCollider = GetComponent<MeshCollider>();
        holder = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetChild(0);
        buildManager = GameObject.FindGameObjectWithTag("BuildManager").GetComponent<BuildManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public string GetInteractionText() => "Hold the Hammer\nPress [E]";

    public void Interact()
    {
        
        
    }

    public void OnPickup(Transform holdPosition)
    {
        
        rb.isKinematic = true;
        hammerCollider.enabled = false;
        
        // Önce parent'tan bağımsız hareket et
        transform.SetParent(null);
        transform.SetParent(holder);
        transform.position = holder.position;
        transform.rotation = holder.rotation;
        holdingAnimDone = true;
        
        player.GetComponent<PlayerInteraction>()._currentHeldObject = this.gameObject;
        player.GetComponent<PlayerInteraction>().isHoldingSomething = true;

    }

    

    public void OnDrop()
    {
        
        if (holdingAnimDone)
        {
            transform.SetParent(null);
            rb.isKinematic = false;
            hammerCollider.enabled = true;
            IsHolding = false;
        
        
            buildManager.CancelBuildMode();;
            holdingAnimDone = false;
            
            player.GetComponent<PlayerInteraction>()._currentHeldObject = null;
            player.GetComponent<PlayerInteraction>().isHoldingSomething = false;

            transform.DOMove(tableHolderPos, 0.5f).OnComplete(() =>
            {
                transform.DORotate(tableHolderRot, 0.5f);
            });
        }
        
        
    }

    
}