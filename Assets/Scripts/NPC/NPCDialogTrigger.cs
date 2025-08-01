using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using NPC;
using ScriptableObjects;
using DialogSystem; // Yeni namespace

public class NPCDialogTrigger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI talkUI;
    [SerializeField] private TextMeshProUGUI inspectUI;
    [SerializeField] private TextMeshProUGUI kickUI;
    [SerializeField] private TextMeshProUGUI dialogTextUI;
    
    [Header("Dialog Options UI")]
    [SerializeField] private TextMeshProUGUI dialogOption1UI;
    [SerializeField] private TextMeshProUGUI dialogOption2UI;
    [SerializeField] private TextMeshProUGUI dialogOption3UI;
    [SerializeField] private TextMeshProUGUI dialogBackUI;
    
    [Header("Dialog Settings")]
    [SerializeField] private string dialogTableReference = "TraitDialogTable";
    [SerializeField] private float dialogDisplayDuration = 3f;
    
    // Private references
    private NPCController npc;
    private FirstPersonController firstPersonController;
    private PlayerInteraction playerInteraction;
    private NPCDialogProfile dialogProfile;
    
    // Dialog state management
    private bool isDialogueActive = false;
    private bool lookingAtNPC = false;
    private bool isInDialogMode = false;
    private int dialogueCount = 0;
    
    // Dialog options
    [SerializeField]private List<string> currentDialogOptions = new List<string>();
    private Dictionary<string, string> localizedTexts = new Dictionary<string, string>();
    private int selectedDialogIndex = 0;

    #region Unity Lifecycle
    void Start()
    {
        InitializeComponents();
        SubscribeToEvents();
        ValidateUIElements();
    }

    void Update()
    {
        HandleUIVisibility();
        
        if (isInDialogMode && firstPersonController.IsPlayerSitting && !firstPersonController.isZoomed)
        {
            HandleDialogInput();
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        npc = GetComponent<NPCController>();
        dialogProfile = GetComponent<NPCDialogProfile>();
        
        // Eğer DialogProfile yoksa ekle
        if (dialogProfile == null)
        {
            dialogProfile = gameObject.AddComponent<NPCDialogProfile>();
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            firstPersonController = player.GetComponent<FirstPersonController>();
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }
        
        if (npc == null || firstPersonController == null || playerInteraction == null)
        {
            Debug.LogError($"NPCDialogTrigger on {gameObject.name}: Missing required components!");
        }
    }

    private void SubscribeToEvents()
    {
        if (npc != null)
            npc.OnDialogueEnter += StartDialogue;
            
        if (playerInteraction != null)
        {
            playerInteraction.PlayerLookingAtNPCWhileSitting += OnLookingAtNPC;
            playerInteraction.PlayerNOTLookingAtNPCWhileSitting += OnNotLookingAtNPC;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (npc != null)
            npc.OnDialogueEnter -= StartDialogue;
            
        if (playerInteraction != null)
        {
            playerInteraction.PlayerLookingAtNPCWhileSitting -= OnLookingAtNPC;
            playerInteraction.PlayerNOTLookingAtNPCWhileSitting -= OnNotLookingAtNPC;
        }
    }
    #endregion

    #region Event Handlers
    private void OnLookingAtNPC()
    {
        lookingAtNPC = true;
    }

    private void OnNotLookingAtNPC()
    {
        lookingAtNPC = false;
    }
    #endregion

    #region UI Management
    private void HandleUIVisibility()
    {
        if (!ValidateUIElements()) return;

        bool shouldShowUI = lookingAtNPC && !firstPersonController.isZoomed;
        
        if (shouldShowUI)
        {
            ShowContextualUI();
        }
        else
        {
            HideAllUI();
        }
    }

    private void ShowContextualUI()
    {
        if (npc.currentState != NPCState.WaitingForApproval)
        {
            HideAllUI();
            return;
        }

        bool playerSitting = firstPersonController.IsPlayerSitting;

        if (playerSitting && isInDialogMode)
        {
            SetUIVisibility(false, false, false, false);
            SetDialogOptionsVisibility(true);
        }
        else if (playerSitting && isDialogueActive)
        {
            SetUIVisibility(false, false, false, true);
            SetDialogOptionsVisibility(false);
        }
        else if (playerSitting && !isDialogueActive)
        {
            SetUIVisibility(true, true, true, false);
            SetDialogOptionsVisibility(false);
        }
        else
        {
            HideAllUI();
        }
    }

    private void HideAllUI()
    {
        SetUIVisibility(false, false, false, false);
        SetDialogOptionsVisibility(false);
    }

    private void SetUIVisibility(bool talk, bool inspect, bool kick, bool dialog)
    {
        if (talkUI != null) talkUI.enabled = talk;
        if (inspectUI != null) inspectUI.enabled = inspect;
        if (kickUI != null) kickUI.enabled = kick;
        if (dialogTextUI != null) dialogTextUI.enabled = dialog;
    }
    
    private void SetDialogOptionsVisibility(bool visible)
    {
        if (dialogOption1UI != null) dialogOption1UI.enabled = visible;
        if (dialogOption2UI != null) dialogOption2UI.enabled = visible;
        if (dialogOption3UI != null) dialogOption3UI.enabled = visible;
        if (dialogBackUI != null) dialogBackUI.enabled = visible;
    }

    private bool ValidateUIElements()
    {
        if (talkUI == null || inspectUI == null || kickUI == null || dialogTextUI == null)
        {
            Debug.LogError($"NPCDialogTrigger on {gameObject.name}: UI elementlerinden biri null!");
            return false;
        }
        return true;
    }
    #endregion

    #region Dialog Options System
    public void EnterDialogMode()
    {
        isInDialogMode = true;
        selectedDialogIndex = 0;
        
        // DialogProfile'dan mevcut dialog seçeneklerini al
        if (dialogProfile != null)
        {
            currentDialogOptions = dialogProfile.GetAvailableDialogOptions();
        }
        else
        {
            // Fallback - eski sistem
            currentDialogOptions = GetDialogKeyPool();
        }
        
        // Localized text'leri async olarak al
        LoadLocalizedDialogOptions();
        
//        Debug.Log("Entered dialog mode");
    }
    
    public void ExitDialogMode()
    {
        isInDialogMode = false;
        currentDialogOptions.Clear();
        localizedTexts.Clear();
        Debug.Log("Exited dialog mode");
    }
    
    private void LoadLocalizedDialogOptions()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager instance not found!");
            UpdateDialogOptionsUI(); // UI'yi güncelle (key'ler ile)
            return;
        }
        
        // Tüm dialog key'leri için localized text'leri al
        string[] keys = currentDialogOptions.ToArray();
        LocalizationManager.Instance.GetMultipleLocalizedTexts(keys, (results) =>
        {
            localizedTexts = results;
            UpdateDialogOptionsUI();
        });
    }
    
    private void HandleDialogInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentDialogOptions.Count > 0)
        {
            SelectDialogOption(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentDialogOptions.Count > 1)
        {
            SelectDialogOption(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentDialogOptions.Count > 2)
        {
            SelectDialogOption(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Q))
        {
            ExitDialogMode();
        }
    }
    
    private void SelectDialogOption(int optionIndex)
    {
        if (optionIndex >= 0 && optionIndex < currentDialogOptions.Count)
        {
            string selectedDialogKey = currentDialogOptions[optionIndex];
            
            // DialogProfile'a seçimi bildir
            if (dialogProfile != null)
            {
                dialogProfile.OnDialogSelected(selectedDialogKey);
            }
            
            // Seçilen dialog'u göster
            DisplayDialogFromKey(selectedDialogKey);
            
            // Dialog mode'dan çık ve cevabı göster
            ExitDialogMode();
            
            Debug.Log($"Selected dialog option {optionIndex + 1}: {selectedDialogKey}");
        }
    }
    
    private void UpdateDialogOptionsUI()
    {
        // Dialog option text'lerini güncelle
        if (dialogOption1UI != null)
        {
            if (currentDialogOptions.Count > 0)
            {
                string dialogText = GetLocalizedOrKeyText(currentDialogOptions[0]);
                dialogOption1UI.text = $"1. {dialogText}";
            }
            else
            {
                dialogOption1UI.text = "";
            }
        }
        
        if (dialogOption2UI != null)
        {
            if (currentDialogOptions.Count > 1)
            {
                string dialogText = GetLocalizedOrKeyText(currentDialogOptions[1]);
                dialogOption2UI.text = $"2. {dialogText}";
            }
            else
            {
                dialogOption2UI.text = "";
            }
        }
        
        if (dialogOption3UI != null)
        {
            if (currentDialogOptions.Count > 2)
            {
                string dialogText = GetLocalizedOrKeyText(currentDialogOptions[2]);
                dialogOption3UI.text = $"3. {dialogText}";
            }
            else
            {
                dialogOption3UI.text = "";
            }
        }
        
        if (dialogBackUI != null)
        {
            dialogBackUI.text = "4. Go Back [Q]";
        }
    }
    
    private string GetLocalizedOrKeyText(string key)
    {
        // Önce cache'den localized text'i kontrol et
        if (localizedTexts.ContainsKey(key))
        {
            return localizedTexts[key];
        }
        
        // Cache'de yoksa LocalizationManager'dan al
        if (LocalizationManager.Instance != null)
        {
            return LocalizationManager.Instance.GetLocalizedText(key);
        }
        
        // Son çare olarak key'i döndür
        return key;
    }
    #endregion

    #region Dialog System
    public void StartDialogue(NPCTrait npcTrait)
    {
        EnterDialogMode();
    }
    
    private void DisplayDialogFromKey(string dialogKey)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.GetLocalizedTextAsync(dialogKey, (localizedText) =>
            {
                DisplayDialog(localizedText);
            });
        }
        else
        {
            DisplayDialog(dialogKey); // Fallback
        }
        
        dialogueCount++;
    }

    private void DisplayDialog(string text)
    {
        if (dialogTextUI != null)
        {
            dialogTextUI.text = text;
            isDialogueActive = true;
        }
        
        StartCoroutine(DialogDisplayCoroutine());
    }

    private IEnumerator DialogDisplayCoroutine()
    {
        yield return new WaitForSeconds(dialogDisplayDuration);
        EndDialog();
    }

    private void EndDialog()
    {
        isDialogueActive = false;
        
        if (dialogTextUI != null)
        {
            dialogTextUI.text = "";
        }
    }

    public void ForceEndDialog()
    {
        EndDialog();
        ExitDialogMode();
    }
    #endregion

    #region Legacy Support - Eski sistem için
    private List<string> GetDialogKeyPool()
    {
        List<string> keyPool = new List<string>();

        if (npc != null && npc.activeTraits != null)
        {
            foreach (var trait in npc.activeTraits)
            {
                if (trait != null && trait.dialogKeys != null)
                {
                    keyPool.AddRange(trait.dialogKeys);
                }
            }
        }

        // Eğer hiç key yoksa default'ları ekle
        if (keyPool.Count == 0)
        {
            keyPool.Add("NORMAL_1");
            keyPool.Add("NORMAL_2");
        }

        return keyPool;
    }
    #endregion

    #region Debug and Utility
    [ContextMenu("Test Enter Dialog Mode")]
    private void TestEnterDialogMode()
    {
        EnterDialogMode();
    }

    [ContextMenu("Test Exit Dialog Mode")]
    private void TestExitDialogMode()
    {
        ExitDialogMode();
    }

    [ContextMenu("Force End Dialog")]
    private void TestEndDialog()
    {
        ForceEndDialog();
    }
    #endregion
}