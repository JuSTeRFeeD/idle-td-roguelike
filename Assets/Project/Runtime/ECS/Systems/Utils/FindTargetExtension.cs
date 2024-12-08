using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.FindTarget
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public static class FindTargetExtension
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
                if (sqrMagnitude > nearestSqrMagnitude) continue;
                nearestSqrMagnitude = sqrMagnitude;
                nearestEntity = target;
            }

            if (nearestEntity != null)
            {
                entity.SetComponent(new AttackTarget
                {
                    Value = nearestEntity
                });
            }
        }
        
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetNearestByFilter(in Entity entity, in Filter targetsFilter, float radius,
            Stash<ViewEntity> viewStash, HashSet<Entity> used)
        {
            var entityPos = viewStash.Get(entity).Value.transform.position;

            var nearestSqrMagnitude = radius * radius; // Начальное значение — квадрат радиуса атаки
            Entity nearestEntity = null;

            foreach (var target in targetsFilter)
            {
                var targetPos = viewStash.Get(target).Value.transform.position;
                var sqrMagnitude = (targetPos - entityPos).sqrMagnitude;

                // Если цель ближе и находится в радиусе атаки
                if (sqrMagnitude > nearestSqrMagnitude || used.Contains(target)) continue;
                nearestSqrMagnitude = sqrMagnitude;
                nearestEntity = target;
            }

            return nearestEntity;
        }
        
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetNearestByFilter(in Entity entity, in Filter targetsFilter, float radius,
            Stash<ViewEntity> viewStash)
        {
            var entityPos = viewStash.Get(entity).Value.transform.position;

            var nearestSqrMagnitude = radius * radius; // Начальное значение — квадрат радиуса атаки
            Entity nearestEntity = null;

            foreach (var target in targetsFilter)
            {
                var targetPos = viewStash.Get(target).Value.transform.position;
                var sqrMagnitude = (targetPos - entityPos).sqrMagnitude;

                // Если цель ближе и находится в радиусе атаки
                if (sqrMagnitude > nearestSqrMagnitude) continue;
                nearestSqrMagnitude = sqrMagnitude;
                nearestEntity = target;
            }

            return nearestEntity;
        }
        
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInRangeFilterNoAlloc(in Entity entity, in Filter targetsFilter, in float attackRange, in Stash<ViewEntity> viewStash, ref Entity[] hits)
        {
            var attackRangeSqr = attackRange * attackRange;
            var entityPos = viewStash.Get(entity).Value.transform.position;
            var count = 0;

            foreach (var target in targetsFilter)
            {
                var targetPos = viewStash.Get(target).Value.transform.position;
                var sqrMagnitude = (targetPos - entityPos).sqrMagnitude;

                // Если цель находится в радиусе атаки
                if (sqrMagnitude <= attackRangeSqr)
                {
                    hits[count++] = target;
                }
            }

            return count;
        }
    }
}