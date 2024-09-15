using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.Leveling;
using Project.Runtime.Scriptable.Card;
using Scellecs.Morpeh;
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
        [Inject] private HandsManager _handsManager;
        
        public World World { get; set; }

        private Filter _levelUpFilter;
        private Filter _choosingCardFilter;
        private Filter _placingCardFilter;

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

            _placingCardFilter = World.Filter
                .With<PlacingBuildingCard>()
                .With<ViewEntity>()
                .Build();

            _levelUpPanel.OnCardSelect += OnCardSelect;
        }

        private void OnCardSelect(CardConfig cardConfig)
        {
            World.UpdateByUnity = true;
            
            foreach (var entity in _choosingCardFilter)
            {
                entity.RemoveComponent<IsChoosingLevelUpCard>();
            }
            
            // Card need put to storage
            if (cardConfig.IsBuildingOrSpell)
            {
                _inventoryStorage.AddCard(cardConfig);
                return;
            }
            
            // Applying card perks to world
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
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _levelUpFilter)
            {
                // Cancelling placing card
                foreach (var placingCardEntity in _placingCardFilter)
                {
                    ref readonly var placingCard = ref placingCardEntity.GetComponent<PlacingBuildingCard>();
                    placingCard.CellEntity.Dispose();
                    placingCardEntity.Dispose();
                    _handsManager.SetIsCardDrag(false);
                    _handsManager.SetPlacingEnabledEnabled(false);
                } 
                
                // Level up
                ref var levelUp = ref entity.GetComponent<LevelUp>();
                levelUp.LevelUpsCount--;
                if (levelUp.LevelUpsCount <= 0)
                {
                    entity.RemoveComponent<LevelUp>();
                }
                
                
                World.UpdateByUnity = false;
                
                _levelUpPanel.Show();

                entity.AddComponent<IsChoosingLevelUpCard>();
            }
        }

        public void Dispose()
        {
        }
    }
}