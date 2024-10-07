using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class AttackTowerView : EntityView
    {
        [SerializeField] private Transform shootPoint;
        [Title("Optional")]
        [SerializeField] private Transform rotatingTower;
        [SerializeField] private TowerViewUpgrades towerViewUpgrades;
        [SerializeField] private Animator animator;

        public Transform ShootPoint => shootPoint;

        public TowerViewUpgrades TowerViewUpgrades => towerViewUpgrades;

        public Animator Animator => animator;

        public void SetTowerRotation(Quaternion quaternion)
        {
            if (!rotatingTower) return;
            rotatingTower.rotation = quaternion;
        }
    }
}