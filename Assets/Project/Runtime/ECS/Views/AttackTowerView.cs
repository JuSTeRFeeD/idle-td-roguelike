using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class AttackTowerView : EntityView
    {
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform rotatingTower;
        [SerializeField] private TowerViewUpgrades towerViewUpgrades;

        public Transform ShootPoint => shootPoint;

        public TowerViewUpgrades TowerViewUpgrades => towerViewUpgrades;

        public void SetTowerRotation(Quaternion quaternion)
        {
            if (!rotatingTower) return;
            rotatingTower.rotation = quaternion;
        }
    }
}