using System;
using System.Collections.Generic;
using UnityEngine;
using NPC; // NPCController’ın bulunduğu namespace
using DG.Tweening;

public class Table : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 4f;
    [SerializeField] Vector3 scaleAmount = Vector3.one;
    [Header("Erkek Bekleme Pozisyonları (Indeks: 0, 1, 2)")]
    [SerializeField] private Transform[] maleWaitingPositions;

    [Header("Kadın Bekleme Pozisyonları (Indeks: 0, 1, 2)")]
    [SerializeField] private Transform[] femaleWaitingPositions;

    // Masada bekleyen NPC’leri sıralı tutan listeler
    private List<NPCController> maleQueue = new List<NPCController>();
    private List<NPCController> femaleQueue = new List<NPCController>();
    
    
    PlayerInteraction playerInteraction;
    FirstPersonController firstPersonController;

    private void Start()
    {
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
        firstPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        HandleTableVisibility();
    }

    void HandleTableVisibility()
    {
        if (playerInteraction.InspectingMode)
        {
            transform.DOScale(scaleAmount, firstPersonController.zoomStepTime * scaleSpeed * Time.deltaTime);
        }
        else if (!playerInteraction.InspectingMode)
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), firstPersonController.zoomStepTime*scaleSpeed* Time.deltaTime);
        }
    }

    /// <summary>
    /// NPC’nin masada beklemesi için sıraya eklenmesi.
    /// </summary>
    /// <param name="npc">Beklemeye başlayacak NPC</param>
    public void RegisterNPCtoTable(NPCController npc)
    {
        if (npc == null) return;

        if (npc.Gender == "Male")
        {
            if (!maleQueue.Contains(npc))
            {
                if (maleQueue.Count < maleWaitingPositions.Length)
                {
                    maleQueue.Add(npc);
                    UpdateQueuePositions("Male");
                }
                else
                {
                    Debug.LogWarning("Erkek masasında boş bekleme alanı yok!");
                }
            }
        }
        else // Kadın
        {
            if (!femaleQueue.Contains(npc))
            {
                if (femaleQueue.Count < femaleWaitingPositions.Length)
                {
                    femaleQueue.Add(npc);
                    UpdateQueuePositions("Female");
                }
                else
                {
                    Debug.LogWarning("Kadın masasında boş bekleme alanı yok!");
                }
            }
        }
    }

    /// <summary>
    /// NPC, oyuncuyla etkileşime girdikten sonra sıradan çıkarılır.
    /// Kalan NPC’lerin bekleme pozisyonları yeniden güncellenir.
    /// </summary>
    /// <param name="npc">Sıradan çıkarılacak NPC</param>
    public void ReleaseNPC(NPCController npc)
    {
        if (npc == null) return;

        if (npc.Gender == "Male")
        {
            if (maleQueue.Remove(npc))
            {
                UpdateQueuePositions("Male");
            }
        }
        else
        {
            if (femaleQueue.Remove(npc))
            {
                UpdateQueuePositions("Female");
            }
        }
    }

    /// <summary>
    /// Belirtilen cinsiyet için kuyruğun güncel durumuna göre NPC’lerin bekleme pozisyonlarını atar.
    /// Örneğin; listede ilk sıradakine index 0, ikinciye index 1, vs.
    /// </summary>
    /// <param name="gender">"Male" veya "Female"</param>
    private void UpdateQueuePositions(string gender)
    {
        if (gender == "Male")
        {
            for (int i = 0; i < maleQueue.Count; i++)
            {
                // Kuyruğun i. elemanına, maleWaitingPositions dizisindeki i indeksli pozisyon atanır.
                Vector3 targetPos = maleWaitingPositions[i].position;
                maleQueue[i].SetQueueTarget(targetPos, i);
            }
        }
        else // Female
        {
            for (int i = 0; i < femaleQueue.Count; i++)
            {
                Vector3 targetPos = femaleWaitingPositions[i].position;
                femaleQueue[i].SetQueueTarget(targetPos, i);
            }
        }
    }
}
