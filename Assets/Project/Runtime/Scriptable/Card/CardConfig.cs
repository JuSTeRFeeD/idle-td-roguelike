using System.Collections.Generic;
using Project.Runtime.Scriptable.Card.Perks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card
{
    [CreateAssetMenu(menuName = "Game/Cards/Card")]
    public class CardConfig : UniqueConfig
    {
        [Title("Card info")]
        [PreviewField]
        [SerializeField] private Sprite icon;
        [SerializeField] private string title;
        [SerializeField] private bool isBuildingOrSpell;
        [Space]
        [Tooltip("How much cards of this type can be dropped during game phase")]
        [SerializeField] private int maxPerGame = 3;

        [Title("Upgrade Params")]
        [SerializeField] private List<PerkConfig> perks = new();

        public Sprite Icon => icon;
        public string Title => title;
        public bool IsBuildingOrSpell => isBuildingOrSpell;
        public int MaxPerGame => maxPerGame;
        public List<IPerk> Perks => perks.ConvertAll<IPerk>(perk => perk);
    }
}