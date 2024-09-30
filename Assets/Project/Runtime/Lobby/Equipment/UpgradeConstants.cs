using Project.Runtime.Player;

namespace Project.Runtime.Lobby.Equipment
{
    public class UpgradeConstants
    {
        public const int FirstUpgradeCost = 100;
        public const int AddCostToUpgradePerLevel = 125;
        
        public const int FirstUpgradeAmount = 3;
        public const int AddAmountToUpgradePerLevel = 2;
        
        public const float HealthMultiplierPerLevel = 0.1f;
        public const float DamageMultiplierPerLevel = 0.15f;

        public static int GetAmountToUpgrade(DeckCard deckCard)
        {
            return FirstUpgradeAmount + deckCard.CardSaveData.level * AddAmountToUpgradePerLevel;
        }
        
        public static int GetUpgradeCost(DeckCard deckCard)
        {
            return FirstUpgradeCost + deckCard.CardSaveData.level * AddCostToUpgradePerLevel;
        }
    }
}