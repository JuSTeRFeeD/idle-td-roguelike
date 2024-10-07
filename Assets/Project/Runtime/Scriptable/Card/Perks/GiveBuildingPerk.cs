using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks
{
    [CreateAssetMenu(menuName = "Game/Perks/Give Building Perk", fileName = "Give Building Perk")]
    public class GiveBuildingPerk : PerkConfig
    {
        [SerializeField] private BuildingConfig buildingConfig;

        public BuildingConfig BuildingConfig => buildingConfig;
        
        public override void Apply(World world, int applyIndex)
        {
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Дает карточку {DescColors.GiveTowerColor}{buildingConfig.Title}</color>, которую можно разместить на карте";
        }
    }
}