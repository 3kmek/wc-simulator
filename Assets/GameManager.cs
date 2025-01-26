using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ToiletManager toiletManager;
    void Start()
    {
        toiletManager = ToiletManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
