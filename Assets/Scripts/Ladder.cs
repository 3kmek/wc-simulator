using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject player, playerOtherPart;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.7f, 0);

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerOtherPart = player.transform.GetChild(0).gameObject;
    }

    public string GetInteractionText()
    {
        throw new NotImplementedException();
    }

    public void Interact()
    {
        //player.transform.position = spawnPoint.position + new Vector3(0, 1f, 0);
        playerOtherPart.transform.position = spawnPoint.position + offset;
    }
}
