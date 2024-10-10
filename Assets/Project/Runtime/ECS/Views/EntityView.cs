using Project.Runtime.ECS.Components.Enemies;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class EntityView : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        
        [SerializeField] private Transform realModelPosition;
        
        public Transform RealModelPosition => realModelPosition;
        
#if UNITY_EDITOR
        [Title("Dev")]
        public EntityId EntityId;
#endif

        public void SetEntity(Entity e)
        {
            Entity = e;
            
#if UNITY_EDITOR
            EntityId = e.ID;
#endif

            
            OnSetEntity();
        }

        protected virtual void OnSetEntity() { }

    
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Entity.IsNullOrDisposed()) return;
            if (!Entity.Has<Components.AStarPath>()) return;
            ref readonly var path = ref Entity.GetComponent<Components.AStarPath>();
            
            var prevPos = Features.Building.GridUtils.ConvertGridToWorldPos(path.Path[0]);
            if (Entity.Has<EnemyTag>()) Gizmos.color = Color.red;
            else Gizmos.color = Color.yellow;
            for (var i = 1; i < path.Path.Count; i++)
            {
                Gizmos.DrawWireSphere(prevPos, 0.2f);
                var gridPos = path.Path[i];
                var pos = Features.Building.GridUtils.ConvertGridToWorldPos(gridPos);
                Gizmos.DrawLine(prevPos, pos);
                prevPos = pos;
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Features.Building.GridUtils.ConvertGridToWorldPos(path.Path[path.CurrentPathIndex]), 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(path.CurrentTargetPosition, 0.4f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(path.RealTargetPosition, 0.7f);
        }
#endif
    }
}