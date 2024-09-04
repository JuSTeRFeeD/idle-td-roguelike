using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Scriptable.Enemies;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Enemies
{
    public class NightTimeWaveSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _nightFilter;
        private Filter _baseTowerFilter;

        private float _currentSpawnDelay;
        private NightWavesConfig.WaveData _waveData;
        private readonly Dictionary<EnemyConfig, int> _limits = new();

        private const float SpawnRadius = 35f;
        
        public void OnAwake()
        {
            _nightFilter = World.Filter
                .With<DayNight>()
                .With<IsNightTimeTag>()
                .Build();
            _baseTowerFilter = World.Filter
                .With<BaseTowerTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_waveData != null)
            {
                _currentSpawnDelay -= deltaTime;
                if (_currentSpawnDelay > 0)
                {
                    return;
                }
                _currentSpawnDelay = Random.Range(_waveData.spawnDelayBetween.x, _waveData.spawnDelayBetween.y);

                var baseTowerPos = _baseTowerFilter.First().ViewPosition();
                
                foreach (var waveDataEnemy in _waveData.enemies)
                {
                    if (!_limits.ContainsKey(waveDataEnemy.enemyConfig)) continue;

                    var spawnDir = Random.insideUnitSphere;
                    spawnDir.y = 0;
                    
                    var spawnRequest = World.CreateEntity();
                    spawnRequest.SetComponent(new SpawnEnemyRequest
                    {
                        EnemyConfig = waveDataEnemy.enemyConfig,
                        Position = baseTowerPos + spawnDir.normalized * SpawnRadius
                    });
                        
                    if (--_limits[waveDataEnemy.enemyConfig] <= 0)
                    {
                        _limits.Remove(waveDataEnemy.enemyConfig);
                    }
                }
                
                return;
            }
            
            foreach (var entity in _nightFilter)
            {
                ref readonly var dayNight = ref entity.GetComponent<DayNight>();
                // cuz dayNumber starts with 1
                _waveData = _worldSetup.NightWavesConfig.GetWave(dayNight.DayNumber - 1); 
                _limits.Clear();
                foreach (var waveDataEnemy in _waveData.enemies)
                {
                    _limits.Add(waveDataEnemy.enemyConfig, waveDataEnemy.countToSpawn);   
                }
            }
        }

        public void Dispose()
        {
        }
    }
}