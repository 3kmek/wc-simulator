using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GradeCalculator.cs – stub; gerçek formülü Roadmap adım 3’te dolduracağız
public class GradeCalculator : MonoBehaviour
{
    public static GradeCalculator Instance { get; private set; }
    void Awake(){ if (Instance!=null){Destroy(gameObject);return;} Instance=this; }

    public int CalculateCurrentGrade()
    {
        // TODO: Temizlik 40 % + Jeton/Olay 30 % + Gelir 20 % + Görev 10 %
        return 80;   // Placeholder
    }
}

