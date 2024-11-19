using Project.Runtime.Services.PlayerProgress;

namespace Project.Runtime.ECS.Systems.Building
{
    public static class ExtStatsByCardLevel
    {
        private const float AddHealthPercentByCardLvl = 0.1f;
        private const float AddDamagePercentByCardLvl = 0.15f;

        public static float GetAddHealthPercentByCardLvl(CardSaveData cardSaveData)
        {
            if (cardSaveData == null) return 0;
            return AddHealthPercentByCardLvl * cardSaveData.level;
        }
        
        public static float GetAddDamagePercentByCardLvl(CardSaveData cardSaveData)
        {
            if (cardSaveData == null) return 0;
            return AddDamagePercentByCardLvl * cardSaveData.level;
        }
    }
}