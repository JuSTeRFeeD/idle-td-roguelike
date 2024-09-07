using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class AttackTowerView : EntityView
    {
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform rotatingTower;

        public Transform ShootPoint => shootPoint;

        private void Update()
        {
            if (!Entity.Has<AttackTarget>()) return;
            ref readonly var e = ref Entity.GetComponent<AttackTarget>().Value;
            if (e.IsNullOrDisposed()) return;
            var targetPos = e.ViewPosition();
            var dir = targetPos - transform.position;
            dir.y = 0;
            rotatingTower.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}