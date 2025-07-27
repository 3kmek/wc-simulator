using UnityEngine;

namespace ScriptableObjects
{
    public enum TraitCategory
    {
        TokenRisk,
        GradeImpact,
        Economy,
        Curse,
        Maintenance
    }

    [CreateAssetMenu(fileName = "NewTrait", menuName = "ScriptableObjects/NPC Trait", order = 2)]
    public class NPCTrait : ScriptableObject
    {
        [Header("Temel Bilgi")]
        public string traitName;
        [TextArea] public string description;
        public TraitCategory category;

        [Header("Olasılık")]
        [Range(0f, 1f)] public float triggerChance = 1f; // Örn: 0.25 = %25 ihtimalle tetiklenir

        [Header("Etkiler")]
        public int tokenDelta;      // Jeton -2 gibi
        public float gradeMultiplier = 1f; // 0.8 → Grade çarpanı
        public int moneyDelta;      // Sahte para = -10 gibi
        public int curseDelta;      // CurseMeter etkisi
        public bool blocksPayment;  // Kaçan NPC gibi mi?
        public bool consumesExtraSupply;

        [Header("İpucu")]
        public GameObject visualHintPrefab; // Örn: burnunu tutan animasyon
        public AudioClip voiceHint;         // "Burası çok pis!" sesi
        
        [Header("Dialog Keyleri (Localization)")]
        public string[] dialogKeys;

        
        
        
    }
}