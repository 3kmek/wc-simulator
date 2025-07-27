using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextManager : MonoBehaviour
{
    public static UITextManager Instance;

    private GameObject player;
    [SerializeField] public TextMeshProUGUI interactPrompt;
    [SerializeField] public TextMeshProUGUI warningPrompt;
    
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        Installation();
    }

    void Installation()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
