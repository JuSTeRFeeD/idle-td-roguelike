using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using Project.Runtime.Scriptable.Enemies;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    public enum AttackTowerType
    {
        Cannon,
        Crossbow,
        Crystal,
        Bomb
    }
    
    [CreateAssetMenu(menuName = "Game/Buildings/AttackTower")]
    public class AttackTowerBuildingConfig : UpgradableTowerConfig
    {
        [Header("AttackTowerBuildingConfig")] 
        [SerializeField] private EnemyType[] focusEnemyTypes;
        [Space]
        [SerializeField] private AttackTowerType attackTowerType;
        [SerializeField] private EntityView projectileView;
        [MinValue(2)]
        [SerializeField] private float projectileSpeed = 8;
        [SerializeField] private MinMaxFloat damage;
        [SerializeField] private MinMaxFloat attackCooldown;
        [SerializeField] private MinMaxFloat attackRange;

        public EnemyType[] FocusEnemyTypes => focusEnemyTypes;
        public AttackTowerType AttackTowerType => attackTowerType;
        public EntityView ProjectileView => projectileView;
        public MinMaxFloat Damage => damage;
        public MinMaxFloat AttackCooldown => attackCooldown;
        public MinMaxFloat AttackRange => attackRange;
        public float ProjectileSpeed => projectileSpeed;
    }
}