using Project.Runtime.ECS.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/PoisonAttackTower")]
    public class PoisonAttackTowerBuildingConfig : AttackTowerBuildingConfig
    {
        [Title("Poison tower")]
        [SerializeField] private MinMaxFloat poisonDamage;
        [SerializeField] private float poisonDustLifetime;
        [SerializeField] private float poisonDamageEachTime;
        [SerializeField] private float poisonRadius;
        [SerializeField] private ParticleSystem poisonDustVfx;
        
        public MinMaxFloat PoisonDamage => poisonDamage;
        public float PoisonDustLifetime => poisonDustLifetime;
        public float PoisonDamageEachTime => poisonDamageEachTime;
        public float PoisonRadius => poisonRadius;
        public ParticleSystem PoisonDustVfx => poisonDustVfx;
    }
}