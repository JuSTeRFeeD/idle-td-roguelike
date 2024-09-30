using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Crystal
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Crystal/CrystalAttackSpeedMultiplierPerk")]
    public class CrystalAttackSpeedMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. Скорость атаки нужно увеличивать умножая на 0.9f, 0.5f")]
        [SerializeField] private float[] attackSpeedMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter
                         .With<CrystalTowerUpgradesTag>()
                         .With<TowerAttackUpgrades>()
                         .Build())
            {
                entity.GetComponent<TowerAttackUpgrades>().AttackSpeedMultiplier = attackSpeedMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Increase {DescColors.SpeedColor}attack speed</color> of crystal towers to {DescColors.ValueColor}{Mathf.RoundToInt(100 - attackSpeedMultipliers[applyIndex] * 100):##.#}%</color>";
        }   
    }
}