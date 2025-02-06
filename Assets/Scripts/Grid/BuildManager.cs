using System;
using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public bool isBuildingMode = false;

    // Inspector’dan ayarlayacağınız Real/Ghost eşleşmeleri
    [Header("Buildable Items (Real + Ghost)")]
    public List<BuildablePair> buildableItems;

    // Seçilmiş index
    private int currentIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (InventorySystem.Instance.Slot != null)
        {
            if (InventorySystem.Instance.Slot.GetComponent<Hammer>() != null)
            {
                HandlePlacingObject();
                HandleKeyboardInput();
                HandleMouseScroll();
                GridSystem.Instance.RemoveObject();
            }
        }
    }

    private void HandlePlacingObject()
    {
        // Sol tık -> Yerleştirmeyi dene
        if (Input.GetMouseButtonDown(0))
        {
            if (GridSystem.Instance.CanPlaceObject())
            {
                GridSystem.Instance.PlaceObject();
            }
        }
    }
    
    private void HandleKeyboardInput()
    {
        // klavyeden 1–8’e basınca buildableItems içindeki ilgili index’i seçiyoruz
        for (int i = 0; i < Mathf.Min(buildableItems.Count, 8); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectItem(i);
                break;
            }
        }
    }
    
    private void HandleMouseScroll()
    {
        // Fare tekeri ile sıradaki buildable eşyaya geç
        if (buildableItems.Count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? 1 : -1;
            currentIndex += direction;

            // Döngüsel (wrap) hale getirelim
            currentIndex = (currentIndex + buildableItems.Count) % buildableItems.Count;
            SelectItem(currentIndex);
        }
    }

    // UI veya menüden de çağrılabilir
    public void SelectItem(int index)
    {
        if (index < 0 || index >= buildableItems.Count) return;
        
        currentIndex = index;
        isBuildingMode = true;

        // GridSystem’e realPrefab ve ghostPrefab veriyoruz
        BuildablePair pair = buildableItems[index];
        GridSystem.Instance.SetObjectToPlace(pair.realPrefab, pair.ghostPrefab);
    }

    // Build modu kapatmak için
    public void CancelBuildMode()
    {
        isBuildingMode = false;
        GridSystem.Instance.CancelPlacement();
    }
}
