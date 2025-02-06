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

    private void Start()
    {
        RecalculateToilets();
    }
    
    private bool AllTheToiletsAreBusy()
    {
        if (womenToilets.Count == 0 || menToilets.Count == 0)
        {
            return true;
        }
        
        if (womenToilets.Count == 0 || menToilets.Count == 0)
        {
            return false;
        }

        return false;
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

            if (toiletSc.wcType.wcGender == "Female")
                womenToilets.Add(toilet);

            if (toiletSc.wcType.wcGender == "Male")
                menToilets.Add(toilet);
        }
    }

    public void RecalculateToilets(GameObject selectedToilet)
    {
        if (!busyToilets.Contains(selectedToilet) )
        {
            busyToilets.Add(selectedToilet);
        }
        
        else
        {
            if (busyToilets.Contains(selectedToilet))
            {
                busyToilets.Remove(selectedToilet);
            }
            else
            {
                Debug.LogError("HATA: " + selectedToilet.name + " busyToilets içinde bulunamadı!");
            }
        }

        currentToilets.Clear();
        womenToilets.Clear();
        menToilets.Clear();

        GameObject[] foundToilets = GameObject.FindGameObjectsWithTag("Toilet");
        foreach (GameObject toilet in foundToilets)
        {
            if (!busyToilets.Contains(toilet))
            {
                currentToilets.Add(toilet);
                Toilet toiletSc = toilet.GetComponent<Toilet>();

                if (toiletSc.wcType.wcGender == "Female")
                    womenToilets.Add(toilet);

                if (toiletSc.wcType.wcGender == "Male")
                    menToilets.Add(toilet);
            }
        }
    }

    public void MakeToiletBusy(GameObject selectedToilet)
    {

        if (selectedToilet.GetComponent<Toilet>().isToiletBusy == false)
        {
            if (!busyToilets.Contains(selectedToilet) )
            {
                busyToilets.Add(selectedToilet);
            }

            currentToilets.Clear();
            womenToilets.Clear();
            menToilets.Clear();

            GameObject[] foundToilets = GameObject.FindGameObjectsWithTag("Toilet");
            foreach (GameObject toilet in foundToilets)
            {
                if (!busyToilets.Contains(toilet))
                {
                    currentToilets.Add(toilet);
                    Toilet toiletSc = toilet.GetComponent<Toilet>();

                    if (toiletSc.wcType.wcGender == "Female")
                        womenToilets.Add(toilet);

                    if (toiletSc.wcType.wcGender == "Male")
                        menToilets.Add(toilet);
                }
            }
            selectedToilet.GetComponent<Toilet>().isToiletBusy = true;
        }
        
    }

    public void MakeToiletAvaible(GameObject selectedToilet)
    {
        if (selectedToilet.GetComponent<Toilet>().isToiletBusy == true)
        {
            
            if (busyToilets.Contains(selectedToilet) )
            {
                busyToilets.Remove(selectedToilet);
            }

            currentToilets.Clear();
            womenToilets.Clear();
            menToilets.Clear();

            GameObject[] foundToilets = GameObject.FindGameObjectsWithTag("Toilet");
            foreach (GameObject toilet in foundToilets)
            {
                if (!busyToilets.Contains(toilet))
                {
                    currentToilets.Add(toilet);
                    Toilet toiletSc = toilet.GetComponent<Toilet>();

                    if (toiletSc.wcType.wcGender == "Female")
                        womenToilets.Add(toilet);

                    if (toiletSc.wcType.wcGender == "Male")
                        menToilets.Add(toilet);
                }
            }

            selectedToilet.GetComponent<Toilet>().isToiletBusy = false;
        }
        
    }
}
