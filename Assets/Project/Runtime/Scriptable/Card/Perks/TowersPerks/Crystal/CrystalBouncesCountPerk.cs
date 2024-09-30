using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Crystal
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Crystal/CrystalBouncesCountPerk")]
    public class CrystalBouncesCountPerk : PerkConfig
    {
        [SerializeField] private int[] setBouncesCount = { 1, 2, 4 };
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter
                         .With<CrystalTowerUpgradesTag>()
                         .With<TowerWithBouncesUpgrade>().Build())
            {
                entity.GetComponent<TowerWithBouncesUpgrade>().AdditionalBounces = setBouncesCount[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Increase {DescColors.SpecialColor}bounces count</color> of crystal tower between enemies up to {DescColors.ValueColor}{setBouncesCount[applyIndex]}";
        }
    }
}