// NPCDisplay.cs

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NPC
{
    public class NPCDisplay : MonoBehaviour
    {
        public TextMeshProUGUI npcText;

        public void UpdateDisplay(NPCController npc)
        {
            if (npcText != null)
            {
                npcText.text = $"{npc.gameObject.name} tuvalet ihtiyacı: {npc.toiletType}";
            }
        }

        public void ClearDisplay()
        {
            if (npcText != null)
            {
                npcText.text = "";
            }
        }
    }
}