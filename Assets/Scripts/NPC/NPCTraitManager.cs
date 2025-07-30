using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NPCTraitManager", menuName = "ScriptableObjects/NPC Trait Manager", order = 1)]
    public class NPCTraitManager : ScriptableObject
    {
        [Header("Core Traits")]
        public List<NPCTrait> coreTraits = new List<NPCTrait>();
        
        [Header("Side Traits")]
        public List<NPCTrait> sideTraits = new List<NPCTrait>();

        [Header("Spawn Settings")]
        [Range(0, 5)] public int maxSideTraitsPerNPC = 2;

        /// <summary>
        /// Rastgele bir core trait seçer (spawn olasılıklarına göre)
        /// </summary>
        public NPCTrait SelectRandomCoreTrait()
        {
            if (coreTraits == null || coreTraits.Count == 0) return null;

            // Ağırlıklı rastgele seçim
            float totalWeight = coreTraits.Sum(t => t.spawnChance);
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var trait in coreTraits)
            {
                currentWeight += trait.spawnChance;
                if (randomValue <= currentWeight)
                    return trait;
            }

            return coreTraits[0]; // Fallback
        }

        /// <summary>
        /// Verilen core trait ile uyumlu side trait'leri seçer
        /// </summary>
        public List<NPCTrait> SelectSideTraits(NPCTrait coreTrait)
        {
            List<NPCTrait> selectedSideTraits = new List<NPCTrait>();
            
            if (coreTrait == null || maxSideTraitsPerNPC == 0) return selectedSideTraits;

            // Core trait'in uyumlu side trait'lerini al
            List<NPCTrait> availableTraits = new List<NPCTrait>();
            
            if (coreTrait.compatibleSideTraits != null && coreTrait.compatibleSideTraits.Length > 0)
            {
                availableTraits.AddRange(coreTrait.compatibleSideTraits);
            }
            else
            {
                // Eğer özel uyumluluk yoksa, tüm side trait'leri kullan
                availableTraits.AddRange(sideTraits);
            }

            // Rastgele side trait'leri seç
            int traitsToSelect = Random.Range(0, Mathf.Min(maxSideTraitsPerNPC + 1, availableTraits.Count + 1));
            
            for (int i = 0; i < traitsToSelect; i++)
            {
                if (availableTraits.Count == 0) break;

                NPCTrait randomTrait = availableTraits[Random.Range(0, availableTraits.Count)];
                
                // Trigger chance kontrolü
                if (Random.Range(0f, 1f) <= randomTrait.triggerChance)
                {
                    selectedSideTraits.Add(randomTrait);
                }
                
                availableTraits.Remove(randomTrait); // Aynı trait'i tekrar seçmeyi engelle
            }

            return selectedSideTraits;
        }

        /// <summary>
        /// Tam bir NPC trait kombinasyonu oluşturur
        /// </summary>
        public NPCTraitCombination CreateNPCTraitCombination()
        {
            NPCTrait coreTrait = SelectRandomCoreTrait();
            List<NPCTrait> sideTraits = SelectSideTraits(coreTrait);

            return new NPCTraitCombination
            {
                coreTrait = coreTrait,
                sideTraits = sideTraits
            };
        }
    }

    [System.Serializable]
    public class NPCTraitCombination
    {
        public NPCTrait coreTrait;
        public List<NPCTrait> sideTraits = new List<NPCTrait>();

        public List<NPCTrait> GetAllTraits()
        {
            List<NPCTrait> allTraits = new List<NPCTrait>();
            if (coreTrait != null) allTraits.Add(coreTrait);
            allTraits.AddRange(sideTraits);
            return allTraits;
        }
    }
}
