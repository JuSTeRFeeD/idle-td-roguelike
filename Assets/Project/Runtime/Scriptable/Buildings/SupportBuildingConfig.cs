using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/SupportBuilding")]
    public class SupportBuildingConfig : UpgradableTowerConfig
    {
        public enum SupportTowerType
        {
            Dummy,
            Candy,
        }

        [Title("SupportBuildingConfig")]
        [SerializeField] private SupportTowerType supportTowerType;
        [SerializeField] private bool isOneLifeBuilding;
        
        public SupportTowerType SupportType => supportTowerType;
        public bool IsOneLifeBuilding => isOneLifeBuilding;
    }
}