using System;
using Project.Runtime.ECS.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    public abstract class UpgradableTowerConfig : BuildingConfig
    {
        [Title("UpgradableTowerConfig")]
        [Tooltip("0...5 == 6 levels")]
        [SerializeField] private int upgradeLevels = 5;
        [SerializeField] private UpgradePrice[] upgradePrices;
        [SerializeField] private MinMaxFloat health;
        
        public MinMaxFloat Health => health;
        public int UpgradeLevels => upgradeLevels;
        public UpgradePrice[] UpgradePrices => upgradePrices;

        [Serializable]
        public struct UpgradePrice
        {
            public int woodPrice;
            public int stonePrice;
        }
    }
}