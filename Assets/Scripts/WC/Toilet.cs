using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ScriptibleObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace WC
{
    public enum WCGender
    {
        Male,
        Female,
        None
    }

    public class Toilet : MonoBehaviour
    {
        public Cubicle Cubicle;
        
        public WCTypeScriptableObject wcType;
        
        public WCGender wcGender;

        public List<Poop> PoopTypes = new List<Poop>();
        
        [SerializeField] private int _poopAmount;
        [SerializeField] public bool isToiletFullOfShit = false;
        [SerializeField] public bool isNPCAssigned = false;

        [SerializeField] private Collider[] _colliders;
        
        [SerializeField] Vector3 colliderBoxSize = new Vector3(2, 2, 2);

        [SerializeField] public GameObject poopHolder;
        
        
        
        
        
        
        private void Start()
        {
            PoopTypes = wcType.PoopTypes;
            
        }

        void Update()
        {
            CalculatePoopAround();
            AssignGenderBasedOnLocation();
        }

        public bool IsWCCompletelyFul()
        {
            if (isToiletFullOfShit || isNPCAssigned)
            {
                return true;
            }

            if (_poopAmount < wcType.ShitLimit && !isToiletFullOfShit)
            {
                
                return false;
            }


            return false;
        }

        void CalculatePoopAround()
        {
            _colliders = Physics.OverlapBox(transform.position + new Vector3(0, 0.5f, 0), colliderBoxSize * 0.5f, Quaternion.identity);
            _poopAmount = 0;
            if (_colliders != null)
            {
                for (int i = 0; i < _colliders.Length; i++)
                {
                    if (_colliders[i].GetComponent<Poop>())
                    {
                        _poopAmount++;
                    }
                }

                if (_poopAmount < wcType.ShitLimit)
                {
                    isToiletFullOfShit = false;
                }

                else
                {
                    isToiletFullOfShit = true;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), colliderBoxSize);
        }


        public void DoneShit(GameObject ToiletAssigned)
        { 
            StartCoroutine("SpawnPoop");
           int selectedPoopInt = Random.Range(0, PoopTypes.Count);
           GameObject selectedPoop = PoopTypes[selectedPoopInt].gameObject;
           if (selectedPoop.name == "Poop2")
           {
               selectedPoop.transform.localScale = new Vector3(Random.Range(0.3f, 0.6f), Random.Range(0.3f, 0.6f), Random.Range(0.3f, 0.6f));
               
               selectedPoop.transform.DORotate(new Vector3(287.875244f,9.6459322f,300.278778f), 1f, RotateMode.FastBeyond360);
           }
           
           if(ToiletAssigned.GetComponent<Toilet>().wcType.Level == 1 )
           {
               Vector3 spawnLocation = ToiletAssigned.GetComponent<Toilet>().poopHolder.transform.position;  
               GameObject poop = Instantiate(selectedPoop, spawnLocation, Quaternion.identity);
               poop.transform.DOMove(spawnLocation, .5f);
           };
           
           if(ToiletAssigned.GetComponent<Toilet>().wcType.Level == 2 )
           {
               Vector3 spawnLocation = ToiletAssigned.GetComponent<Toilet>().poopHolder.transform.position;  
               GameObject poop = Instantiate(selectedPoop, spawnLocation, Quaternion.identity);
               poop.transform.DOMove(spawnLocation, .5f);
           };
           
           
           
        }

        IEnumerable SpawnPoop()
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        private void AssignGenderBasedOnLocation()
        {
            // Oyunda cinsiyet bölgelerini belirten "MaleSection" ve "FemaleSection" isimli boş nesneler (GameObject) olduğunu varsayalım.
            GameObject maleSection = GameObject.FindGameObjectWithTag("MaleSection");
            GameObject femaleSection = GameObject.FindGameObjectWithTag("FemaleSection");

            float distanceToMale = Vector3.Distance(transform.position, maleSection.transform.position);
            float distanceToFemale = Vector3.Distance(transform.position, femaleSection.transform.position);

            // Hangi bölgeye daha yakınsa ona atanır
            if (distanceToMale < distanceToFemale)
            {
                wcGender = WCGender.Male;
            }
            else
            {
                wcGender = WCGender.Female;
            }

            // Eğer her iki bölgeye de uzaksa, Uni olarak atanır.
            //if (distanceToMale > 10f && distanceToFemale > 10f)
            //{
            //    wcGender = WCGender.None;
            //}

            ToiletManager.Instance.RecalculateToilets();
        }
        
    }
}
