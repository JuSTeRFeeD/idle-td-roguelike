using System.Collections.Generic;
using Project.Runtime.Scriptable.Card.Perks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card
{
    [CreateAssetMenu(menuName = "Game/Cards/Card")]
    public class CardConfig : UniqueConfig
    {
        [Header("Card info")]
        [PreviewField]
        [SerializeField] private Sprite icon;
        [SerializeField] private string title;
        [SerializeField] private bool isBuildingOrSpell;

        [Header("Upgrade Params")]
        [SerializeField] private List<PerkConfig> perks = new();

        public Sprite Icon => icon;
        public string Title => title;
        public bool IsBuildingOrSpell => isBuildingOrSpell;
        public List<IPerk> Perks => perks.ConvertAll<IPerk>(perk => perk);
    }
}