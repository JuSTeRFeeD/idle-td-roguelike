using Project.Runtime.ECS.Views;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/AttackTower")]
    public class AttackTowerBuildingConfig : UpgradableTowerConfig
    {
        [Header("AttackTowerBuildingConfig")] 
        [SerializeField] private EntityView projectileView;
        [SerializeField] private float damage;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float attackRange;

        public EntityView ProjectileView => projectileView;
        public float Damage => damage;
        public float AttackCooldown => attackCooldown;
        public float AttackRange => attackRange;
    }
}