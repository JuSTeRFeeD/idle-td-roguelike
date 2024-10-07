using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Scriptable.Card.Perks.TowersPerks.Bomb
{
    [CreateAssetMenu(menuName = "Game/Perks/Towers/Bomb/BombDontDestroyPerk")]
    public class BombDontDestroyPerk : PerkConfig
    {
        [SerializeField] private float[] chanceToDontDestroy = { 0.35f, 0.55f, 0.95f };
        
        public override void Apply(World world, int applyIndex)
        {
            var filter = world.Filter.With<DontDestroyBombTowerPerk>().Build();
            if (filter.IsEmpty())
            {
                world.CreateEntity().SetComponent(new DontDestroyBombTowerPerk
                {
                    ChanceToDontDestroy  = chanceToDontDestroy[applyIndex]
                });
            }
            else
            {
                filter.First().GetComponent<DontDestroyBombTowerPerk>().ChanceToDontDestroy =
                    chanceToDontDestroy[applyIndex];
            }
        }

        public override string GetPerkDescription(int applyIndex)
        {
            return $"{DescColors.GiveTowerColor}Бомба</color> имеет шанс {DescColors.ValueColor}{chanceToDontDestroy[applyIndex] * 100:##.#}%</color> уйти на перезарядку вместо уничтожения";
        }
    }
}