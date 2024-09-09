using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Enemies
{
    [CreateAssetMenu(menuName = "Game/Enemies/NightWavesConfig", fileName = "Night Waves Config")]
    public class NightWavesConfig : ScriptableObject
    {
        [Serializable]
        public class WaveEnemyData
        {
            public EnemyConfig enemyConfig;
            public int countToSpawn;
        }
        
        [Serializable]
        public class WaveData
        {
            public List<WaveEnemyData> enemies = new();
            [MinMaxSlider(0.05f, .5f)]
            public Vector2 spawnDelayBetween = new (0.1f, .5f);
        }

        [SerializeField] private List<WaveData> waves;

        public int WavesCount => waves.Count;
        
        public WaveData GetWave(int index)
        {
            return waves[index];
        }
    }
}