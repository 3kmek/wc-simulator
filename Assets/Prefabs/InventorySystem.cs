using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private static InventorySystem _instance;
    public static InventorySystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventorySystem>();
                if (_instance == null)
                {
                    Debug.LogError("InventorySystem bulunamadı! Sahneye eklenmiş mi?");
                }
            }
            return _instance;
        }
    }

    public GameObject Slot = null;
    private PlayerInteraction playerInteraction;

    public bool holderHasSomething = false;

    private IHoldable _holdable;
    private void Start()
    {
        
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnyHoldingItem();
        HandleDropInput();
    }

    
    // sunu emekle yazdim amk
    void HandleAnyHoldingItem()
    {
        if (playerInteraction._currentHeldObject != null)
        {
            if (playerInteraction.isHoldingSomething)
            {
                Slot = playerInteraction._currentHeldObject.gameObject;
                holderHasSomething = true;
                _holdable = Slot.GetComponent<IHoldable>();
                _holdable.Use();
            }
        }
        if (!playerInteraction.isHoldingSomething && playerInteraction._currentHeldObject == null)
        {
            Slot = null;
            holderHasSomething = false;
        }
    }
    
    private void HandleDropInput()
    {
        if (Input.GetKeyDown(KeyCode.G) && playerInteraction._currentHeldObject != null)
        {
            IHoldable holdable = playerInteraction._currentHeldObject.GetComponent<IHoldable>();
            holdable?.OnDrop();
            //_currentHeldObject = null;
        }
    }
    
}
