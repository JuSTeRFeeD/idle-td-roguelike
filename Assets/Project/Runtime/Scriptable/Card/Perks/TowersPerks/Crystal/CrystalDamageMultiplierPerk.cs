using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Crystal
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Crystal/CrystalDamageMultiplierPerk")]
    public class CrystalDamageMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. RuntimeDamage будет умножен на число")]
        [SerializeField] private float[] attackDamageMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter
                         .With<CrystalTowerUpgradesTag>()
                         .With<TowerAttackUpgrades>()
                         .Build())
            {
                entity.GetComponent<TowerAttackUpgrades>().AttackDamageMultiplier = attackDamageMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Increase {DescColors.DamageColor}attack damage</color> of crystal towers to {DescColors.ValueColor}{Mathf.RoundToInt(attackDamageMultipliers[applyIndex] * 100 - 100):##.#}%</color>";
        }
    }
}