using Project.Runtime.ECS.Components;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/BaseTower")]
    public class BaseBuildingConfig : BuildingConfig
    {
        [SerializeField] private float health;
        [Space] 
        [SerializeField] private int unitsCountDefault;
        [SerializeField] private MinMaxInt lumberjacksCapacity;
        [SerializeField] private MinMaxInt minerCapacity;
        [Space]
        [SerializeField] private int woodStorageSize;
        [SerializeField] private int stoneStorageSize;
        
        public float Health => health;
        
        public int UnitsCountDefault => unitsCountDefault;
        public MinMaxInt LumberjacksCapacity => lumberjacksCapacity;
        public MinMaxInt MinerCapacity => minerCapacity;
        
        public int WoodStorageSize => woodStorageSize;
        public int StoneStorageSize => stoneStorageSize;
    }
}