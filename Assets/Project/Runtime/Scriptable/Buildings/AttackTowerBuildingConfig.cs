using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/AttackTower")]
    public class AttackTowerBuildingConfig : BuildingConfig
    {
        [SerializeField] private float damage;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float attackRange;

        public float Damage => damage;
        public float AttackCooldown => attackCooldown;
        public float AttackRange => attackRange;
    }
}