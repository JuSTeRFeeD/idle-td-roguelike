using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    public class UpgradableTowerConfig : BuildingConfig
    {
        [Title("UpgradableTowerConfig")]
        [Tooltip("0...5 == 6 levels")]
        [SerializeField] private int upgradeLevels = 5;
        [SerializeField] private float health;
        
        public float Health => health;
        public float UpgradeLevels => upgradeLevels;
        
    }
}