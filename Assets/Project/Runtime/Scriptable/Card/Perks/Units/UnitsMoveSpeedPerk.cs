using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.Units
{
    [CreateAssetMenu(menuName = "Game/Perks/Units/UnitMoveSpeedPerk")]
    public class UnitsMoveSpeedPerk : PerkConfig
    {
        [SerializeField] private float[] addMoveSpeedValue;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<UnitTag>().With<MoveSpeed>().Build())
            {
                entity.GetComponent<MoveSpeed>().Value += addMoveSpeedValue[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Увеличить {DescColors.SpeedColor}скорость передвижения</color> рабочих на {DescColors.ValueColor}{Mathf.RoundToInt(addMoveSpeedValue[applyIndex] * 10):##.#}</color>";
        }
    }
}