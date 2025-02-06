using System;
using DG.Tweening;
using UnityEngine;

public class InteractableCenterGlow : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1);
    }

    private void Update()
    {
        if (player == null) return;

        // Oyuncuya doğru dönmesi için DOLookAt kullan
        transform.DOLookAt(player.position, 0.5f);
    }
}