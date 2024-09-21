using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.ECS
{
    public class VfxSetup : MonoBehaviour
    {
        [Title("Building & Towers")]
        [SerializeField] private ParticleSystem putTowerVfx;
        [SerializeField] private ParticleSystem towerLevelUpVfx;
        [SerializeField] private ParticleSystem destroyTowerVfx;
        [SerializeField] private ParticleSystem cannonShootImpactVfx;
        
        public ParticleSystem PutTowerVfx => putTowerVfx;
        public ParticleSystem TowerLevelUpVfx => towerLevelUpVfx;
        public ParticleSystem DestroyTowerVfx => destroyTowerVfx;
        public ParticleSystem CannonShootImpactVfx => cannonShootImpactVfx;
    }
}