using System.Runtime.CompilerServices;
using NTC.Pool;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class DeathSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private VfxSetup _vfxSetup;
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        private Filter _buildingFilter;
        private Filter _enemyFilter;

        private Stash<OneLifeTag> _oneLifeTagStash;
        private Stash<HealthbarEntityRef> _healthbarEntityRefStash;
        
        public void OnAwake()
        {
            _buildingFilter = World.Filter
                .With<BuildingTag>()
                .With<HealthCurrent>()
                .With<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();
            
            _enemyFilter = World.Filter
                .With<EnemyTag>()
                .With<HealthCurrent>()
                .With<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();
            
            _healthbarEntityRefStash = World.GetStash<HealthbarEntityRef>();
            _oneLifeTagStash = World.GetStash<OneLifeTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            // Buildings
            foreach (var entity in _buildingFilter)
            {
                if (!entity.Has<BombTowerTag>()) // bomb has custom explosion vfx
                {
                    VfxPool.Spawn(_vfxSetup.DestroyTowerVfx, entity.ViewPosition());
                }
                
                if (_oneLifeTagStash.Has(entity))
                {
                    if (!string.IsNullOrEmpty(entity.GetComponent<BuildingTag>().BuildingConfigId))
                    {
                        _mapManager.DestroyBuilding(entity.ViewPosition());    
                    }
                    
                    DisposeHealthbar(entity);
                    entity.Dispose();
                    continue;
                }
                
                // Spawning "destroyed view" for this towers
                entity.SafeRemove<AttackTarget>();
                entity.RemoveComponent<ToDestroyTag>();
                entity.AddComponent<DestroyedTag>();
                SpawnDestroyedBuildingView(entity);    
            }
            
            // Enemies
            foreach (var entity in _enemyFilter)
            {
                var addExp = World.CreateEntity();
                addExp.SetComponent(new PlayerAddExp
                {
                    Value = 4.5f
                });
                
                DisposeHealthbar(entity);
                entity.Dispose();
            }
        }

        private void DisposeHealthbar(in Entity entity)
        {
            if (_healthbarEntityRefStash.Has(entity)) 
            {
                _healthbarEntityRefStash.Get(entity).Value.Dispose();
            }
        }

        // Disable/Hide real view & spawn destroyed view
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SpawnDestroyedBuildingView(Entity entity)
        {
            ref var view = ref entity.GetComponent<ViewEntity>().Value;
            var viewTransform = view.transform;
            
            view.gameObject.SetActive(false);
            var destroyedView = NightPool.Spawn(
                _worldSetup.DestroyedBuildingView, 
                viewTransform.position,
                viewTransform.rotation);
            entity.SetComponent(new DestroyedView
            {
                Value = destroyedView
            });
        }

        public void Dispose()
        {
        }
    }
}