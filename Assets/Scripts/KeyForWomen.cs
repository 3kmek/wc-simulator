using NPC;
using UnityEngine;

public class KeyForWomen : MonoBehaviour, IInteractable
{
    QueueManager queueManager;
    [SerializeField]private GameObject kova;
    public void Interact()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.black);
        Debug.Log(gameObject.name + " ile etkileşime geçildi.");
        // Nesne ile ilgili etkileşim işlemleri
        queueManager.npcsInQueue[0].GetComponent<NPCController>().actionPosition = kova.transform;
        
        queueManager.DequeueFront();
        queueManager.npcsInQueue[0].GetComponent<NPCController>().ApproveAction();
    }

    public void Deneme()
    {
        
    }
}