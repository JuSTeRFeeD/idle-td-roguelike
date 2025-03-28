using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.Scriptable.Enemies;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Enemies
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class NightTimeWaveSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _nightFilter;
        private Filter _baseTowerFilter;
        private Filter _enemiesFilter;
        
        private NightWavesConfig.WaveData _waveData;
        private readonly Dictionary<int, int> _limits = new();
        private int _lastSpawnedIndex = -1;
        private float _currentSpawnDelay;
        private float _elapsedTime;
        
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
                _elapsedTime += deltaTime;
                _currentSpawnDelay -= deltaTime;
                if (_currentSpawnDelay > 0)
                {
                    return;
                }
                _currentSpawnDelay = Random.Range(_waveData.spawnDelayBetween.x, _waveData.spawnDelayBetween.y);

                var baseTowerPos = _baseTowerFilter.First();
                if (baseTowerPos.IsNullOrDisposed()) return;

                for (var index = 0; index < _waveData.enemies.Count; index++)
                {
                    var waveDataEnemy = _waveData.enemies[index];
                    if (!_limits.ContainsKey(index)) continue;
                    if (waveDataEnemy.delay > _elapsedTime) continue;

                    var point = _worldSetup.EnemySpawnPoints[Random.Range(0, _worldSetup.EnemySpawnPoints.Length)];

                    var spawnRequest = World.CreateEntity();
                    spawnRequest.SetComponent(new SpawnEnemyRequest
                    {
                        EnemyConfig = waveDataEnemy.enemyConfig,
                        Position = point.position,
                        WaveIndex = _lastSpawnedIndex
                    });

                    if (--_limits[index] <= 0)
                    {
                        _limits.Remove(index);
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
                _elapsedTime = 0;
                _waveData = _worldSetup.NightWavesConfig.GetWave(index);
                for (var waveIndexKey = 0; waveIndexKey < _waveData.enemies.Count; waveIndexKey++)
                {
                    var waveDataEnemy = _waveData.enemies[waveIndexKey];
                    _limits.Add(waveIndexKey, waveDataEnemy.countToSpawn);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}