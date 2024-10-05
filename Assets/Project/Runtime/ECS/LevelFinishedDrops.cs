using Project.Runtime.Scriptable.Shop;

namespace Project.Runtime.ECS
{
    public class LevelFinishedDrops
    {
        public readonly DropChancesConfig WinConfig;
        public readonly DropChancesConfig LoseConfig;
        public readonly DropChancesConfig BonusWinConfig;

        public LevelFinishedDrops(DropChancesConfig winConfig, DropChancesConfig loseConfig, DropChancesConfig bonusWinConfig)
        {
            WinConfig = winConfig;
            LoseConfig = loseConfig;
            BonusWinConfig = bonusWinConfig;
        }
    }
}