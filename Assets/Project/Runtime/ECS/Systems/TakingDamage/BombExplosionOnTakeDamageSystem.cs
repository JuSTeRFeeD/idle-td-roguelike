using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BombExplosionOnTakeDamageSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private VfxSetup _vfxSetup;
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        private Filter _filter;
        private Filter _bombPerkFilter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<BombTowerTag>()
                .With<DamageAccumulator>()
                .Without<IsAttackOnCooldown>()
                .Without<DestroyedTag>()
                .Build();
            
            _bombPerkFilter = World.Filter
                .With<DontDestroyBombTowerPerk>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                VfxPool.Spawn(_vfxSetup.BombExplosionVfx, entity.ViewPosition());
                
                var bombSplashDamageEntity = World.CreateEntity();
                bombSplashDamageEntity.InstantiateView(_worldSetup.NullView, entity.ViewPosition(), Quaternion.identity);
                bombSplashDamageEntity.SetComponent(entity.GetComponent<AttackDamageRuntime>());
                bombSplashDamageEntity.SetComponent(new ProjectileTag());
                bombSplashDamageEntity.SetComponent(new ProjectileHit
                {
                    HitEntity = bombSplashDamageEntity
                });
                bombSplashDamageEntity.SetComponent(new SplashDamageRuntime
                {
                    Radius = entity.GetComponent<AttackRangeRuntime>().Value,
                    PercentFromDamage = 1f
                });

                if (_bombPerkFilter.IsNotEmpty())
                {
                    ref readonly var chance = ref _bombPerkFilter.First().GetComponent<DontDestroyBombTowerPerk>()
                        .ChanceToDontDestroy;

                    if (Random.Range(0, 1f) > chance) return;
                    
                    // prevent bomb death & go to cooldown
                    entity.RemoveComponent<DamageAccumulator>(); 
                    entity.RemoveComponent<BuildingDamagedTag>();
                    
                    entity.AddComponent<DestroyedTag>();
                    entity.SetComponent(new IsAttackOnCooldown
                    {
                        EstimateTime = entity.GetComponent<AttackCooldownRuntime>().Value
                    });
                }
            }
        }

        public void Dispose()
        {
        }
    }
}