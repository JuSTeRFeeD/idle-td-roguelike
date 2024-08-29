using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card
{
    [CreateAssetMenu(menuName = "Game/Cards/ActiveCardsList", fileName = "ActiveCardsList")]
    public class ActiveCardsListConfig : ScriptableObject
    {
        [SerializeField] private List<CardConfig> cards = new();

        public List<CardConfig> Cards => cards;

        public CardConfig GetCardByUniqueId(string id)
        {
            return cards.FirstOrDefault(i => i.uniqueID.Equals(id));
        }
    }
}