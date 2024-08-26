using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitGatheringResourceStartSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<UnitTag>()
                .With<MoveToTargetComplete>()
                .With<MoveToResource>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var progressUiEntity = World.CreateEntity();
                progressUiEntity.SetComponent(new Owner { Entity = entity });
                progressUiEntity.SetComponent(new FollowOwner());
                progressUiEntity.InstantiateView(_worldSetup.WorldProgressBarView, entity.ViewPosition(), Quaternion.identity);
                
                
                entity.SetComponent(new Gathering
                {
                    CurrentTime = 0,
                    TargetResource = entity.GetComponent<MoveToResource>().Entity,
                    ProgressEntity = progressUiEntity
                });
                entity.RemoveComponent<MoveToResource>();
            }
        }

        public void Dispose()
        {
        }
    }
}