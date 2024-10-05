using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.Leveling;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Scriptable.Card;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class LevelUpSystem : ISystem
    {
        [Inject] private InventoryStorage _inventoryStorage;
        [Inject] private LevelUpPanel _levelUpPanel;
        [Inject] private LevelUpCardsManager _levelUpCardsManager;
        [Inject] private HandsManager _handsManager;
        
        public World World { get; set; }

        private Filter _levelUpFilter;
        private Filter _choosingCardFilter;
        private Filter _placingCardFilter;
        
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

            _placingCardFilter = World.Filter
                .With<PlacingBuildingCard>()
                .With<ViewEntity>()
                .Build();

            _levelUpPanel.OnCardSelect += OnCardSelect;
        }

        private void OnCardSelect(CardConfig cardConfig)
        {
            TimeScale.SetNormalTimeScale();
            World.UpdateByUnity = true;
            
            foreach (var entity in _choosingCardFilter)
            {
                entity.RemoveComponent<IsChoosingLevelUpCard>();
            }
            
            // Card need put to storage
            if (cardConfig.IsBuilding)
            {
                _inventoryStorage.AddCard(cardConfig);
                return;
            }
            
            // Applying card perks to world
            foreach (var cardConfigPerk in cardConfig.Perks)
            {
                if (_levelUpCardsManager.AppliesCountByPerkUniqueId.TryGetValue(cardConfig.uniqueID, out var appliesCount))
                {
                    cardConfigPerk.Apply(World, appliesCount);
                }
                else
                {
                    cardConfigPerk.Apply(World, 0);
                    _levelUpCardsManager.AppliesCountByPerkUniqueId.Add(cardConfig.uniqueID, 1);
                }
            }
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _levelUpFilter)
            {
                // Cancelling placing card on level up
                foreach (var placingCardEntity in _placingCardFilter)
                {
                    ref readonly var placingCard = ref placingCardEntity.GetComponent<PlacingBuildingCard>();
                    if (placingCardEntity.Has<RadiusViewEntity>())
                    {
                        placingCardEntity.GetComponent<RadiusViewEntity>().Entity.Dispose();
                    }
                    placingCard.CellEntity.Dispose();
                    placingCardEntity.Dispose();
                    _handsManager.SetIsCardDrag(false);
                    _handsManager.SetPlacingEnabledEnabled(false);
                    EventSystem.current.SetSelectedGameObject(null);
                } 
                
                // Level up
                ref var levelUp = ref entity.GetComponent<LevelUp>();
                levelUp.LevelUpsCount--;
                if (levelUp.LevelUpsCount <= 0)
                {
                    entity.RemoveComponent<LevelUp>();
                }
                
                
                World.UpdateByUnity = false;
                Time.timeScale = 1f;
                
                _levelUpPanel.Show();

                entity.AddComponent<IsChoosingLevelUpCard>();
            }
        }

        public void Dispose()
        {
        }
    }
}