﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using ScriptableObjects;
using UnityEngine.Serialization;
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
        InFrontOfDoor,
        EnteringTheCubicle,
        PerformDone,
        KeyGiver,
        KeyThief,
        AllDone
    }
    
    public class NPCController : MonoBehaviour, IInteractable
    {
        [Header("NPC Specs")] 
        public GameObject hair;
        public string Gender, npcName;
        public int CreepinessLevel, Weight, Chance;
        public int moneyToGive = 10;
        
        public NPCState currentState;
        public NavMeshAgent agent;

        // WC Kuyruk Sistemi
        [HideInInspector] public QueueManager queueManager;
        private ToiletManager toiletManager;
        private Vector3 currentQueueTarget;
        private int currentQueueIndex = -1;

        [SerializeField] public Transform actionPosition;
        [SerializeField] private GameObject player;
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] public PlayerInteraction playerInteraction;

        public string toiletType;
        private bool isShitting = false;
        [SerializeField] private float nextWCCheckTime = 0f;

        public GameObject ToiletAssigned;
        public Key HavingKey;
        private NPCAnimatonController npcAnimatonController;
        public GameObject selectedToilet;
        public CurrencySystem currencySystem;

        // Table (Key Giver) Sistemi
        private Table table;
        private bool isTableRegistered = false;
        
        // CUBICLE VARIABLES
        public Cubicle Cubicle;
        
        [Header("NPC Trait")]
        public List<NPCTrait> activeTraits = new();
        
        // Dialog system variables
        private bool isDialogueActive = false;
        
        
        // Delegates
        public event Action<NPCTrait> OnDialogueEnter;

        public NPCTrait CoreTrait
        {
            get
            {
                foreach (NPCTrait coreAday in activeTraits)
                {
                    if (coreAday.category == TraitCategory.Core) // veya başka bir property kontrolü
                    {
                        return coreAday;
                    }
                }
                return null;
            }
        }
        
        void Start()
        {
            #region Install
            
            agent = GetComponent<NavMeshAgent>();
            npcAnimatonController = GetComponent<NPCAnimatonController>();
            player = GameObject.FindGameObjectWithTag("Player");
            firstPersonController = player.GetComponent<FirstPersonController>();
            queueManager = GameObject.FindGameObjectWithTag("Queue Manager").GetComponent<QueueManager>();
            toiletManager = ToiletManager.Instance;
            table = GameObject.FindGameObjectWithTag("Table").GetComponent<Table>();
            playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

                #endregion
           
            
            
            if (activeTraits != null)
            {
                npcName = "Samet";
                Gender = Random.Range(0, 2) == 0 ? "Male" : "Female";
                Weight = Random.Range(40, 151);
                CreepinessLevel = Random.Range(0, 101);
                Chance = Random.Range(0, 51);
                moneyToGive = Random.Range(10, 15);

                Debug.Log($"Spawned NPC: {npcName} | Gender: {Gender} | Weight: {Weight} | Creepiness: {CreepinessLevel} | Chance: {Chance}");
            }
            else
            {
                Debug.LogError("NPCTrait is NULL!");
            }
            
            
            
            // Eğer NPC kuyruğa eklenme durumundaysa
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
            // Input kontrollerini kaldırdık, sadece Interact() içinde olacak
            
            switch (currentState)
            {
                case NPCState.Neutral:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    IsNPCGoingToWC();
                    break;

                case NPCState.Queuing:
                    // Kuyrukta hedefe gitmek için
                    if (!agent.pathPending && agent.remainingDistance > 0.5f)
                        agent.SetDestination(currentQueueTarget);
                    
                    npcAnimatonController.ChangeAnimationState(agent.remainingDistance > 0.5f
                        ? NPCAnimationState.WALKING
                        : NPCAnimationState.IDLE);

                    // Kuyruğun başındaysa
                    if (queueManager.npcsInQueue != null &&
                        queueManager.npcsInQueue.Count > 0 &&
                        queueManager.npcsInQueue[0] == this &&
                        agent.remainingDistance < 0.5f)
                    {
                        npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE);
                        OnTurn();
                    }
                    break;

                case NPCState.WaitingForApproval:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLERUSH);
                    gameObject.layer = 6;
                    break;
                

                case NPCState.MovingToAction:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    gameObject.layer = 0;
                    if (!agent.pathPending && agent.remainingDistance < 0.01f)
                    {
                        currentState = NPCState.InFrontOfDoor;
                    }
                        
                    break;
                
                
                
                case NPCState.InFrontOfDoor:

                    if (Cubicle.cubicleDoor.isNPCLookingAndClose && !Cubicle.isCubicleBusy)
                    {
                        npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE);
                        StartCoroutine(NPCWaitsTheOpenCubicle());
                    }
                    break;
                
                
                
                case NPCState.EnteringTheCubicle:

                    if (!agent.pathPending && agent.remainingDistance < 0.01f)
                    {
                        currentState = NPCState.PerformingAction;
                        Cubicle.cubicleDoor.CloseDoor();
                        Cubicle.isCubicleBusy = true;
                    }
                    
                    break;
                

                case NPCState.PerformingAction:
                    transform.position = ToiletAssigned.transform.position;
                    if (Gender == "Male")
                        transform.DORotate(new Vector3(0, 90f, 0), 1f, RotateMode.FastBeyond360);
                    else if (Gender == "Female")
                        transform.DORotate(ToiletAssigned.transform.rotation.eulerAngles, 1f, RotateMode.FastBeyond360);
                    
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.SHITTING);
                    
                    if (!isShitting)
                    {
                        isShitting = true;
                        if (CreepinessLevel + Chance > 120)
                            StartCoroutine(DoSteelShit());
                        else if (Weight + Chance > 221)
                            StartCoroutine(DoBrokerShit());
                        else
                            StartCoroutine(DoStandartShit());
                    }
                    break;
                

                case NPCState.PerformDone:
                    Cubicle.isCubicleBusy = false;
                    
                    isShitting = false;
                    currentState = NPCState.KeyGiver;
                    break;
                

                case NPCState.KeyGiver:
                    gameObject.layer = 6;
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    // TableManager üzerinden bekleme pozisyonuna yönlendirme
                    if (table == null)
                        table = GameObject.FindGameObjectWithTag("Table").GetComponent<Table>();
                    
                    if (!isTableRegistered)
                    {
                        table.RegisterNPCtoTable(this);
                        isTableRegistered = true;
                    }

                    if (agent.remainingDistance < 0.5f)
                    {
                        npcAnimatonController.ChangeAnimationState(NPCAnimationState.IDLE);
                    }
                    
                    break;
                

                case NPCState.KeyThief:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.RUNNING);
                    break;
                

                case NPCState.AllDone:
                    npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
                    gameObject.layer = 0;
                    agent.SetDestination(new Vector3(9, 0, 62));
                    break;
            }
        }
        
        // Input kontrollerini ayrı bir metoda taşıdık
        private void HandleInputs()
        {
            // WaitingForApproval state'inde ve player oturuyorsa inputları kontrol et
            if (currentState == NPCState.WaitingForApproval && 
                firstPersonController.IsPlayerSitting && 
                !firstPersonController.isZoomed)
            {
                // E tuşu - Dialog başlat
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Interact();
                }
                
                // X tuşu - NPC'yi reddet (AllDone yap)
                if (Input.GetKeyDown(KeyCode.X))
                {
                    RejectNPC();
                }
                
                // F tuşu - Zoom
                if (Input.GetKeyDown(KeyCode.F))
                {
                    InspectNPC();
                }
            }
        }
        
        // Ayrı metodlar oluşturduk
        public void RejectNPC()
        {
            currentState = NPCState.AllDone;
            QueueManager.Instance.DequeueFront();
            Debug.Log("X pressed - NPC Rejected - State changed to AllDone");
        }
        
        public void InspectNPC()
        {
            
            if (!firstPersonController.isZoomed && !playerInteraction.InspectingMode)
            {
                firstPersonController.isZoomed = true;
                playerInteraction.InspectingMode = true;
            }
            else if (firstPersonController.isZoomed && playerInteraction.InspectingMode)
            {
                firstPersonController.isZoomed = false;
                playerInteraction.InspectingMode = false;
            }
            
            
            
        }

        /// <summary>
        /// Rastgele belirlenen olasılığa göre NPC WC kuyruğuna giriyor.
        /// </summary>
        void IsNPCGoingToWC()
        {
            float chanceToQueue = Random.Range(15f, 20f);
            if (chanceToQueue > 15f && currentState == NPCState.Neutral)
            {
                if (queueManager != null && queueManager.npcsInQueue.Count != 7)
                {
                    currentState = NPCState.Queuing;
                    queueManager.RegisterNPC(this);
                }
            }
        }

        /// <summary>
        /// Bekleme pozisyonunu günceller. Artık her durumda agent.SetDestination çağrılıyor.
        /// </summary>
        public void SetQueueTarget(Vector3 targetPos, int index)
        {
            currentQueueTarget = targetPos;
            currentQueueIndex = index;
            // Artık durum kontrolü olmadan hedef pozisyona yönlendiriyoruz.
            agent.SetDestination(targetPos);
        }

        public void OnTurn()
        {
            currentState = NPCState.WaitingForApproval;
        }

        public void ApproveAction()
        {
            currentState = NPCState.MovingToAction;
            agent.SetDestination(actionPosition.position);
        }

        public void AssignNPCToToilet(GameObject selectedToilet, Key key)
        {
            ToiletAssigned = selectedToilet;
            ToiletAssigned.GetComponent<Toilet>().Cubicle = Cubicle;
            ToiletAssigned.GetComponent<Toilet>().isNPCAssigned = true;
            TakeKey(key);
        }
        
        public void GiveTheKeyToThePlayer()
        {
            HavingKey.gameObject.SetActive(true);
            currentState = NPCState.AllDone;
            transform.GetComponentInChildren<Key>().gameObject.GetComponent<MeshRenderer>().enabled = false;
            ToiletAssigned.GetComponent<Toilet>().isNPCAssigned = false;
            currencySystem.AddMoney(moneyToGive);

            if (table == null)
                table = GameObject.FindGameObjectWithTag("Table").GetComponent<Table>();
            
            table.ReleaseNPC(this);
            isTableRegistered = false;
        }

        public void TakeKey(Key key)
        {
            key.gameObject.SetActive(false);
            StartCoroutine(MoveKeyToNPC(key));
        }

        IEnumerator NPCWaitsTheOpenCubicle()
        {
            yield return new WaitForSeconds(1f);
            currentState = NPCState.EnteringTheCubicle;
            Cubicle.cubicleDoor.OpenDoor();
            npcAnimatonController.ChangeAnimationState(NPCAnimationState.WALKING);
            agent.SetDestination(ToiletAssigned.transform.position);
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
            Debug.Log("NPC does standard shit");
            ToiletAssigned.GetComponent<Toilet>().DoneShit(ToiletAssigned);
            yield return new WaitForSeconds(0.5f);
            Cubicle.cubicleDoor.OpenDoor();
            
            currentState = NPCState.PerformDone;
        }
        
        private IEnumerator DoBrokerShit()
        {
            yield return new WaitForSeconds(7f);
            Debug.Log("NPC broke the toilet");
            
            yield return new WaitForSeconds(0.5f);
            Cubicle.cubicleDoor.OpenDoor();
            
            currentState = NPCState.PerformDone;
        }
        
        private IEnumerator DoSteelShit()
        {
            yield return new WaitForSeconds(7f);
            Debug.Log("NPC steels the toilet");
            
            yield return new WaitForSeconds(0.5f);
            Cubicle.cubicleDoor.OpenDoor();
            
            currentState = NPCState.PerformDone;
        }

        void PerformAction()
        {
            Debug.Log($"{gameObject.name} is using the toilet: {toiletType}");
            Invoke(nameof(FinishAction), 5f);
        }

        void FinishAction()
        {
            Debug.Log($"{gameObject.name} finished using the toilet.");
            currentState = NPCState.PerformDone;
            if (queueManager != null)
                queueManager.UnregisterNPC(this);
        }
        

        // IInteractable interface implementasyonu
        public string GetInteractionText()
        {
            if (currentState == NPCState.KeyGiver)
                return "Receive the key\nPress [E]";
            
            // WaitingForApproval durumunda etkileşim metinleri
            if (currentState == NPCState.WaitingForApproval && 
                firstPersonController.IsPlayerSitting && 
                !firstPersonController.isZoomed && 
                !isDialogueActive)
            {
                return "";
            }
            
            return null;
        }

        public void Interact()
        {
            // KeyGiver durumunda anahtarı ver
            if (currentState == NPCState.KeyGiver)
            {
                GiveTheKeyToThePlayer();
                return;
            }
            
            // WaitingForApproval durumunda dialog mode'a gir (sadece E tuşu için)
            if (currentState == NPCState.WaitingForApproval && 
                firstPersonController.IsPlayerSitting && 
                !firstPersonController.isZoomed && 
                !isDialogueActive)
            {
                // Dialog trigger'a dialog mode'a gir komutu gönder
                NPCDialogTrigger dialogTrigger = GetComponent<NPCDialogTrigger>();
                if (dialogTrigger != null)
                {
                    dialogTrigger.EnterDialogMode();
                    Debug.Log("E pressed - Entering dialog mode");
                }
                else
                {
                    // Fallback - eski sistem
                    if (activeTraits != null && activeTraits.Count > 0)
                    {
                        OnDialogueEnter?.Invoke(activeTraits[0]);
                        Debug.Log("E pressed - Starting dialogue (fallback)");
                    }
                }
            }
        }
        
        // Dialog sistemi için yardımcı metodlar
        public void SetDialogueActive(bool active)
        {
            isDialogueActive = active;
        }
        
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }
    }
}