using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
        [SerializeField] private GameObject NPC;
        [SerializeField] private int waitingForDice = 10;
        [SerializeField] [Range(1, 21)] int minChance = 13 ;
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
                    Instantiate(NPC, selectedSpawnPoint.transform.position, Quaternion.identity);
                }

                // 100 saniye bekle ve tekrar çalıştır
                yield return new WaitForSeconds(waitingForDice);
            }
        }
    }


    
}

