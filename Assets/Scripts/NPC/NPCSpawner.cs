using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
        [SerializeField] private GameObject NPC;
        [SerializeField] private int waitingForDice = 10;
        [SerializeField] [Range(1, 21)] int minChance = 13 ;
        [SerializeField] private List<NPCTypeScriptableObject> npcTypes;
        int chanceToSpawn;

        private void Start()
        {
            // Coroutine'i yalnızca bir kez başlatıyoruz
            StartCoroutine(StartSpawner());
        }

        private IEnumerator StartSpawner()
        {
            while (true) // Sonsuz döngü içinde coroutine tekrar eder
            {
                chanceToSpawn = Random.Range(0, 21);

                if (chanceToSpawn > minChance - 1)
                {
                    int selectedSpawn = Random.Range(0, spawnPoints.Count); // Dinamik olarak spawnPoints'in boyutuna göre seç
                    GameObject selectedSpawnPoint = spawnPoints[selectedSpawn];
                    
                    NPCTypeScriptableObject selectedNPCType = npcTypes[Random.Range(0, npcTypes.Count)];
                    
                    GameObject newNPC = Instantiate(selectedNPCType.npcPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                    NPCController npcController = newNPC.GetComponent<NPCController>();
                    
                    if (npcController != null) npcController.npcType = selectedNPCType;
                }

                // 100 saniye bekle ve tekrar çalıştır
                yield return new WaitForSeconds(waitingForDice);
            }
        }
    }


    
}

