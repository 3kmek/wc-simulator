using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public CurrencySystem currencySystem;
    
    private void Update()
    {
        moneyText.text = "MONEY: " + currencySystem.currentMoney.ToString();
    }
}
