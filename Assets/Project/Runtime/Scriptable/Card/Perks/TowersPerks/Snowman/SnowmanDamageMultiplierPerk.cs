using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Snowman
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Snowman/SnowmanDamageMultiplierPerk")]
    public class SnowmanDamageMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. RuntimeDamage будет умножен на число")]
        [SerializeField] private float[] attackDamageMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter
                         .With<SnowmanTowerUpgradesTag>()
                         .With<TowerAttackUpgrades>()
                         .Build())
            {
                entity.GetComponent<TowerAttackUpgrades>().AttackDamageMultiplier = attackDamageMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Увеличить {DescColors.DamageColor}урон</color> снеговика на {DescColors.ValueColor}{Mathf.RoundToInt(attackDamageMultipliers[applyIndex] * 100 - 100):##.#}%</color>";
        }
    }
}