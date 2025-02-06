using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ScriptibleObjects;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using WC;
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
        PerformDone,
        KeyGiver,
        KeyThief,
        AllDone
    }

    
    public class NPCController : MonoBehaviour, IInteractable
    {
        [Header("NPC Type")]
        public NPCTypeScriptableObject npcType;
        
        [Header("NPC Specs")]
        public string Gender, npcName;
        public int CreepinessLevel, Weight, Chance;
        public int moneyToGive = 10;
        
        public NPCState currentState;
        public NavMeshAgent agent;

        // Queue system
        [HideInInspector] public QueueManager queueManager;
        private ToiletManager ToiletManager;
        private Vector3 currentQueueTarget; // Sırada duracağı konum
        private int currentQueueIndex = -1; // Kendi sıralama index’im

        [SerializeField] public Transform actionPosition; // Örnek
        [SerializeField] private GameObject player;        // Örnek

        public string toiletType;
        private bool isShitting = false;

        // --- EKLENDİ: Belli aralıklarla tuvalet kontrolü yapmak için bir sayaç. ---
        [SerializeField]private float nextWCCheckTime = 0f;

        
        
        public GameObject ToiletAssigned;

        public Key HavingKey;
        
        private NPCAnimatonController npcAnimatonController;

        public GameObject selectedToilet;
        
        public CurrencySystem currencySystem;
       
        void Start()
        {
            if (npcType != null)
            {
                // ScriptableObject'ten referans değerleri al ama her NPC için override et
                npcName = npcType.npcName;
                Gender = Random.Range(0, 2) == 0 ? "Male" : "Female";
                Weight = Random.Range(40, 151);
                CreepinessLevel = Random.Range(0, 101);
                Chance = Random.Range(0, 51);
                moneyToGive = Random.Range(10, 15);

                Debug.Log($"Spawned NPC: {npcName} | Gender: {Gender} | Weight: {Weight} | Creepiness: {CreepinessLevel} | Chance: {Chance}");
            }
            else
            {
                Debug.LogError("NPCTypeScriptableObject is NULL!");
            }
            
            agent = GetComponent<NavMeshAgent>();
            npcAnimatonController = GetComponent<NPCAnimatonController>();
            player = GameObject.FindGameObjectWithTag("Player");
            queueManager = GameObject.FindGameObjectWithTag("Queue Manager").GetComponent<QueueManager>();
            ToiletManager = ToiletManager.Instance;
            //ToiletManager = GameObject.FindGameObjectWithTag("Toilet Manager").GetComponent<ToiletManager>();
            
            if (queueManager != null && currentState == NPCState.Queuing)
            {
                queueManager.RegisterNPC(this);
            }
            if (currentState == NPCState.Neutral)
            {
                nextWCCheckTime = Time.time + Random.Range(5f, 10f);
            }
        }

        void Update()
        {
            
            switch (currentState)
            {
                case NPCState.Neutral:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    IsNPCGoingToWC();
                    //if (Time.time >= nextWCCheckTime)
                    //{
                   //     IsNPCGoingToWC();
                    //    nextWCCheckTime = Time.time + Random.Range(5f, 10f);
                    //}
                    break;

                case NPCState.Queuing:
                    // Walking animasyonuna geç
                    

                    if (!agent.pathPending && agent.remainingDistance > 0.5f)
                    {
                        agent.SetDestination(currentQueueTarget);
                    }
                    if(agent.remainingDistance > 0.5f) npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    if (agent.remainingDistance < 0.5f)
                    {
                        npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE); ///////////////////////
                    }

                    // Kuyruğun başı mı?
                    if (queueManager.npcsInQueue != null && queueManager.npcsInQueue.Count > 0 && queueManager.npcsInQueue[0] == this && agent.remainingDistance < 0.5f)
                    {
                        npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE);
                        OnTurn();
                    }
                    break;

                case NPCState.WaitingForApproval:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLERUSH);
                    // transform.DOLookAt(player.transform.rotation.eulerAngles + new Vector3(0, 180f, 0), 1f);
                    break;

                case NPCState.MovingToAction:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);

                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        currentState = NPCState.PerformingAction;
                    }
                    break;

                case NPCState.PerformingAction:
                    transform.position = ToiletAssigned.transform.position;
                    if (Gender == "Male") transform.DORotate(new Vector3(0, 90f, 0), 1f, RotateMode.FastBeyond360);
                    if (Gender == "Female") transform.DORotate(new Vector3(0, 270f, 0), 1f, RotateMode.FastBeyond360);
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.SHITTING);
                    
                    if (!isShitting) // Eğer zaten tuvaletini yapıyorsa tekrar başlatma
                    {
                        isShitting = true; // Tekrar çağrılmasını engelle

                        if (CreepinessLevel + Chance > 120)
                        {
                            StartCoroutine(DoSteelShit());
                        }
                        else if (Weight + Chance > 221)
                        {
                            StartCoroutine(DoBrokerShit());
                        }
                        else
                        {
                            StartCoroutine(DoStandartShit());
                        }
                    }
                    break;

                case NPCState.PerformDone:
                    isShitting = false;
                    currentState = NPCState.KeyGiver;
                    break;

                case NPCState.KeyGiver:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    agent.SetDestination(Gender == "Female" ? new Vector3(3, 0, 1) : new Vector3(-3, 0, 1));
                    if (agent.remainingDistance < 0.5f) npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE);
                    break;

                case NPCState.KeyThief:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.RUNNING);
                    break;

                case NPCState.AllDone:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);

                    agent.SetDestination(new Vector3(10, 0, 20));
                    break;
            }
        }

        /// <summary>
        /// NPC'nin WC'ye gitmeye karar verdiği metod.
        /// Burada random bir olasılık belirleyip, >15 ise kuyrukta sıraya giriyoruz.
        /// </summary>
        void IsNPCGoingToWC()
        {
            float chanceToQueue = Random.Range(15f, 20f);

            // Burada eğer NPC halihazırda "Neutral" durumdaysa, girsin. 
            // chanceToQueue > 15f ise -> "Queuing" yap ve Register et.
            if (chanceToQueue > 15f && currentState == NPCState.Neutral)
            {
                
                
                if (queueManager != null && queueManager.npcsInQueue.Count != 7)
                {
                    currentState = NPCState.Queuing;
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
            //Debug.Log($"{gameObject.name} tuvalet tipini belirtiyor: {toiletType}");
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

        public void AssignNPCToToilet(GameObject selectedToilet, Key key)
        {
            ToiletAssigned = selectedToilet;
            ToiletManager.Instance.MakeToiletBusy(selectedToilet);
            TakeKey(key);
        }
        
        public void GiveTheKeyToThePlayer()
        {
            
            HavingKey.gameObject.SetActive(true);
            currentState = NPCState.AllDone;
            transform.GetComponentInChildren<Key>().gameObject.GetComponent<MeshRenderer>().enabled = false;
            ToiletManager.MakeToiletAvaible(selectedToilet);
            currencySystem.AddMoney(moneyToGive);
        }

        public void TakeKey(Key key)
        {
            key.gameObject.SetActive(false);
            StartCoroutine(MoveKeyToNPC(key));
            //key.keyObject.DOMove(transform.position + new Vector3(0, 2f, 0), 1f);
            
        }

        private IEnumerator MoveKeyToNPC(Key key)
        {
            float duration = 1.0f;
            float elapsedTime = 0f;
            HavingKey = key;
            Vector3 startPos = key.keyObject.transform.position;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                key.keyObject.position = Vector3.Lerp(startPos, transform.position + new Vector3(0, 2f, 0), elapsedTime / duration);
                key.keyObject.DORotate(Random.rotation.eulerAngles, 1f, RotateMode.FastBeyond360);
                yield return null;
            }
            
            key.keyObject.gameObject.SetActive(false);
            transform.GetComponentInChildren<Key>().gameObject.GetComponent<MeshRenderer>().enabled = true;
            
        }

        private IEnumerator DoStandartShit()
        {
            yield return new WaitForSeconds(6f);
            Debug.Log("NPC Does standart shit");
            ToiletAssigned.GetComponent<Toilet>().DoneShit(ToiletAssigned);
            
            currentState = NPCState.PerformDone;
            yield return null;

        }
        
        private IEnumerator DoBrokerShit()
        {
            yield return new WaitForSeconds(7f);
            
            Debug.Log("NPC Broke the toilet");
            
            
            currentState = NPCState.PerformDone;
            
            
        }
        
        private IEnumerator DoSteelShit()
        {
            yield return new WaitForSeconds(7f);
            Debug.Log("NPC Steels the toilet");
            
            
            currentState = NPCState.PerformDone;
            
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

        public string GetInteractionText()
        {
            if (currentState == NPCState.KeyGiver)
            {
                return "Receive the key\nPress [E]";
            }

            return null;
        }

        public void Interact()
        {
            if (currentState == NPCState.KeyGiver)
            {
                GiveTheKeyToThePlayer();
            }
            
        }
    }
}
