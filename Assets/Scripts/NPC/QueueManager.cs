// QueueManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class QueueManager : MonoBehaviour
    {
        
        public List<NPCController> npcsInQueue = new List<NPCController>();
        [Tooltip("Queue’de kullanılacak tüm pozisyonlar. Index sırasına göre NPC'ler dizeceğiz.")]
        public List<Transform> queuePositions = new List<Transform>();

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                queuePositions.Add(transform.GetChild(i));
            }
        }
        
        /// <summary>
        /// Yeni bir NPC gelip sıraya girmek istediğinde bu fonksiyon çağrılacak.
        /// </summary>
        public void RegisterNPC(NPCController npc)
        {
            // Sırada boş bir yer var mı diye bak. (queuePositions boyutu kadar limit olsun)
            if (npcsInQueue.Count < queuePositions.Count)
            {
                npcsInQueue.Add(npc);
                npc.queueManager = this; // NPC, hangi QueueManager’a bağlı olduğunu bilsin
                RecalculateQueuePositions();
            }
            else
            {
                // Kuyruk doluysa ne yapmak istediğine sen karar ver
                Debug.LogWarning("Kuyruk dolu, NPC eklenemedi: " + npc.name);
                //StartCoroutine();
            }
        }
        
        
        /// <summary>
        /// Bir NPC kuyruktan ayrıldığında (örn. tuvalete girdiğinde) bu fonksiyonu çağır.
        /// </summary>
        public void UnregisterNPC(NPCController npc)
        {
            if (npcsInQueue.Contains(npc))
            {
                npcsInQueue.Remove(npc);
                // Herkes bir öne kayacak
                RecalculateQueuePositions();
            }
        }
        
        /// <summary>
        /// Sırayı yeniden hesapla. Yani index sırasına göre her NPC'ye
        /// ait queuePositions[i] konumuna gitmelerini söyle.
        /// </summary>
        private void RecalculateQueuePositions()
        {
            for (int i = 0; i < npcsInQueue.Count; i++)
            {
                NPCController npc = npcsInQueue[i];
                Transform targetPos = queuePositions[i];

                // NPC'ye "senin sıradaki hedefin bu" diye bildirelim.
                npc.SetQueueTarget(targetPos.position, i);
            }
        }
        
        /// <summary>
        /// En öndeki NPC (index 0) onay / işlem aldıysa, onu kuyruktan çıkar.
        /// </summary>
        public void DequeueFront()
        {
            if (npcsInQueue.Count > 0)
            {
                var frontNPC = npcsInQueue[0];
                UnregisterNPC(frontNPC);
            }
        }
        
        /// <summary>
        /// Mevcut sırayı NPC'lerin sayısına göre döndürür. Gerekirse UI vs. için kullanabilirsin.
        /// </summary>
        public int GetQueueCount()
        {
            return npcsInQueue.Count;
        }
        
        /// <summary>
        /// Örnek: Dışarıdan gelen bir NPC'ye "en yakın boş yer" bulma (opsiyonel).
        /// </summary>



        public Transform GetClosestEmptyPosition()
        {
            // Bu örnekte, sadece ilk boş slot'u döndürüyoruz.
            if (npcsInQueue.Count < queuePositions.Count)
            {
                return queuePositions[npcsInQueue.Count]; 
            }
            return null;
        }

        

        
    }
}