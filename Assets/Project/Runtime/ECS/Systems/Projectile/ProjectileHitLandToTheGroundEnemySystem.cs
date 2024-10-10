using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ProjectileHitLandToTheGroundEnemySystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileHit>()
                .With<TowerAttackLandsToTheGroundEnemy>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // Опускаем летающим мобов на землю
            
            foreach (var entity in _filter)
            {
                ref var hitEntity = ref entity.GetComponent<ProjectileHit>().HitEntity;
                hitEntity.SafeRemove<FlyingEnemyTag>();
                hitEntity.AddComponent<FlyingEnemyTag>();
            }
        }

        public void Dispose()
        {
        }
    }
}