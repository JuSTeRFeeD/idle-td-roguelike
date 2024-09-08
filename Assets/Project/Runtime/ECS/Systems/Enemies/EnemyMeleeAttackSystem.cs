using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Enemies
{
    public class EnemyMeleeAttackSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<EnemyTag>()
                .With<AttackTarget>()
                .With<AttackDamageRuntime>()
                .With<AttackRangeRuntime>()
                .With<AttackCooldownRuntime>()
                .Without<IsAttackOnCooldown>()
                .Without<AttackProjectileData>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var attackTarget = ref entity.GetComponent<AttackTarget>().Value;
                if (attackTarget.IsNullOrDisposed())
                {
                    entity.RemoveComponent<AttackTarget>();
                    continue;
                }
                
                ref readonly var attackCooldown = ref entity.GetComponent<AttackCooldownRuntime>().Value;
                entity.SetComponent(new IsAttackOnCooldown
                {
                    EstimateTime = attackCooldown
                });
                
                ref readonly var attackDamage = ref entity.GetComponent<AttackDamageRuntime>().Value;
                attackTarget.AddOrGet<DamageAccumulator>().Value += attackDamage;
                
                Debug.Log($"Enemy melee attack to {attackTarget.ID}");
            }
        }

        public void Dispose()
        {
        }
    }
}