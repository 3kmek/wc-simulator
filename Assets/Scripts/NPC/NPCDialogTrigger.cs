using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using System.Collections;
using NPC;
using ScriptableObjects;

public class NPCDialogTrigger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI talkUI;
    [SerializeField] private TextMeshProUGUI inspectUI;
    [SerializeField] private TextMeshProUGUI kickUI;
    [SerializeField] private TextMeshProUGUI dialogTextUI;
    
    [Header("Dialog Settings")]
    [SerializeField] private float dialogDisplayDuration = 3f;
    [SerializeField] private float fadeOutDuration = 1f;
    
    [Header("Localization Settings")]
    [SerializeField] private string dialogTableReference = "TraitDialogTable";
    [SerializeField] private string firstMeetingKey = "first_meeting";
    [SerializeField] private string subsequentMeetingKey = "subsequent_meeting";
    [SerializeField] private string fallbackDialogKey = "generic_hello";
    
    // Private references
    private NPCController npc;
    private FirstPersonController firstPersonController;
    private PlayerInteraction playerInteraction;
    
    // Dialog state management
    private bool isItFirstDialogue = true;
    private bool isDialogueActive = false;
    private bool lookingAtNPC = false;
    private int dialogueCount = 0;
    
    // Coroutine reference for cleanup
    private Coroutine currentDialogCoroutine;

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

        if (playerSitting && isDialogueActive)
        {
            SetUIVisibility(false, false, false, true);
        }
        else if (playerSitting && !isDialogueActive)
        {
            SetUIVisibility(true, true, true, false);
        }
        else
        {
            HideAllUI();
        }
    }

    private void HideAllUI()
    {
        SetUIVisibility(false, false, false, false);
    }

    private void SetUIVisibility(bool talk, bool inspect, bool kick, bool dialog)
    {
        if (talkUI != null) talkUI.enabled = talk;
        if (inspectUI != null) inspectUI.enabled = inspect;
        if (kickUI != null) kickUI.enabled = kick;
        if (dialogTextUI != null) dialogTextUI.enabled = dialog;
    }

    private bool ValidateUIElements()
    {
        if (talkUI == null || inspectUI == null || kickUI == null || dialogTextUI == null)
        {
            Debug.LogError($"NPCDialogTrigger on {gameObject.name}: UI elementlerinden biri null! Inspector'dan tüm UI elementlerini atadığınızdan emin olun.");
            return false;
        }
        return true;
    }
    #endregion

    #region Dialog System - Localized
    public void StartDialogue(NPCTrait npcTrait)
    {
        if (isDialogueActive) return;

        dialogueCount++;
        isDialogueActive = true;

        // Localized dialog key'ini belirle
        string dialogKey = GetDialogKeyForState();
        DisplayLocalizedDialog(dialogKey);

        if (isItFirstDialogue)
        {
            isItFirstDialogue = false;
        }
    }

    private string GetDialogKeyForState()
    {
        // İlk karşılaşma için özel key
        if (isItFirstDialogue)
        {
            return firstMeetingKey;
        }

        // NPC trait'lerine göre dialog key havuzu oluştur
        List<string> availableKeys = GetDialogKeyPool();
        
        if (availableKeys.Count > 0)
        {
            return GetRandomDialogKey(availableKeys);
        }

        // Fallback key
        return fallbackDialogKey;
    }

    private void DisplayLocalizedDialog(string dialogKey)
    {
        var localizedString = new LocalizedString(dialogTableReference, dialogKey);

        // String değişikliğini dinle
        localizedString.StringChanged += OnLocalizedStringChanged;

        // String'i yenile ve göster
        localizedString.RefreshString();
    }

    private void OnLocalizedStringChanged(string localizedText)
    {
        // Localized text'i display et
        DisplayDialog(localizedText);
    }

    private void DisplayDialog(string text)
    {
        if (dialogTextUI != null)
        {
            dialogTextUI.text = text;
        }

        // Önceki coroutine'i durdur
        if (currentDialogCoroutine != null)
        {
            StopCoroutine(currentDialogCoroutine);
        }

        // Yeni dialog display coroutine başlat
        currentDialogCoroutine = StartCoroutine(DialogDisplayCoroutine());
    }

    private IEnumerator DialogDisplayCoroutine()
    {
        yield return new WaitForSeconds(dialogDisplayDuration);

        if (fadeOutDuration > 0)
        {
            yield return StartCoroutine(FadeOutDialog());
        }

        EndDialog();
    }

    private IEnumerator FadeOutDialog()
    {
        if (dialogTextUI == null) yield break;

        Color originalColor = dialogTextUI.color;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            dialogTextUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        dialogTextUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    private void EndDialog()
    {
        isDialogueActive = false;
        
        if (dialogTextUI != null)
        {
            Color color = dialogTextUI.color;
            dialogTextUI.color = new Color(color.r, color.g, color.b, 1f);
        }

        currentDialogCoroutine = null;
    }

    public void ForceEndDialog()
    {
        if (currentDialogCoroutine != null)
        {
            StopCoroutine(currentDialogCoroutine);
            currentDialogCoroutine = null;
        }
        EndDialog();
    }
    #endregion

    #region Interaction System
    // public string GetInteractionText()
    // {
    //     if (npc.currentState == NPCState.WaitingForApproval && firstPersonController.IsPlayerSitting)
    //     {
    //         if (isDialogueActive)
    //             return null;
    //         else
    //             return "Talk\n[E]";
    //     }
    //     return null;
    // }
    //
    // public void Interact()
    // {
    //     if (npc.currentState != NPCState.WaitingForApproval || isDialogueActive) 
    //         return;
    //
    //     // E tuşuna basıldığında da localized dialog göster
    //     List<string> keyPool = GetDialogKeyPool();
    //     string selectedKey = GetRandomDialogKey(keyPool);
    //     
    //     if (string.IsNullOrEmpty(selectedKey))
    //     {
    //         selectedKey = fallbackDialogKey;
    //     }
    //     
    //     DisplayLocalizedDialog(selectedKey);
    // }

    private List<string> GetDialogKeyPool()
    {
        List<string> keyPool = new List<string>();

        // NPC'nin aktif trait'lerinden dialog key'lerini topla
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

        return keyPool;
    }

    private string GetRandomDialogKey(List<string> keyPool)
    {
        if (keyPool == null || keyPool.Count == 0)
        {
            return fallbackDialogKey;
        }
        
        return keyPool[Random.Range(0, keyPool.Count)];
    }
    #endregion

    #region Debug and Utility
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void LogDebugInfo(string message)
    {
        Debug.Log($"[NPCDialogTrigger - {gameObject.name}] {message}");
    }

    [ContextMenu("Test Start Dialog")]
    private void TestStartDialog()
    {
        if (npc != null && npc.activeTraits != null && npc.activeTraits.Count > 0)
        {
            StartDialogue(npc.activeTraits[0]);
        }
    }

    [ContextMenu("Force End Dialog")]
    private void TestEndDialog()
    {
        ForceEndDialog();
    }

    [ContextMenu("Test Localization")]
    private void TestLocalization()
    {
        DisplayLocalizedDialog(fallbackDialogKey);
    }
    #endregion
}