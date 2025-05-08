using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using ScriptibleObjects;
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

        // WC Kuyruk Sistemi
        [HideInInspector] public QueueManager queueManager;
        private ToiletManager toiletManager;
        private Vector3 currentQueueTarget;
        private int currentQueueIndex = -1;

        [SerializeField] public Transform actionPosition;
        [SerializeField] private GameObject player;

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
        
        void Start()
        {
            if (npcType != null)
            {
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
            toiletManager = ToiletManager.Instance;
            table = GameObject.FindGameObjectWithTag("Table").GetComponent<Table>();
            
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
                    if (!agent.pathPending && agent.remainingDistance < 2f)
                        currentState = NPCState.PerformingAction;
                    break;

                case NPCState.PerformingAction:
                    transform.position = ToiletAssigned.transform.position;
                    if (Gender == "Male")
                        transform.DORotate(new Vector3(0, 90f, 0), 1f, RotateMode.FastBeyond360);
                    else if (Gender == "Female")
                        transform.DORotate(new Vector3(0, 270f, 0), 1f, RotateMode.FastBeyond360);
                    
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
            currentState = NPCState.PerformDone;
        }
        
        private IEnumerator DoBrokerShit()
        {
            yield return new WaitForSeconds(7f);
            Debug.Log("NPC broke the toilet");
            currentState = NPCState.PerformDone;
        }
        
        private IEnumerator DoSteelShit()
        {
            yield return new WaitForSeconds(7f);
            Debug.Log("NPC steels the toilet");
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

        public string GetInteractionText()
        {
            if (currentState == NPCState.KeyGiver)
                return "Receive the key\nPress [E]";
            return null;
        }

        public void Interact()
        {
            if (currentState == NPCState.KeyGiver)
                GiveTheKeyToThePlayer();
        }
    }
}
