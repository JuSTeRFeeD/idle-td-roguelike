using System.Runtime.CompilerServices;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.FindTarget
{
    public static class FindByAttackRangeExt
    {
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindTargetWithFilter(in Entity entity, in Filter targetsFilter,
            Stash<AttackRangeRuntime> attackRangeRuntimeStash, Stash<ViewEntity> viewStash)
        {
            ref readonly var attackRange = ref attackRangeRuntimeStash.Get(entity).Value;
            var attackRangeSqr = attackRange * attackRange;
            var entityPos = viewStash.Get(entity).Value.transform.position;

            var nearestSqrMagnitude = attackRangeSqr; // Начальное значение — квадрат радиуса атаки
            Entity nearestEntity = null;

            foreach (var target in targetsFilter)
            {
                var targetPos = viewStash.Get(target).Value.transform.position;
                var sqrMagnitude = (targetPos - entityPos).sqrMagnitude;

                // Если цель ближе и находится в радиусе атаки
                if (sqrMagnitude < nearestSqrMagnitude)
                {
                    nearestSqrMagnitude = sqrMagnitude;
                    nearestEntity = target;
                }
            }

            if (nearestEntity != null)
            {
                entity.SetComponent(new AttackTarget
                {
                    Value = nearestEntity
                });
            }
        }
    }
}