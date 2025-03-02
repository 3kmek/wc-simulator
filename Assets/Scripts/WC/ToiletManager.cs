using System;
using System.Collections.Generic;
using UnityEngine;
using ScriptibleObjects;
using WC;

public class ToiletManager : MonoBehaviour
{
    private static ToiletManager _instance;

    public static ToiletManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ToiletManager>();
                if (_instance == null)
                {
                    Debug.LogError("ToiletManager bulunamadı! Sahneye eklenmiş mi?");
                }
            }

            return _instance;
        }
    }

    public List<GameObject> currentToilets = new List<GameObject>();
    public List<GameObject> womenToilets = new List<GameObject>();
    public List<GameObject> menToilets = new List<GameObject>();
    public List<GameObject> busyToilets = new List<GameObject>();
    
    PlayerInteraction playerInteraction;

    private void Start()
    {
        RecalculateToilets();
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
    }

    private void Update()
    {
        RecalculateToilets();
        HandleNoAvaibleToiletText();
    }

    public void HandleNoAvaibleToiletText()
    {
        if (menToilets.Count == 0 && womenToilets.Count == 0)
        {
            playerInteraction.noToiletAvaibleText.enabled = true;
            playerInteraction.noToiletAvaibleText.text = "There is no toilet available.";
        }
        
        else if (menToilets.Count == 0)
        {
            playerInteraction.noToiletAvaibleText.enabled = true;
            playerInteraction.noToiletAvaibleText.text = "There is no men toilet available.";
        }
        else if (womenToilets.Count == 0)
        {
            playerInteraction.noToiletAvaibleText.enabled = true;
            playerInteraction.noToiletAvaibleText.text = "There is no women toilet available.";
        }
        else
        {
            playerInteraction.noToiletAvaibleText.enabled = false;
        }
    }


    public void RecalculateToilets()
    {
        currentToilets.Clear();
        womenToilets.Clear();
        menToilets.Clear();

        GameObject[] foundToilets = GameObject.FindGameObjectsWithTag("Toilet");
        foreach (GameObject toilet in foundToilets)
        {
            Toilet toiletSc = toilet.GetComponent<Toilet>();
            currentToilets.Add(toilet);

            if (!busyToilets.Contains(toilet))
            {
                busyToilets.Add(toilet);
            }

            if (!toiletSc.IsWCCompletelyFul())
            {
                busyToilets.Remove(toilet);
                switch (toiletSc.wcGender)
                {
                    case WCGender.Female:
                        womenToilets.Add(toilet);
                        break;
                    case WCGender.Male:
                        menToilets.Add(toilet);
                        break;
                    case WCGender.None:
                        currentToilets.Add(toilet);
                        break;
                }
            }
        }
    }

}