using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Cannon/CannonDamageMultiplierPerk")]
    public class CannonDamageMultiplierPerk : PerkConfig
    {
        [InfoBox("ЭТО МНОЖИТЕЛЬ. RuntimeDamage будет умножен на число")]
        [SerializeField] private float[] attackDamageMultipliers;
        
        public override void Apply(World world, int applyIndex)
        {
            foreach (var entity in world.Filter.With<CannonTowerPerkUpgrades>().Build())
            {
                entity.GetComponent<CannonTowerPerkUpgrades>().AttackDamageMultiplier =
                    attackDamageMultipliers[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return "Increase attack damage of cannon towers";
        }
    }
}