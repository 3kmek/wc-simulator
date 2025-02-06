using System;
using NPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private GameObject kova;
    public NPCController frontNpc;
    public Transform keyObject;
    public string genderOfKey;

    private void Start()
    {
        keyObject = transform.parent.GetChild(1);
    }

    public string GetInteractionText()
    {
        return "Give the key\n" + genderOfKey + "\nPress [E]";
    }

    public void Interact()
    {
        queueManager = GameObject.FindGameObjectWithTag("Queue Manager").GetComponent<QueueManager>();

        Debug.Log($"{gameObject.name} ile etkileşime geçildi.");

        // Kuyrukta NPC var mı ve ön sıradaki NPC hazır mı?
        if (!TryGetFrontNPC(out frontNpc)) return;
    
        // NPC'nin cinsiyeti uygun mu?
        if (IsValidNPC(frontNpc))
        {
            
            AssignToToilet(frontNpc);
        }
    }

    

    private bool TryGetFrontNPC(out NPCController npc)
    {
        npc = null;
        if (queueManager == null || queueManager.npcsInQueue.Count == 0) return false;
    
        npc = queueManager.npcsInQueue[0];
        return npc.agent.remainingDistance < 0.5f;
    }

    private bool IsValidNPC(NPCController npc) =>
        (npc.Gender == "Female" && genderOfKey == "Female") ||
        (npc.Gender == "Male" && genderOfKey == "Male") ||
        (npc.Gender == "Uni");


    private void AssignToToilet(NPCController npc)
    {
        // Doğru listeyi seç
        var toiletList = npc.Gender switch
        {
            "Female" => ToiletManager.Instance.womenToilets,
            "Male" => ToiletManager.Instance.menToilets,
            _ => ToiletManager.Instance.currentToilets
        };
        
        

        // Eğer uygun bir tuvalet yoksa işlem yapma
        if (toiletList.Count == 0) return;
        queueManager.DequeueFront();

        // Random bir tuvalet seç ve NPC’ye ata
        GameObject selectedToilet = toiletList[Random.Range(0, toiletList.Count)];
        npc.selectedToilet = selectedToilet;
        npc.actionPosition = selectedToilet.transform;
        npc.ApproveAction();
        npc.AssignNPCToToilet(selectedToilet, this);

        Debug.Log($"{npc.Gender} NPC, {selectedToilet.name} tuvaletine atandı.");
    }
}
