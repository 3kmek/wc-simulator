using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WC;

public class ToiletGenderAssigner : MonoBehaviour
{
    public WC.WCGender GenderOfTheArea;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toilet"))
        {
            other.GetComponent<Toilet>().wcGender = GenderOfTheArea;
        }
    }
}
