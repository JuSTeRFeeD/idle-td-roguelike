using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Scriptable.Card;

namespace Project.Runtime.Player
{
    public class PlayerDeck
    {
        private readonly ActiveCardsListConfig _commonCards;
        
        // todo удалить тк эти карточки будет выбираь игрок 
        private readonly ActiveCardsListConfig _devPlayerSelectedCards;
        
        public PlayerDeck(ActiveCardsListConfig commonCardsList, ActiveCardsListConfig devPlayerSelectedCards)
        {
            _commonCards = commonCardsList;
            _devPlayerSelectedCards = devPlayerSelectedCards;
        }

        public List<CardConfig> GetCardsForGame()
        {
            return _commonCards.Cards.Concat(_devPlayerSelectedCards.Cards).ToList();
        }
    }
}