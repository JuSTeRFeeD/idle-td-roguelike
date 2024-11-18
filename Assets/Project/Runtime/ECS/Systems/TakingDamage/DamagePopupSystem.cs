using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class DamagePopupSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        private Stash<DamageAccumulator> _damageAccumulatorStash;
        private Stash<ViewEntity> _viewEntityStash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<DamageAccumulator>()
                .Build();

            _damageAccumulatorStash = World.GetStash<DamageAccumulator>();
            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var damageAccumulator = ref _damageAccumulatorStash.Get(entity);
                ref readonly var viewEntity = ref _viewEntityStash.Get(entity);
                var pos = viewEntity.Value.transform.position;

                var popupEntity = World.CreateEntity();
                var view = popupEntity.InstantiateView<PopupTextView>(_worldSetup.PopupTextView, pos, Quaternion.identity);
                view.SetValue(damageAccumulator.Value);
                view.SetIsCritical(damageAccumulator.IsCritical);
                popupEntity.SetComponent(new DestroyOverTime
                {
                    EstimateTime = 1.1f
                });
            }
        }

        public void Dispose()
        {
        }
    }
}