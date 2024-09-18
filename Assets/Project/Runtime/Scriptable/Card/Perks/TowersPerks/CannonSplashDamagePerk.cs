using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Cannon/CannonSplashDamagePerk")]
    public class CannonSplashDamagePerk : PerkConfig
    {
        [SerializeField] private float[] additionalSplashDamagePercent = new[] { 0.30f, 0.6f, 1.1f };
        [SerializeField] private float[] additionalSplashDamageRadius = new[] { 1f, 2f, 3f };
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<TowerWithSplashDamageUpgrades>().With<CannonTowerUpgradesTag>().Build())
            {
                ref var data = ref entity.GetComponent<TowerWithSplashDamageUpgrades>();
                data.AdditionalSplashDamagePercent = additionalSplashDamagePercent[applyIndex];
                data.AdditionalSplashRadius = additionalSplashDamageRadius[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Cannon Tower gets additional " +
                   $"{DescColors.DamageColor}Splash Damage</color> ({DescColors.ValueColor}{additionalSplashDamagePercent[applyIndex] * 100:##.#}%</color>) " +
                   $"and {DescColors.SpecialColor}Splash Damage Range</color> ({DescColors.ValueColor}{additionalSplashDamageRadius[applyIndex]:##.#}</color>)";
        }
    }
}