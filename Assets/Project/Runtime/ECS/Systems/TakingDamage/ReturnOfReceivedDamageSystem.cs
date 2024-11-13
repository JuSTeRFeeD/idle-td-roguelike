using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ReturnOfReceivedDamageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ReturnOfReceivedDamage>()
                .With<DamageAccumulator>()
                .Without<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var returnOfReceivedDamage = ref entity.GetComponent<ReturnOfReceivedDamage>();
                ref readonly var damageAccum = ref entity.GetComponent<DamageAccumulator>();
                
                for (var i = 0; i <= damageAccum.DamagersAmount && i < damageAccum.Damagers.Length; i++)
                {
                    var damager = damageAccum.Damagers[i];
                    
                    if (damager.IsNullOrDisposed()) continue;
                    if (damager.Has<ToDestroyTag>() || damager.Has<DestroyedTag>()) continue;

                    ref var returnDamageAccum = ref damager.AddOrGet<DamageAccumulator>();
                    returnDamageAccum.DamagersAmount++;
                    returnDamageAccum.Damagers ??= new Entity[UtilConstants.DamageAccumulatorDamagersCapacity];
                    returnDamageAccum.Damagers[returnDamageAccum.DamagersAmount] = entity;
                    
                    if (damager.Has<AttackDamageRuntime>())
                    {
                        // От наносимого урона
                        ref readonly var attackDamageRuntime = ref damager.GetComponent<AttackDamageRuntime>();
                        returnDamageAccum.Value += returnOfReceivedDamage.FixedReturnDamage + attackDamageRuntime.Value * returnOfReceivedDamage.Percent;
                    }
                    else
                    {
                        // Фикса
                        returnDamageAccum.Value += returnOfReceivedDamage.FixedReturnDamage;
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}