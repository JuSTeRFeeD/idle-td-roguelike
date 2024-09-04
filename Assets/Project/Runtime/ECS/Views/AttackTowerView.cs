using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class AttackTowerView : EntityView
    {
        [SerializeField] private Transform shootPoint;

        public Transform ShootPoint => shootPoint;
    }
}