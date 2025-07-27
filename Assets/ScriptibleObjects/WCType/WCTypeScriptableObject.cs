using System.Collections.Generic;
using UnityEngine;
using WC;

namespace ScriptableObjects
{
    

    [CreateAssetMenu(fileName = "WCType", menuName = "ScriptableObjects/WCTypeScriptableObject", order = 2)]
    public class WCTypeScriptableObject : ScriptableObject
    {
        [Header("WC Özellikleri")]
        public string wcName;
        public int Level;
        public int Durability;
        public int ShitLimit;
    
        [Header("Özel Efektler")]
        public bool hasAutoFlush;
        public bool isLuxury;
        public AudioClip flushSound;

        [Header("Poop Types")] 
        public List<Poop> PoopTypes = new List<Poop>();
    }

}
