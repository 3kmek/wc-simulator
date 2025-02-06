using UnityEngine;

[System.Serializable]
public class BuildablePair
{
    public GameObject realPrefab;   // Yerleştirileceği zaman kullanılan gerçek obje
    public GameObject ghostPrefab;  // Ekranda gezerken kullanılan hayalet obje
}