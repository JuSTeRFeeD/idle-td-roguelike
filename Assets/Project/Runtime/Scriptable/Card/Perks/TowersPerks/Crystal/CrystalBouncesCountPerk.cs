using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Crystal
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Crystal/CrystalBouncesCountPerk")]
    public class CrystalBouncesCountPerk : PerkConfig
    {
        [SerializeField] private int[] setBouncesCount = { 2, 3, 4 };
        
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
            return $"Увеличить {DescColors.SpecialColor}количество отскоков</color> снаряда после попадания кристалла до {DescColors.ValueColor}{setBouncesCount[applyIndex]}";
        }
    }
}