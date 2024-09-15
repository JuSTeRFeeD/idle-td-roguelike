using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Shooting
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)] 
    public class RotateTowerToAttackTargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<AttackTarget> _attackTargetStash;
        private Stash<ViewEntity> _viewEntityStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<BuildingTag>()
                .With<AttackTarget>()
                .Without<DestroyedTag>()
                .Build();
            _attackTargetStash = World.GetStash<AttackTarget>();
            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var target = ref _attackTargetStash.Get(entity).Value;
                if (target.IsNullOrDisposed()) continue;
                
                var attackTowerView = (AttackTowerView)_viewEntityStash.Get(entity).Value;

                var targetPos = _viewEntityStash.Get(target).Value.transform.position;
                var dir = targetPos - attackTowerView.transform.position;
                dir.y = 0;
                attackTowerView.SetTowerRotation(Quaternion.LookRotation(dir, Vector3.up));
            }
        }

        public void Dispose()
        {
        }
    }
}