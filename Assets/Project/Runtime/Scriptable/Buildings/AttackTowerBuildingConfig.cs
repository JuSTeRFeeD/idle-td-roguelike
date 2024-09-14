using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/AttackTower")]
    public class AttackTowerBuildingConfig : UpgradableTowerConfig
    {
        [Header("AttackTowerBuildingConfig")] 
        [SerializeField] private EntityView projectileView;
        [SerializeField] private MinMaxFloat damage;
        [SerializeField] private MinMaxFloat attackCooldown;
        [SerializeField] private MinMaxFloat attackRange;

        public EntityView ProjectileView => projectileView;
        public MinMaxFloat Damage => damage;
        public MinMaxFloat AttackCooldown => attackCooldown;
        public MinMaxFloat AttackRange => attackRange;
    }
}