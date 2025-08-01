using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    /// <summary>
    /// NPC'ler için dialog seçeneklerini yöneten sınıf
    /// </summary>
    [System.Serializable]
    public class NPCDialogProfile : MonoBehaviour
    {
        [Header("Dialog Sequences")]
        [SerializeField] private DialogSequence[] dialogSequences;
        
        [Header("State")]
        [SerializeField] private bool hasMetBefore = false;
        [SerializeField] private int interactionCount = 0;
        
        // NPC trait reference
        private NPC.NPCController npcController;
        
        void Start()
        {
            npcController = GetComponent<NPC.NPCController>();
        }
        
        /// <summary>
        /// Mevcut duruma göre uygun dialog seçeneklerini döndürür
        /// </summary>
        public List<string> GetAvailableDialogOptions()
        {
            List<string> availableOptions = new List<string>();
            
            // İlk karşılaşma kontrolü
            if (!hasMetBefore)
            {
                var firstMeetingSequence = GetSequenceByType(DialogType.FirstMeeting);
                if (firstMeetingSequence != null)
                {
                    availableOptions.Add(firstMeetingSequence.GetCurrentDialogKey());
                }
            }
            else
            {
                // Sonraki karşılaşmalar
                var subsequentSequence = GetSequenceByType(DialogType.SubsequentMeeting);
                if (subsequentSequence != null)
                {
                    availableOptions.Add(subsequentSequence.GetCurrentDialogKey());
                }
            }
            
            // Trait-specific dialog'lar
            if (npcController != null && npcController.activeTraits != null)
            {
                foreach (var trait in npcController.activeTraits)
                {
                    var traitSequence = GetSequenceByTraitName(trait.traitName);
                    if (traitSequence != null)
                    {
                        availableOptions.Add(traitSequence.GetCurrentDialogKey());
                    }
                }
            }
            
            // Generic options
            var genericSequence = GetSequenceByType(DialogType.Generic);
            if (genericSequence != null && availableOptions.Count < 3)
            {
                availableOptions.Add(genericSequence.GetCurrentDialogKey());
            }
            
            return availableOptions;
        }
        
        /// <summary>
        /// Belirli bir dialog key'i seçildiğinde çağrılır
        /// </summary>
        public void OnDialogSelected(string selectedKey)
        {
            interactionCount++;
            
            if (!hasMetBefore)
            {
                hasMetBefore = true;
            }
            
            // İlgili sequence'ı ilerlet
            var sequence = FindSequenceContainingKey(selectedKey);
            if (sequence != null)
            {
                sequence.GetNextDialogKey(); // Index'i ilerlet
            }
            
            Debug.Log($"Dialog selected: {selectedKey}, Interaction count: {interactionCount}");
        }
        
        private DialogSequence GetSequenceByType(DialogType type)
        {
            if (dialogSequences == null) return null;
            
            foreach (var sequence in dialogSequences)
            {
                if (sequence.dialogType == type)
                    return sequence;
            }
            return null;
        }
        
        private DialogSequence GetSequenceByTraitName(string traitName)
        {
            if (dialogSequences == null) return null;
            
            foreach (var sequence in dialogSequences)
            {
                if (sequence.requiresSpecificTrait && sequence.requiredTraitName == traitName)
                    return sequence;
            }
            return null;
        }
        
        private DialogSequence FindSequenceContainingKey(string key)
        {
            if (dialogSequences == null) return null;
            
            foreach (var sequence in dialogSequences)
            {
                if (sequence.dialogKeys != null)
                {
                    foreach (string dialogKey in sequence.dialogKeys)
                    {
                        if (dialogKey == key)
                            return sequence;
                    }
                }
            }
            return null;
        }
        
        /// <summary>
        /// Debug için - tüm sequence'ları resetle
        /// </summary>
        [ContextMenu("Reset All Sequences")]
        public void ResetAllSequences()
        {
            if (dialogSequences != null)
            {
                foreach (var sequence in dialogSequences)
                {
                    sequence.ResetSequence();
                }
            }
            hasMetBefore = false;
            interactionCount = 0;
        }
    }
}
