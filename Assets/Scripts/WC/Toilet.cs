using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ScriptibleObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WC
{
    public class Toilet : MonoBehaviour
    {
        public WCTypeScriptableObject wcType;

        public List<Poop> PoopTypes = new List<Poop>();

        [SerializeField]private int triggeredPoop = 0;
        
        [SerializeField] public bool isToiletBusy = false;

        private void Start()
        {
            PoopTypes = wcType.PoopTypes;
        }

        void Update()
        {
            if (IsWCCapacityFull())
            {
                ToiletManager.Instance.MakeToiletBusy(gameObject);
            }
            else
            {
                ToiletManager.Instance.MakeToiletAvaible(gameObject);
            }
        }

        bool IsWCCapacityFull()
        {
            if (triggeredPoop >= wcType.ShitLimit || isToiletBusy)
            {
                return true;
            }

            if (triggeredPoop < wcType.ShitLimit && !isToiletBusy)
            {
                return false;
            }


            return false;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ( other.GetComponent<Poop>())
            {
                triggeredPoop += 1;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Poop>())
            {
                triggeredPoop -= 1;
            }
        }

        public void DoneShit(GameObject ToiletAssigned)
        {
           
           int selectedPoopInt = Random.Range(0, PoopTypes.Count);
           GameObject selectedPoop = PoopTypes[selectedPoopInt].gameObject;
           if (selectedPoop.name == "Poop2")
           {
               selectedPoop.transform.localScale = new Vector3(Random.Range(0.3f, 0.6f), Random.Range(0.3f, 0.6f), Random.Range(0.3f, 0.6f));
               selectedPoop.transform.DORotate(new Vector3(287.875244f,9.6459322f,300.278778f), 1f, RotateMode.FastBeyond360);
           }
           Vector3 spawnLocation = ToiletAssigned.transform.position + new Vector3(0f, 1.3f, 0f);
           GameObject poop = Instantiate(selectedPoop, spawnLocation, Quaternion.identity);


        }
        
        
        
    }
}
