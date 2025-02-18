using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitCan : MonoBehaviour, IInteractable
{
    private PlayerInteraction _playerInteraction;
    [SerializeField] List<GameObject> _shitLayers = new List<GameObject>();
    [SerializeField] private int _showedLayer = 0;

    private void Start()
    {
        CalculateShitLayer();
        _playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
    }

    void CalculateShitLayer()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _shitLayers.Add(transform.GetChild(i).gameObject);
        }
    }

    public string GetInteractionText()
    {
        if (_playerInteraction.isHoldingSomething)
        {
            if(_playerInteraction._currentHeldObject.GetComponent<WC.Poop>() != null && _playerInteraction.IsInteractWhileHolding)
            {
                return "Insert the poop\nPress [E]";
            }
        }

        return "You should hold a poop to insert to can.";
    }

    public void Interact()
    {
        Debug.Log(_playerInteraction._currentHeldObject);
        if (_playerInteraction.isHoldingSomething)
        {
            Debug.Log(_playerInteraction._currentHeldObject.GetComponent<WC.Poop>());
            if (_playerInteraction._currentHeldObject.GetComponent<WC.Poop>() != null)
            {
                _shitLayers[_showedLayer].SetActive(true);
                _showedLayer++;
                Destroy(_playerInteraction._currentHeldObject);
                _playerInteraction.isHoldingSomething = false;
                _playerInteraction.IsInteractWhileHolding = false;
            }
        }
    }
}
