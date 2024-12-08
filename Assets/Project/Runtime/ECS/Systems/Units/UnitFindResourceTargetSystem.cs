using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Systems.FindTarget;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Units
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class UnitFindResourceTargetSystem : ISystem
    {
        [Inject] private ResourceCounter _resourceCounter;
        
        public World World { get; set; }

        private Filter _unitsFilter;
        private Filter _anyResourcesFilter;
        private Filter _treeResourcesFilter;
        private Filter _stoneResourcesFilter;
        
        private Stash<ViewEntity> _viewEntityStash;
        
        public void OnAwake()
        {
            _unitsFilter = World.Filter
                .With<UnitTag>()
                .With<FindResourceRequest>()
                .With<ViewEntity>()
                .With<UnitBackpack>()
                .Without<MoveToResource>()
                .Build();
            
            _anyResourcesFilter = World.Filter
                .With<MapResourceTag>()
                .With<ViewEntity>()
                .Without<SomeUnitInteractsWithThisTag>()
                .Build();
            _treeResourcesFilter  = World.Filter
                .With<MapResourceTag>()
                .With<TreeTag>()
                .With<ViewEntity>()
                .Without<SomeUnitInteractsWithThisTag>()
                .Build();
            _stoneResourcesFilter = World.Filter
                .With<MapResourceTag>()
                .With<StoneTag>()
                .With<ViewEntity>()
                .Without<SomeUnitInteractsWithThisTag>()
                .Build();

            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_resourceCounter.WoodFull && _resourceCounter.StoneFull)
            {
                foreach (var entity in _unitsFilter)
                {
                    entity.RemoveComponent<FindResourceRequest>();
                }
                return;
            }

            var woodIsPrimary = _resourceCounter.WoodAmount < _resourceCounter.StoneAmount;
            var stoneIsPrimary = _resourceCounter.WoodAmount > _resourceCounter.StoneAmount;
            var used = new HashSet<Entity>();

            if (woodIsPrimary)
            {
                foreach (var unitEntity in _unitsFilter)
                {
                    var nearestResource = FindTargetExtension.GetNearestByFilter(unitEntity, _treeResourcesFilter, 100f, _viewEntityStash, used);
                    if (nearestResource == null) continue;
                    SetTargetResource(unitEntity, nearestResource, used);
                }
            }
            
            if (stoneIsPrimary)
            {
                foreach (var unitEntity in _unitsFilter)
                {
                    var nearestResource = FindTargetExtension.GetNearestByFilter(unitEntity, _stoneResourcesFilter, 100f, _viewEntityStash, used);
                    if (nearestResource == null) continue;
                    SetTargetResource(unitEntity, nearestResource, used);
                }
            }
            
            World.Commit();
            
            foreach (var unitEntity in _unitsFilter)
            {
                var nearestResource = FindTargetExtension.GetNearestByFilter(unitEntity, _anyResourcesFilter, 100f, _viewEntityStash, used);
                if (nearestResource == null) continue;
                SetTargetResource(unitEntity, nearestResource, used);
            }
        }

        private void SetTargetResource(Entity unitEntity, Entity nearestResource, HashSet<Entity> used)
        {
            unitEntity.RemoveComponent<FindResourceRequest>();
            unitEntity.SetComponent(new AStarCalculatePathRequest
            {
                Entity = nearestResource,
                TargetPosition = _viewEntityStash.Get(nearestResource).Value.transform.position
            });
            unitEntity.SetComponent(new MoveToResource
            {
                Entity = nearestResource
            });
            nearestResource.SetComponent(new SomeUnitInteractsWithThisTag());
            used.Add(nearestResource);
        }

        public void Dispose() { }
    }
}