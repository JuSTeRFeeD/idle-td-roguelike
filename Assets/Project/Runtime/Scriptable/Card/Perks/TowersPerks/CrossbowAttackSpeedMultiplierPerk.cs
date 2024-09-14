using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Crossbow/CrossbowAttackSpeedMultiplierPerk")]
    public class CrossbowAttackSpeedMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. Скорость атаки нужно увеличивать умножая на 0.9f, 0.5f")]
        [SerializeField] private float[] attackSpeedMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<CrossbowTowerPerkUpgrades>().Build())
            {
                entity.GetComponent<CrossbowTowerPerkUpgrades>().AttackSpeedMultiplier =
                    attackSpeedMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return "Increase attack speed of cannon towers";
        }   
    }
}