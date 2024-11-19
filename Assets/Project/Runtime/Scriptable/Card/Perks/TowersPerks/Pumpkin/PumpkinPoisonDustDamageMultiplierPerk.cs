using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Pumpkin
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Pumpkin/PumpkinPoisonDustDamagePerk")]
    public class PumpkinPoisonDustDamageMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. RuntimeDamage будет умножен на число")]
        [SerializeField] private float[] poisonDustDamageMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter
                         .With<PumpkinTowerUpgradesTag>()
                         .With<PoisonDustDamageUpgrade>()
                         .Build())
            {
                entity.GetComponent<PoisonDustDamageUpgrade>().DustDamageMultiplier = poisonDustDamageMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"Увеличить {DescColors.DamageColor}урон</color> тыквы на {DescColors.ValueColor}{Mathf.RoundToInt(poisonDustDamageMultipliers[applyIndex] * 100 - 100):##.#}%</color>";
        }
    }
}