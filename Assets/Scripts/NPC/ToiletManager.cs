using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletManager : MonoBehaviour
{
    private static ToiletManager _instance;
    public static ToiletManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("ToiletManagerGG");
                _instance = obj.AddComponent<ToiletManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    public List<GameObject> currentToilets = new List<GameObject>();
    public List<GameObject> womenToilets = new List<GameObject>();
    public List<GameObject> menToilets = new List<GameObject>();
    public List<GameObject> busyToilets = new List<GameObject>();
    private void Start()
    {
        RecalculateToilets();
        
    }

    public void RecalculateToilets()
    {
        currentToilets.Clear();
        womenToilets.Clear();
        menToilets.Clear();
        
        GameObject[] foundToilet = GameObject.FindGameObjectsWithTag("Toilet");
        foreach (GameObject toilet in foundToilet)
        {
            
                currentToilets.Add(toilet);
                if (toilet.name.StartsWith("W"))
                {
                    womenToilets.Add(toilet);
                }
                if (toilet.name.StartsWith("M"))
                {
                    menToilets.Add(toilet);
                }
            
            
        }
    }
    public void RecalculateToilets(GameObject selectedToilet)
    {
        busyToilets.Add(selectedToilet);
        currentToilets.Clear();
        womenToilets.Clear();
        menToilets.Clear();
        
        GameObject[] foundToilet = GameObject.FindGameObjectsWithTag("Toilet");
        foreach (GameObject toilet in foundToilet)
        {
            if (busyToilets.Count > 0 )
            {
                if (!busyToilets.Contains(toilet))
                {
                    currentToilets.Add(toilet);
                    if (toilet.name.StartsWith("W"))
                    {
                        womenToilets.Add(toilet);
                    }
                    if (toilet.name.StartsWith("M"))
                    {
                        menToilets.Add(toilet);
                    }
                }
            }
            else
            {
                if (toilet != selectedToilet)
                {
                    currentToilets.Add(toilet);
                    if (toilet.name.StartsWith("W"))
                    {
                        womenToilets.Add(toilet);
                    }
                    if (toilet.name.StartsWith("M"))
                    {
                        menToilets.Add(toilet);
                    }
                }
            }
            /*if (toilet != selectedToilet)
            {
                currentToilets.Add(toilet);
                if (toilet.name.StartsWith("W"))
                {
                    womenToilets.Add(toilet);
                }
                if (toilet.name.StartsWith("M"))
                {
                    menToilets.Add(toilet);
                }
                Debug.Log("saka mi amk");
            }
            if (toilet == selectedToilet && selectedToilet)
            {
                busyToilets.Add(selectedToilet);
                Debug.Log("nasilya");
            }*/
        }
    }
    
    
    
    
}
