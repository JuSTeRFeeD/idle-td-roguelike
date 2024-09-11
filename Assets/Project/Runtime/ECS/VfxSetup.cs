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
        
        public ParticleSystem PutTowerVfx => putTowerVfx;
        public ParticleSystem TowerLevelUpVfx => towerLevelUpVfx;
        public ParticleSystem DestroyTowerVfx => destroyTowerVfx;
    }
}