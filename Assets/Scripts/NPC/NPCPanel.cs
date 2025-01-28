using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NPC
{
    public class NPCPanel : MonoBehaviour
    {
        [SerializeField] private QueueManager queueManager;
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            queueManager = GameObject.FindGameObjectWithTag("Queue Manager").GetComponent<QueueManager>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (queueManager != null && queueManager.npcsInQueue != null && queueManager.npcsInQueue.Count > 0 && queueManager.npcsInQueue[0] != null)
            {
                NPCController frontNpc = queueManager.npcsInQueue[0];
                string gender = frontNpc.Gender;
                int npcCreepinessLevel = frontNpc.CreepinessLevel;
                int npcWeight = frontNpc.Weight;
                text.text = "Gender - " + gender + "\nCreepiness Level - " + npcCreepinessLevel + "\nWeight - " + npcWeight + " (kg)" + "\n-blank";
            }
            else
            {
                text.text = "-blank\n-blank\n-blank\n-blank";
            }
            
            
        }
    }
}

