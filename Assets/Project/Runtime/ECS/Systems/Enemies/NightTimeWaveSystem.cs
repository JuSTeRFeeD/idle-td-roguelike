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
        private Filter _enemiesFilter;
        
        private int _lastSpawnedIndex = -1;
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
            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .Without<ToDestroyTag>()
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

                var baseTowerPos = _baseTowerFilter.First();
                if (baseTowerPos.IsNullOrDisposed()) return;

                foreach (var waveDataEnemy in _waveData.enemies)
                {
                    if (!_limits.ContainsKey(waveDataEnemy.enemyConfig)) continue;

                    var spawnDir = Random.insideUnitSphere;
                    spawnDir.y = 0;
                    
                    var spawnRequest = World.CreateEntity();
                    spawnRequest.SetComponent(new SpawnEnemyRequest
                    {
                        EnemyConfig = waveDataEnemy.enemyConfig,
                        Position = baseTowerPos.ViewPosition() + spawnDir.normalized * SpawnRadius
                    });
                        
                    if (--_limits[waveDataEnemy.enemyConfig] <= 0)
                    {
                        _limits.Remove(waveDataEnemy.enemyConfig);
                    }
                }

                if (_limits.Count == 0)
                {
                    _waveData = null;
                }
                
                return;
            }

            foreach (var entity in _nightFilter)
            {
                ref readonly var dayNight = ref entity.GetComponent<DayNight>();
                var index = dayNight.DayNumber - 1; // cuz dayNumber starts with 1, array from 0
                
                // ВОЗВРАЩАЕМ ДЕНЬ если сейчас ночь, нет врагов и индекс времени суток совпадает с закешированным
                if (_enemiesFilter.IsEmpty() && _lastSpawnedIndex == index)
                {
                    // Return to day time
                    entity.RemoveComponent<IsNightTimeTag>();
                    entity.GetComponent<DayNight>().EstimateTime = 3f;
                    return;
                }
                
                // Этой ночью уже не нужно спавнить врагов
                if (_lastSpawnedIndex == index || index >= _worldSetup.NightWavesConfig.WavesCount)
                {
                    return;
                }
                
                // Сохраняем данные чтобы спавнить врагов
                _limits.Clear();
                _lastSpawnedIndex = index;
                _waveData = _worldSetup.NightWavesConfig.GetWave(index); 
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