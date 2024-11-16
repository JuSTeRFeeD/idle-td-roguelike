using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using Project.Runtime.Scriptable.Enemies;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    public enum AttackTowerType
    {
        Cannon = 0,
        Crossbow = 1,
        Crystal = 2,
        Bomb = 3,
        Tomb = 4,
        Snowman = 5,
        Pumpkin = 6,
    }
    
    [CreateAssetMenu(menuName = "Game/Buildings/AttackTower")]
    public class AttackTowerBuildingConfig : UpgradableTowerConfig
    {
        [Title("AttackTowerBuildingConfig")] 
        [SerializeField] private EnemyType[] focusEnemyTypes;
        
        [Space]
        [SerializeField] private AttackTowerType attackTowerType;
        
        [Title("Projectile")]
        [SerializeField] private EntityView projectileView;
        [SerializeField] private ParticleSystem shootVfx;
        [SerializeField] private ParticleSystem hitVfx;
        [SerializeField, MinValue(2)] private float projectileSpeed = 8;
        
        [Title("Stats")]
        [SerializeField] private MinMaxFloat damage;
        [SerializeField] private MinMaxFloat attackCooldown;
        [SerializeField] private MinMaxFloat attackRange;

        public EnemyType[] FocusEnemyTypes => focusEnemyTypes;
        
        public AttackTowerType AttackTowerType => attackTowerType;
        
        public EntityView ProjectileView => projectileView;
        public ParticleSystem ShootVfx => shootVfx;
        public ParticleSystem HitVfx => hitVfx;
        public float ProjectileSpeed => projectileSpeed;
        
        
        public MinMaxFloat Damage => damage;
        public MinMaxFloat AttackCooldown => attackCooldown;
        public MinMaxFloat AttackRange => attackRange;
    }
}