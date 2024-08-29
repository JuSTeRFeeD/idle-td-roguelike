using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Leveling;
using Project.Runtime.Scriptable.Card;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player.Upgrades
{
    public class LevelUpSystem : ISystem
    {
        [Inject] private LevelUpPanel _levelUpPanel;
        
        public World World { get; set; }

        private Filter _levelUpFilter;
        private Filter _choosingCardFilter;

        private readonly Dictionary<string, int> _appliesCountByPerkUniqueId = new();
        
        public void OnAwake()
        {
            _choosingCardFilter = World.Filter
                .With<IsChoosingLevelUpCard>()
                .Build();
            
            _levelUpFilter = World.Filter
                .With<LevelUp>()
                .With<PlayerLevel>()
                .Without<IsChoosingLevelUpCard>()
                .Build();

            _levelUpPanel.OnCardSelect += OnCardSelect;
        }

        private void OnCardSelect(CardConfig cardConfig)
        {
            foreach (var cardConfigPerk in cardConfig.Perks)
            {
                if (_appliesCountByPerkUniqueId.TryGetValue(cardConfig.uniqueID, out var appliesCount))
                {
                    cardConfigPerk.Apply(World, appliesCount);
                }
                else
                {
                    cardConfigPerk.Apply(World, 0);
                    _appliesCountByPerkUniqueId.Add(cardConfig.uniqueID, 1);
                }
            }
            
            Time.timeScale = 1f;
            foreach (var entity in _choosingCardFilter)
            {
                entity.RemoveComponent<IsChoosingLevelUpCard>();
            }
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _levelUpFilter)
            {
                ref var levelUp = ref entity.GetComponent<LevelUp>();
                levelUp.LevelUpsCount--;
                if (levelUp.LevelUpsCount <= 0)
                {
                    entity.RemoveComponent<LevelUp>();
                }
                
                Time.timeScale = 0.5f;
                
                _levelUpPanel.Show();

                entity.AddComponent<IsChoosingLevelUpCard>();
            }
        }

        public void Dispose()
        {
        }
    }
}