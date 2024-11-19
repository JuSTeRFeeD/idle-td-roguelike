using Project.Runtime.Player;

namespace Project.Runtime.Lobby.Equipment
{
    public class UpgradeConstants
    {
        public const int FirstUpgradeCostSoft = 100;
        public const int AddCostToUpgradePerLevelSoft = 125;
        public const int FirstUpgradeCostHex = 200;
        public const int AddCostToUpgradePerLevelHex = 150;
        
        public const int FirstUpgradeAmount = 3;
        public const int AddAmountToUpgradePerLevel = 2;
        
        public const float HealthMultiplierPerLevel = 0.1f;
        public const float DamageMultiplierPerLevel = 0.15f;

        public static int GetCardAmountToUpgrade(DeckCard deckCard)
        {
            return FirstUpgradeAmount + deckCard.CardSaveData.level * AddAmountToUpgradePerLevel;
        }
        
        public static int GetUpgradeCostSoftCurrency(DeckCard deckCard)
        {
            return FirstUpgradeCostSoft + deckCard.CardSaveData.level * AddCostToUpgradePerLevelSoft;
        }
        
        public static int GetUpgradeCostHexCurrency(DeckCard deckCard)
        {
            return FirstUpgradeCostHex + deckCard.CardSaveData.level * AddCostToUpgradePerLevelHex;
        }
    }
}