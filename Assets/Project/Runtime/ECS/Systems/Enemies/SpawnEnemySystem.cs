using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Enemies
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class SpawnEnemySystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;

        private const float HealthMultiplierByWave = .2f; // +20% per wave index
        private const float DamageMultiplierByWave = .1f; // + 10% per wave index
        private const float MoveSpeedMultiplierByWave = .1f; // + 10% per wave index
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<SpawnEnemyRequest>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var request = ref entity.GetComponent<SpawnEnemyRequest>();

                var enemy = World.CreateEntity();
                enemy.InstantiateView(request.EnemyConfig.EnemyView, request.Position, Quaternion.identity);
                
                enemy.SetComponent(new EnemyTag());
                
                // Health
                var health = request.EnemyConfig.Health + 
                             request.EnemyConfig.Health * HealthMultiplierByWave * request.WaveIndex; 
                enemy.SetComponent(new HealthDefault
                {
                    Value = health
                });
                enemy.SetComponent(new HealthCurrent
                {
                    Value = health, 
                    GhostValue = health
                });
                
                // Move
                var moveSpeed = request.EnemyConfig.MoveSpeed + 
                                request.EnemyConfig.MoveSpeed * MoveSpeedMultiplierByWave * request.WaveIndex;
                enemy.SetComponent(new MoveSpeed
                {
                    Value = moveSpeed
                });
                enemy.SetComponent(new MoveSpeedRuntime
                {
                    Value = moveSpeed
                });
                
                // Damage
                var damage = request.EnemyConfig.AttackDamage + 
                             request.EnemyConfig.AttackDamage * DamageMultiplierByWave * request.WaveIndex; 
                enemy.SetComponent(new AttackDamage
                {
                    Value = damage  
                });
                enemy.SetComponent(new AttackDamageRuntime
                {
                    Value = damage
                });
                
                // Range
                enemy.SetComponent(new AttackRange
                {
                    Value = request.EnemyConfig.AttackRange
                });
                enemy.SetComponent(new AttackRangeRuntime
                {
                    Value = request.EnemyConfig.AttackRange
                });
                
                // Cooldown
                enemy.SetComponent(new AttackCooldown
                {
                    Value = request.EnemyConfig.AttackCooldown
                });
                enemy.SetComponent(new AttackCooldownRuntime
                {
                    Value = request.EnemyConfig.AttackCooldown
                });
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}