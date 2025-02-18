using System.Collections.Generic;
using UnityEngine;


namespace ScriptibleObjects
{
    public enum GenderType { Male, Female, None }

    [CreateAssetMenu(fileName = "NPCType", menuName = "ScriptableObjects/NPCTypeScriptableObject", order = 1)]
    public class NPCTypeScriptableObject : ScriptableObject
    {
        [Header("Base NPC Attributes")]
        public string npcName = "Samet";
        public GenderType gender;
        public int Weight;
        public int CreepinessLevel;
        public int Chance;

        [Header("Visuals")]
        public GameObject npcPrefab; 

        [Header("Behavior")]
        public bool isAggressive;
        public float patienceLevel;
    }
}