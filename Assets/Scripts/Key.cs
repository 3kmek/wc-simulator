using System;
using NPC;
using UnityEngine;
using Random = UnityEngine.Random;


public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] QueueManager queueManager;
    
    [SerializeField]private GameObject kova;
    public NPCController frontNpc;

    public Transform keyObject;

    public string genderOfKey;

    private void Start()
    {
        keyObject = transform.parent.GetChild(1);
    }

    public void Interact()
    {
        queueManager = GameObject.FindGameObjectWithTag("Queue Manager").GetComponent<QueueManager>();
        
        Debug.Log(gameObject.name + " ile etkileşime geçildi.");
        
        // Nesne ile ilgili etkileşim işlemleri
        if (queueManager != null && queueManager.npcsInQueue.Count > 0)
        {
            frontNpc = queueManager.npcsInQueue[0];

            if (frontNpc.Gender == "Female" && genderOfKey == "Female")
            {
                queueManager.DequeueFront();

                if (ToiletManager.Instance.womenToilets.Count > 0)
                {
                    GameObject selectedToilet = ToiletManager.Instance.womenToilets[Random.Range(0, ToiletManager.Instance.womenToilets.Count)];
                    frontNpc.actionPosition = selectedToilet.transform;
                    frontNpc.ApproveAction();
                    frontNpc.AssignNPCToToilet(selectedToilet, this);
                }
            }

            if (frontNpc.Gender == "Male" && genderOfKey == "Male")
            {
                queueManager.DequeueFront();
                
                if (ToiletManager.Instance.menToilets.Count > 0)
                {
                    GameObject selectedToilet = ToiletManager.Instance.menToilets[Random.Range(0, ToiletManager.Instance.menToilets.Count)];
                    frontNpc.AssignNPCToToilet(selectedToilet, this);
                    frontNpc.actionPosition = selectedToilet.transform;
                    frontNpc.ApproveAction();
                    
                }
            }

            if (frontNpc.Gender == "Uni")
            {
                queueManager.DequeueFront();
                
                if (ToiletManager.Instance.currentToilets.Count > 0)
                {
                    GameObject selectedToilet = ToiletManager.Instance.currentToilets[Random.Range(0, ToiletManager.Instance.currentToilets.Count)];
                    frontNpc.actionPosition = selectedToilet.transform;
                    frontNpc.ApproveAction();
                    frontNpc.AssignNPCToToilet(selectedToilet, this);
                }
            }
        }
    }

    public void Deneme()
    {
        
    }
}