using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class StatsWidget : MonoBehaviour
    {
        [Serializable]
        public struct Stat
        {
            public Image bg;
            public TextMeshProUGUI valueText;
        }

        [SerializeField] private List<Stat> stats = new();

        public void SetStats(List<string> statsText)
        {
            int i;
            for (i = 0; i < statsText.Count; i++)
            {
                var stat = stats[i];
                stat.valueText.enabled = stat.bg.enabled = true; 
                stats[i].valueText.SetText(statsText[i]);
            }

            for (; i < stats.Count; i++)
            {
                var stat = stats[i];
                stat.valueText.enabled = stat.bg.enabled = false;
            }
        }
    }
}