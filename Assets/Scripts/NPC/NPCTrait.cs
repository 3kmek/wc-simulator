using UnityEngine;
using DialogSystem;

namespace ScriptableObjects
{
    public enum TraitCategory
    {
        Core,
        GradeImpact,
        Economy,
        Curse,
        Maintenance
    }

    [CreateAssetMenu(fileName = "NewTrait", menuName = "ScriptableObjects/NPC Trait", order = 2)]
    public class NPCTrait : ScriptableObject
    {
        [Header("Temel Bilgi")]
        public GameObject NPCPrefab; // Sadece Core trait'lerde dolu olacak
        public string traitName;
        [TextArea] public string description;
        public TraitCategory category;

        [Header("Olasılık")]
        [Range(0f, 1f)] public float spawnChance = 1f; // Core trait için spawn olasılığı
        [Range(0f, 1f)] public float triggerChance = 1f; // Side trait için tetiklenme olasılığı

        [Header("Etkiler")]
        public int tokenDelta;
        public float gradeMultiplier = 1f;
        public int moneyDelta;
        public int curseDelta;
        public bool blocksPayment;
        public bool consumesExtraSupply;

        [Header("İpucu")]
        public GameObject visualHintPrefab;
        public AudioClip voiceHint;
        
        [Header("Dialog Keys (Legacy - Backward Compatibility)")]
        [Tooltip("Eski sistem ile uyumluluk için - Yeni sistem DialogSequence kullanır")]
        public string[] dialogKeys;

        [Header("Dialog Sequences (New System)")]
        [Tooltip("Yeni dialog sistemi - sıralı ve koşullu dialog'lar")]
        public DialogSequence[] dialogSequences;

        [Header("Uyumlu Side Trait'ler (Sadece Core için)")]
        public NPCTrait[] compatibleSideTraits; // Bu core trait ile uyumlu yan trait'ler

        /// <summary>
        /// Bu trait için mevcut dialog sequence'larını döndürür
        /// </summary>
        public DialogSequence[] GetDialogSequences()
        {
            return dialogSequences ?? new DialogSequence[0];
        }

        /// <summary>
        /// Belirli bir dialog type için sequence döndürür
        /// </summary>
        public DialogSequence GetSequenceByType(DialogType type)
        {
            if (dialogSequences == null) return null;

            foreach (var sequence in dialogSequences)
            {
                if (sequence.dialogType == type)
                    return sequence;
            }
            return null;
        }

        /// <summary>
        /// Trait-specific dialog sequence'ı döndürür
        /// </summary>
        public DialogSequence GetTraitSpecificSequence()
        {
            return GetSequenceByType(DialogType.TraitSpecific);
        }

        /// <summary>
        /// Legacy support - eski dialog key sistemini destekler
        /// </summary>
        public string[] GetLegacyDialogKeys()
        {
            return dialogKeys ?? new string[0];
        }

        /// <summary>
        /// Tüm dialog key'lerini döndürür (yeni ve eski sistem)
        /// </summary>
        public string[] GetAllDialogKeys()
        {
            var allKeys = new System.Collections.Generic.List<string>();

            // Yeni sistem - sequence'lardan key'leri topla
            if (dialogSequences != null)
            {
                foreach (var sequence in dialogSequences)
                {
                    if (sequence.dialogKeys != null)
                    {
                        allKeys.AddRange(sequence.dialogKeys);
                    }
                }
            }

            // Legacy support - eski key'leri ekle
            if (dialogKeys != null)
            {
                allKeys.AddRange(dialogKeys);
            }

            return allKeys.ToArray();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Inspector'da dialog preview için
        /// </summary>
        [Header("Debug Info")]
        [SerializeField, TextArea] private string debugInfo;

        void OnValidate()
        {
            // Debug bilgisini güncelle
            var allKeys = GetAllDialogKeys();
            debugInfo = $"Total Dialog Keys: {allKeys.Length}\n";
            debugInfo += $"Sequences: {(dialogSequences?.Length ?? 0)}\n";
            debugInfo += $"Legacy Keys: {(dialogKeys?.Length ?? 0)}";
        }
#endif
    }

    /// <summary>
    /// Inspector'da dialog sequence oluşturmak için helper sınıf
    /// </summary>
    [System.Serializable]
    public class TraitDialogSequencePreset
    {
        [Header("Common Presets")]
        public bool createFirstMeetingSequence = true;
        public bool createSubsequentMeetingSequence = true;
        public bool createTraitSpecificSequence = true;
        public bool createGenericSequence = false;

        [Header("Auto Key Generation")]
        public bool autoGenerateKeys = true;
        public string keyPrefix = "TRAIT_";

        /// <summary>
        /// Bu trait için otomatik dialog sequence'ları oluşturur
        /// </summary>
        public DialogSequence[] GenerateSequences(string traitName)
        {
            var sequences = new System.Collections.Generic.List<DialogSequence>();

            if (createFirstMeetingSequence)
            {
                sequences.Add(new DialogSequence
                {
                    sequenceName = "First Meeting",
                    dialogType = DialogType.FirstMeeting,
                    dialogKeys = autoGenerateKeys ? 
                        new string[] { $"{keyPrefix}{traitName.ToUpper()}_FIRST1", $"{keyPrefix}{traitName.ToUpper()}_FIRST2" } :
                        new string[] { "FIRST_MEETING_1", "FIRST_MEETING_2" },
                    isFirstMeeting = true
                });
            }

            if (createSubsequentMeetingSequence)
            {
                sequences.Add(new DialogSequence
                {
                    sequenceName = "Subsequent Meeting",
                    dialogType = DialogType.SubsequentMeeting,
                    dialogKeys = autoGenerateKeys ?
                        new string[] { $"{keyPrefix}{traitName.ToUpper()}_NORMAL1", $"{keyPrefix}{traitName.ToUpper()}_NORMAL2" } :
                        new string[] { "NORMAL_1", "NORMAL_2" }
                });
            }

            if (createTraitSpecificSequence)
            {
                sequences.Add(new DialogSequence
                {
                    sequenceName = $"{traitName} Specific",
                    dialogType = DialogType.TraitSpecific,
                    dialogKeys = autoGenerateKeys ?
                        new string[] { $"{keyPrefix}{traitName.ToUpper()}_SPECIAL1", $"{keyPrefix}{traitName.ToUpper()}_SPECIAL2" } :
                        new string[] { $"TRAIT_{traitName.ToUpper()}_1", $"TRAIT_{traitName.ToUpper()}_2" },
                    requiresSpecificTrait = true,
                    requiredTraitName = traitName
                });
            }

            if (createGenericSequence)
            {
                sequences.Add(new DialogSequence
                {
                    sequenceName = "Generic",
                    dialogType = DialogType.Generic,
                    dialogKeys = new string[] { "GENERIC_HELLO", "GENERIC_SMALL_TALK" }
                });
            }

            return sequences.ToArray();
        }
    }
}