using UnityEngine;

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
        
        [Header("Dialog Keyleri (Localization)")]
        public string[] dialogKeys;

        [Header("Uyumlu Side Trait'ler (Sadece Core için)")]
        public NPCTrait[] compatibleSideTraits; // Bu core trait ile uyumlu yan trait'ler
    }
}