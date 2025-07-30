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

        private const string EmptyText = "-blank\n-blank\n-blank\n-blank";

        private void Start()
        {
            queueManager = GameObject.FindGameObjectWithTag("Queue Manager")?.GetComponent<QueueManager>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            text.text = TryGetFrontNPC(out NPCController frontNpc) && frontNpc.agent.remainingDistance < 0.5f
                ? GetNPCInfo(frontNpc)
                : EmptyText;
        }

        private bool TryGetFrontNPC(out NPCController frontNpc)
        {
            frontNpc = null;
            return queueManager != null && queueManager.npcsInQueue.Count > 0 && (frontNpc = queueManager.npcsInQueue[0]) != null;
        }

        private string GetNPCInfo(NPCController npc)
        {
            return $"NPC: {npc.npcName}\nGender: {npc.Gender}\nCreepiness: {npc.CreepinessLevel}\nWeight: {npc.Weight}kg";
        }
    }
}