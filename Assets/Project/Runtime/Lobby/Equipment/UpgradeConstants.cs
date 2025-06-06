using Project.Runtime.Player;

namespace Project.Runtime.Lobby.Equipment
{
    public class UpgradeConstants
    {
        public const int FirstUpgradeCostSoft = 110;
        public const int AddCostToUpgradePerLevelSoft = 625;
        
        public const int FirstUpgradeCostHex = 420;
        public const int AddCostToUpgradePerLevelHex = 925;
        
        public const int FirstUpgradeAmount = 3;
        public const int AddAmountToUpgradePerLevel = 1;
        
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