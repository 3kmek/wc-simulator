using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance;
    
    [SerializeField] public Mop currentMop;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (InventorySystem.Instance.Slot != null)
        {
            if (InventorySystem.Instance.Slot.GetComponent<Mop>() != null)
            {
                HandleCleaningMovement();
                //HandlePlacingObject();
                //HandleKeyboardInput();
                //HandleMouseScroll();
                GridSystem.Instance.RemoveObject();
            }
        }
    }

    void HandleCleaningMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // PLAY ANIM
            if (currentMop.ShitAmountOfMop < 3)
            {
                currentMop.Use();
            }
        }
    }

    void HandleCleaningStatus()
    {
        if (true)
        {
                
        }
    }
}
