using UnityEngine;

namespace Project.Runtime.Scriptable.MapLevelConfigs
{
    [CreateAssetMenu(menuName = "Game/Global/GlobalDifficultSettingsConfig")]
    public class GlobalDifficultSettingsConfig : ScriptableObject
    {
        [Tooltip("От Y высоты запущенного уровня MapPoint")]
        [SerializeField] private float multiplyEnemiesHealthByHeight = 0.05f;
        [Tooltip("От кол-ва пройденных карт")]
        [SerializeField] private float multiplyEnemiesHealthByCompletedMaps = 0.1f;

        public float MultiplyEnemiesHealthByHeight => multiplyEnemiesHealthByHeight;
        public float MultiplyEnemiesHealthByCompletedMaps => multiplyEnemiesHealthByCompletedMaps;
    }
}