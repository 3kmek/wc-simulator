using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
        [SerializeField] private NPCTraitManager traitManager; // Inspector'da assign edilecek
        [SerializeField] private int waitingForDice = 10;
        [SerializeField] [Range(1, 21)] private int minChance = 13;
        
        private int chanceToSpawn;

        private void Start()
        {
            if (traitManager == null)
            {
                Debug.LogError("NPCTraitManager is not assigned!");
                return;
            }

            StartCoroutine(StartSpawner());
        }

        private IEnumerator StartSpawner()
        {
            while (true)
            {
                chanceToSpawn = Random.Range(0, 21);

                if (chanceToSpawn > minChance - 1)
                {
                    SpawnNPC();
                }

                yield return new WaitForSeconds(waitingForDice);
            }
        }

        private void SpawnNPC()
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points available!");
                return;
            }

            // Trait kombinasyonunu oluştur
            NPCTraitCombination traitCombination = traitManager.CreateNPCTraitCombination();
            
            if (traitCombination.coreTrait == null || traitCombination.coreTrait.NPCPrefab == null)
            {
                Debug.LogWarning("No valid core trait or prefab found!");
                return;
            }

            // Spawn pozisyonunu seç
            int selectedSpawn = Random.Range(0, spawnPoints.Count);
            GameObject selectedSpawnPoint = spawnPoints[selectedSpawn];

            // NPC'yi spawn et
            GameObject newNPC = Instantiate(
                traitCombination.coreTrait.NPCPrefab, 
                selectedSpawnPoint.transform.position, 
                Quaternion.identity
            );

            // NPCController'a trait'leri ata
            NPCController npcController = newNPC.GetComponent<NPCController>();
            if (npcController != null)
            {
                npcController.activeTraits = traitCombination.GetAllTraits();
                
                Debug.Log($"Spawned NPC with Core: {traitCombination.coreTrait.traitName} " +
                         $"and {traitCombination.sideTraits.Count} side traits");
            }
            else
            {
                Debug.LogWarning("NPCController component not found on spawned NPC!");
            }
        }
    }
}