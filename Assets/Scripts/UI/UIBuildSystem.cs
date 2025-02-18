using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBuildSystem : MonoBehaviour
{
    private static UIBuildSystem _instance;
    public static UIBuildSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIBuildSystem>();
                if (_instance == null)
                {
                    Debug.LogError("UIBuildSystem bulunamadı! Sahneye eklediniz mi?");
                }
            }
            return _instance;
        }
    }
    
    public GraphicRaycaster raycaster; // UI raycast işlemi için
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    public bool isBuildingMenuOn = false;

    
    
    public Button selectedMainButton;
    public Button selectedItemButton;
    [SerializeField] GameObject buildPanel;
    [SerializeField] FirstPersonController firstPersonController;
    
    [SerializeField] TextMeshProUGUI demolishText;
    [SerializeField] string demolishBaseText = "[Q] to demolish for ";
    [SerializeField] private float removeRange = 5f;
    
    private void Start()
    {
        demolishBaseText = "[Q] to demolish for ";
        eventSystem = EventSystem.current;
        pointerEventData = new PointerEventData(eventSystem);
        firstPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        

        HandleOpenSidePanelWithRaycast();

        
        HandleShowBuildPanel();
    }
    
    private void HandleShowBuildPanel()
    {
        if (InventorySystem.Instance.Slot != null && InventorySystem.Instance.Slot.GetComponent<Hammer>() != null)
        {
            
            if (Input.GetMouseButtonDown(1))
            {
                isBuildingMenuOn = true;
                buildPanel.SetActive(true);
                firstPersonController.cameraCanMove = false;
                Cursor.lockState = CursorLockMode.None;
            }
            
            else if (Input.GetMouseButtonUp(1))
            {
                isBuildingMenuOn = false;
                buildPanel.SetActive(false);
                firstPersonController.cameraCanMove = true;
                Cursor.lockState = CursorLockMode.Locked;
                selectedMainButton.interactable = true;
                selectedMainButton.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        
    }

    void HandleShowDemolishPanel()
    {
        
    }

    public void SetShowDemolishText()
    {
        
        if (InventorySystem.Instance.Slot != null && InventorySystem.Instance.Slot.GetComponent<Hammer>() != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.transform.GetComponent<Breakable>() != null && hit.distance < removeRange)
                {
                    Breakable breakable = hit.transform.GetComponent<Breakable>();
                    demolishText.enabled = true;
                    demolishText.text = demolishBaseText + breakable.GoldForBreak;
                }
                else
                {
                    demolishText.enabled = false;
                    demolishText.text = "";
                }
            }
            
        }
        else
        {
            demolishText.enabled = false;
            demolishText.text = "";
        }
    }

    void HandleOpenSidePanelWithRaycast()
    {
        if(isBuildingMenuOn)
        {
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            bool isOverUIImage = false;

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("UIElement"))
                {
                    if (selectedMainButton != null)
                    {
                        selectedMainButton.transform.GetChild(0).gameObject.SetActive(false);
                        selectedMainButton.interactable = true;
                    }

                    selectedMainButton = result.gameObject.GetComponent<Button>();
                    isOverUIImage = true;
                    selectedMainButton.transform.GetChild(0).gameObject.SetActive(isOverUIImage);
                    selectedMainButton.interactable = false;
                    break;
                }
                else
                {

                }
            }
        }
    }
    
    
    public void OpenSidePanel(Button button)
    {
        if (selectedMainButton != null)
        {
            selectedMainButton.transform.GetChild(0).gameObject.SetActive(false);
            selectedMainButton.interactable = true;
        }
        selectedMainButton = button; 
        selectedMainButton.transform.GetChild(0).gameObject.SetActive(true);
        selectedMainButton.interactable = false;
    }

    public void CloseOtherSidePanel()
    {
        
        
    }

    public void SelectItemOnSidePanel(int itemIndex)
    {
        BuildManager.Instance.SelectItem(itemIndex);
    }
    public void SelectAndUnselectItemButton(Button button)
    {
        selectedItemButton.interactable = true;
        
        selectedItemButton = button;
        selectedItemButton.interactable = false;
    }
    
    
    
}
