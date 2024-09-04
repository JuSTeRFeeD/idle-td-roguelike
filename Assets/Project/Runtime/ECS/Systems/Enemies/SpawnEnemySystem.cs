using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Enemies
{
    public class SpawnEnemySystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
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
                
                // Move
                enemy.SetComponent(new MoveSpeed
                {
                    Value = request.EnemyConfig.MoveSpeed
                });
                enemy.SetComponent(new MoveSpeedRuntime
                {
                    Value = request.EnemyConfig.MoveSpeed
                });
                
                // Damage
                enemy.SetComponent(new AttackDamage
                {
                    Value = request.EnemyConfig.AttackDamage
                });
                enemy.SetComponent(new AttackDamageRuntime
                {
                    Value = request.EnemyConfig.AttackDamage
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