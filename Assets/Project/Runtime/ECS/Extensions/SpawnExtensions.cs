using System.Runtime.CompilerServices;
using NTC.Pool;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Extensions
{
    public static class SpawnExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeRemove<T>(this Entity entity) where T : struct, IComponent
        {
            if (entity.Has<T>()) entity.RemoveComponent<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Entity Owner(this Entity entity)
        {
            return ref entity.GetComponent<Owner>().Entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ViewTransform(this Entity entity)
        {
            return entity.GetComponent<ViewEntity>().Value.transform;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ViewPosition(this Entity entity)
        {
            return entity.GetComponent<ViewEntity>().Value.transform.position;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ViewRealModelPosition(this Entity entity)
        {
            return entity.GetComponent<ViewEntity>().Value.RealModelPosition.position;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T InstantiateView<T>(this Entity forEntity, EntityView view, Vector3 position, Quaternion quaternion)
            where T : EntityView
        {
            var spawned = NightPool.Spawn(view, position, quaternion);
            LinkView(forEntity, spawned);
            return spawned as T;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InstantiateView(this Entity forEntity, EntityView view, Vector3 position, Quaternion quaternion)
        {
            var spawned = NightPool.Spawn(view, position, quaternion);
            LinkView(forEntity, spawned);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkView(this Entity forEntity, EntityView spawnedView)
        {
            forEntity.SetComponent(new ViewEntity
            {
                Value = spawnedView
            });
            spawnedView.SetEntity(forEntity);
        }
    }
}