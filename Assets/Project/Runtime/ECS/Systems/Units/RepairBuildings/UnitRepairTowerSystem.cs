using NTC.Pool;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Units.RepairBuildings
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class UnitRepairTowerSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _moveEndFilter;
        private Filter _repairingFilter;
        
        public void OnAwake()
        {
            _moveEndFilter = World.Filter
                .With<UnitMoveToRepairTower>()
                .With<MoveToTargetCompleted>()
                .Build();
            
            _repairingFilter = World.Filter
                .With<UnitRepairingTower>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // On move to tower complete
            foreach (var entity in _moveEndFilter)
            {
                ref readonly var tower = ref entity.GetComponent<UnitMoveToRepairTower>().Tower;
                var progressEntity = World.CreateEntity();
                progressEntity.SetComponent(new Owner { Entity = entity });
                progressEntity.InstantiateView(_worldSetup.WorldProgressBarView, entity.ViewPosition(), Quaternion.identity);
                entity.SetComponent(new UnitRepairingTower
                {
                    Tower = tower,
                    Progress = 0,
                    TowerHealthOnStart = tower.GetComponent<HealthCurrent>().Value,
                    ProgressEntity = progressEntity
                });
                entity.RemoveComponent<UnitMoveToRepairTower>();
            }
            
            // Repairing
            foreach (var entity in _repairingFilter)
            {
                ref var repairingTowerData = ref entity.GetComponent<UnitRepairingTower>();
                var tower = repairingTowerData.Tower;

                ref readonly var defaultHealth = ref tower.GetComponent<HealthDefault>().Value;
                ref var currentHealth = ref tower.GetComponent<HealthCurrent>().Value;

                currentHealth += deltaTime * defaultHealth * 0.5f; // TODO: можно вынести параметр для ускорения починки таверов
                repairingTowerData.Progress = (currentHealth - repairingTowerData.TowerHealthOnStart) / (defaultHealth - repairingTowerData.TowerHealthOnStart);
                
                if (currentHealth >= defaultHealth)
                {
                    currentHealth = defaultHealth;
                    Debug.Log($"Tower Repaired {currentHealth}/{defaultHealth}");
                    
                    repairingTowerData.ProgressEntity.Dispose();
                    tower.RemoveComponent<SomeUnitInteractsWithThisTag>();
                    tower.RemoveComponent<BuildingDamagedTag>();
                    if (tower.Has<DestroyedTag>())
                    {
                        if (tower.Has<DestroyedView>())
                        {
                            NightPool.Despawn(tower.GetComponent<DestroyedView>().Value);
                            tower.ViewTransform().gameObject.SetActive(true);
                            tower.RemoveComponent<DestroyedView>();
                        }
                        tower.RemoveComponent<DestroyedTag>();
                    }
                    if (tower.Has<HealthbarEntityRef>())
                    {
                        tower.GetComponent<HealthbarEntityRef>().Value.Dispose();
                        tower.RemoveComponent<HealthbarEntityRef>();
                    }
                    
                    entity.RemoveComponent<UnitRepairingTower>();
                    
                    if (!entity.GetComponent<UnitBackpack>().IsCapacityReached)
                    {
                        entity.SetComponent(new FindResourceRequest());
                    }
                    else
                    {
                        entity.SetComponent(new FindStorageRequest());
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}