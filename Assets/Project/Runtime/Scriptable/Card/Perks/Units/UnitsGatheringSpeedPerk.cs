using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.Units
{
    [CreateAssetMenu(menuName = "Game/Perks/Units/UnitsGatheringSpeedPerk")]
    public class UnitsGatheringSpeedPerk : PerkConfig
    {
        [SerializeField] private float[] setGatheringTime;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<UnitTag>().With<GatheringTime>().Build())
            {
                entity.GetComponent<GatheringTime>().Time = setGatheringTime[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Increase {DescColors.SpecialColor}gathering speed</color> of all units";
        }
    }
}