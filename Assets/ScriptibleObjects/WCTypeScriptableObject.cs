using System.Collections.Generic;
using UnityEngine;
using WC;

namespace ScriptibleObjects
{
    [CreateAssetMenu(fileName = "WCType", menuName = "ScriptableObjects/WCTypeScriptableObject", order = 2)]
    public class WCTypeScriptableObject : ScriptableObject
    {
        [Header("WC Özellikleri")]
        public string wcName;       // Tuvaletin adı (Luxury, Public, Porta Potty)
        public int Level;           // Tuvaletin seviyesi (1-5 gibi)
        public int Durability;      // Dayanıklılık (kaç kere kullanılabilir)
        public int ShitLimit;       // Aynı anda kaç kişi kullanabilir
        public string wcGender;
        
        
        [Header("Özel Efektler")]
        public bool hasAutoFlush;    // Otomatik sifon var mı?
        public bool isLuxury;        // Lüks bir tuvalet mi?
        public AudioClip flushSound; // Sifon sesi

        [Header("Poop Types")] 
        public List<Poop> PoopTypes = new List<Poop>();
        
    }
}
