using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencySystem", menuName = "ScriptableObjects/CurrencySystem", order = 3)]
public class CurrencySystem : ScriptableObject
{
    [Header("Player Money")]
    public int currentMoney;
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }
    
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log("Money Spent! Remaining: " + currentMoney);
            return true;
        }
        Debug.LogWarning("Not enough money!");
        return false;
    }
    
    
    
    
    
}
