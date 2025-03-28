using Project.Runtime.ECS.Views;
using UnityEngine;

namespace Project.Runtime.Scriptable.Enemies
{
    [CreateAssetMenu(menuName = "Game/Enemies/Enemy", fileName = "Enemy")]
    public class EnemyConfig : UniqueConfig
    {
        [SerializeField] private EnemyView enemyView;
        [SerializeField] private float health;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCooldown;
        [SerializeField] private EnemyType enemyType;

        public EnemyView EnemyView => enemyView;
        public float Health => health;
        public float MoveSpeed => moveSpeed;
        public float AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public EnemyType EnemyType => enemyType;
    }
}