using Project.Runtime.Scriptable.Enemies;
using UnityEngine;

namespace Project.Runtime.Scriptable.MapLevelConfigs
{
    [CreateAssetMenu(menuName = "Game/Global/MapLevelConfig")]
    public class MapLevelConfig : ScriptableObject
    {
        [SerializeField] private NightWavesConfig[] commonConfig;
        [SerializeField] private NightWavesConfig[] bonusConfig;
        [SerializeField] private NightWavesConfig[] bossWavesConfig;

        public NightWavesConfig GetCommonConfig(int completedMapsCount)
        {
            if (completedMapsCount >= commonConfig.Length)
                completedMapsCount = commonConfig.Length - 1;
            return commonConfig[completedMapsCount];
        }
        
        public NightWavesConfig GetBonusConfig(int completedMapsCount)
        {
            if (completedMapsCount >= bonusConfig.Length)
                completedMapsCount = bonusConfig.Length - 1;
            return bonusConfig[completedMapsCount];
        }
        
        public NightWavesConfig GetBossConfig(int completedMapsCount)
        {
            if (completedMapsCount >= bossWavesConfig.Length)
                completedMapsCount = bossWavesConfig.Length - 1;
            return bossWavesConfig[completedMapsCount];
        }
    }
}