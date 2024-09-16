using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.PlayerPassivePerks
{
    [CreateAssetMenu(menuName = "Game/Perks/ExpGainMultiplierPerk")]
    public class ExpGainMultiplierPerk : PerkConfig
    {
        public float[] multipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<PlayerLevel>().Build())
            {
                entity.SetComponent(new ExpGainIncreasePerk
                {
                    Multiplier = multipliers[applyIndex]
                });
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Increase {DescColors.SpecialColor}gaining exp</color> for {DescColors.ValueColor}{Mathf.RoundToInt(multipliers[applyIndex] * 100 - 100):##.#}%</color>";
        }
    }
}