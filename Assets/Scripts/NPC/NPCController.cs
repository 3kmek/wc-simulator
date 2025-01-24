using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPC
{
    public enum NPCState
    {
        Neutral,
        Queuing,
        WaitingForApproval,
        MovingToAction,
        PerformingAction,
        PerformDone
    }

    public class NPCController : MonoBehaviour
    {
        public NPCState currentState;
        private NavMeshAgent agent;

        // Queue system
        [HideInInspector] public QueueManager queueManager;
        private Vector3 currentQueueTarget; // Sırada duracağı konum
        private int currentQueueIndex = -1; // Kendi sıralama index’im

        [SerializeField] public Transform actionPosition; // Örnek
        [SerializeField] private GameObject player;        // Örnek

        public string toiletType;

        // --- EKLENDİ: Belli aralıklarla tuvalet kontrolü yapmak için bir sayaç. ---
        [SerializeField]private float nextWCCheckTime = 0f;
       

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            
            // Burada NPC o an "Queuing" ile başlatılırsa (örneğin Inspector'dan), direkt kaydol
            if (queueManager != null && currentState == NPCState.Queuing)
            {
                queueManager.RegisterNPC(this);
            }

            // Eğer NPC, oyuna "Neutral" olarak başlıyorsa, ilk WC kontrol zamanını hesaplayalım.
            if (currentState == NPCState.Neutral)
            {
                // 5-10 saniye arasında rastgele bir süre sonra ilk kontrol
                nextWCCheckTime = Time.time + Random.Range(5f, 10f);
            }
        }

        void Update()
        {
            
            switch (currentState)
            {
                case NPCState.Neutral:
                    // Dışarda geziyor. Belli aralıklarla (5-10 sn) bir kere WC'ye gitmek isteyebilir.
                    // Her frame kontrol etmek yerine, Time.time ile vakti gelince 1 kez kontrol ediyoruz:
                    if (Time.time >= nextWCCheckTime)
                    {
                        IsNPCGoingToWC();
                        // Bir sonraki kontrolü tekrar 5-10 sn sonrasına planla.
                        nextWCCheckTime = Time.time + Random.Range(5f, 10f);
                    }
                    break;

                case NPCState.Queuing:
                    // NPC Kaydı -> Sırada durma / Sıradaki pozisyona gitme
                    if (!agent.pathPending && agent.remainingDistance > 0.5f)
                    {
                        agent.SetDestination(currentQueueTarget);
                    }

                    // Kuyruğun başı isek (index = 0)
                    if (queueManager.npcsInQueue.Count > 0 && queueManager.npcsInQueue[0] == this)
                    {
                        OnTurn();
                    }
                    break;

                case NPCState.WaitingForApproval:
                    // Oyuncudan onay bekleniyor
                    break;

                case NPCState.MovingToAction:
                    // Belirlenen noktaya hareket
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        currentState = NPCState.PerformingAction;
                        // PerformAction();
                    }
                    break;

                case NPCState.PerformingAction:
                    // Eylem tamamlanınca...
                    break;

                // PerformDone durumunu da istersen ekleyebilirsin; senaryona göre handling yapabilirsin
            }
        }

        /// <summary>
        /// NPC'nin WC'ye gitmeye karar verdiği metod.
        /// Burada random bir olasılık belirleyip, >15 ise kuyrukta sıraya giriyoruz.
        /// </summary>
        void IsNPCGoingToWC()
        {
            float chanceToQueue = Random.Range(0f, 20f);

            // Burada eğer NPC halihazırda "Neutral" durumdaysa, girsin. 
            // chanceToQueue > 15f ise -> "Queuing" yap ve Register et.
            if (chanceToQueue > 15f && currentState == NPCState.Neutral)
            {
                currentState = NPCState.Queuing;
                
                if (queueManager != null)
                {
                    queueManager.RegisterNPC(this);
                }
            }
            else
            {
                // Aksi hâlde yine Neutral kal.
                // (Zaten Neutral'da isek tekrar Neutral yapmamıza gerek yok.)
            }
        }

        /// <summary>
        /// QueueManager bizi hangi index’e konumlandırdıysa, oradaki pozisyona gideceğiz.
        /// </summary>
        public void SetQueueTarget(Vector3 targetPos, int index)
        {
            currentQueueTarget = targetPos;
            currentQueueIndex = index;

            if (currentState == NPCState.Queuing)
            {
                agent.SetDestination(targetPos);
            }
        }

        /// <summary>
        /// Sıranın başına gelince (index = 0) onay işlemlerini tetikleyebilirsin.
        /// Örn. OnTurn() metodu.
        /// </summary>
        public void OnTurn()
        {
            Debug.Log($"{gameObject.name} tuvalet tipini belirtiyor: {toiletType}");
            currentState = NPCState.WaitingForApproval;
        }

        /// <summary>
        /// Oyuncu onay verince action’a gideceğiz.
        /// </summary>
        public void ApproveAction()
        {
            currentState = NPCState.MovingToAction;
            agent.SetDestination(actionPosition.position);
        }

        /// <summary>
        /// Gerçek tuvalet kullanma animasyonu / bekleme vs.
        /// </summary>
        void PerformAction()
        {
            Debug.Log($"{gameObject.name} tuvalet kullanılıyor: {toiletType}");
            // Bir süre sonra biter
            Invoke(nameof(FinishAction), 5f);
        }

        void FinishAction()
        {
            Debug.Log($"{gameObject.name} tuvalet kullanımını tamamladı.");
            currentState = NPCState.PerformDone;

            // Kuyruktan çıkart. (Artık sıramızı kullandık, diğerleri öne geçsin.)
            if (queueManager != null)
            {
                queueManager.UnregisterNPC(this);
            }
        }
    }
}
