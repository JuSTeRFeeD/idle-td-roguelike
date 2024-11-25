using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.Leveling;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Services.PlayerProgress;
using Runtime.Features.Tutorial;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Tutorial
{
    public struct TutorialPreventLevelUp : IComponent {}
    public struct TutorialPreventChangeDayTime : IComponent {}
    public struct TutorialPreventSpawnUnits : IComponent {}
    
    public class TutorialSystem : ISystem
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private TutorialPanel _tutorialPanel;
        [Inject] private LevelUpPanel _levelUpPanel;
        [Inject] private InventoryStorage _inventoryStorage;
        
        public World World { get; set; }

        private Filter _preventLevelUpFilter;
        private Filter _preventChangeDayTimeFilter;
        private Filter _tutorialWaitFirstPlaceTowerFilter;
        private Filter _preventSpawnUnitsFilter;
        private Filter _upgradeTowerFilter;
        
        public void OnAwake()
        {
            if (_persistentPlayerData.IsInGameTutorialCompleted) return;

            _upgradeTowerFilter = World.Filter.With<UpgradeBuildingRequest>().Build();
            
            _tutorialPanel.StepClicked += OnStepConfirmClicked;
            
            World.CreateEntity().SetComponent(new TutorialPreventSpawnUnits());
            _preventSpawnUnitsFilter = World.Filter.With<TutorialPreventSpawnUnits>().Build();
            
            World.CreateEntity().SetComponent(new TutorialPreventChangeDayTime());
            _preventChangeDayTimeFilter = World.Filter.With<TutorialPreventChangeDayTime>().Build();
            
            TimeScale.SetTimeScale(0f);
            _tutorialPanel.ShowStep(0);
            _preventLevelUpFilter = World.Filter.With<TutorialPreventLevelUp>().Build();
            World.CreateEntity().SetComponent(new TutorialPreventLevelUp());

            _levelUpPanel.OnCardSelect += OnSelectCardFirstTime;
            _inventoryStorage.OnRemoveCard += PlacedCardFirstTime;
        }

        private void OnSelectCardFirstTime(CardConfig _)
        {
            _levelUpPanel.OnCardSelect -= OnSelectCardFirstTime;
            
            Debug.Log("first time card select");
            _tutorialPanel.ShowStep(1);
            
            var pauseEntity = World.CreateEntity();
            pauseEntity.SetComponent(new TutorialPreventLevelUp());
        }

        private void PlacedCardFirstTime()
        {
            _inventoryStorage.OnRemoveCard -= PlacedCardFirstTime;
            _tutorialPanel.ClearStep();
            _tutorialPanel.ShowStep(2);
        }

        private void OnStepConfirmClicked()
        {
            if (_tutorialPanel.CurrentStep is 0 or 2)
            {
                TimeScale.SetTimeScale(1f);
            }

            if (_tutorialPanel.CurrentStep == 0)
            {
                foreach (var entity in _preventLevelUpFilter)
                    entity.Dispose();
            }
            else if (_tutorialPanel.CurrentStep == 2)
            {
                World.CreateEntity().SetComponent(new TutorialPreventLevelUp());
                TimeScale.SetTimeScale(0f);
                _tutorialPanel.ShowStep(3);
            } 
            else if (_tutorialPanel.CurrentStep == 3)
            {
                TimeScale.SetTimeScale(1f);
                foreach (var entity in _preventLevelUpFilter)
                    entity.Dispose();
                foreach (var entity in _preventSpawnUnitsFilter)
                    entity.Dispose();
                foreach (var entity in _preventChangeDayTimeFilter)
                    entity.Dispose();
                Debug.Log("3");
                
                _tutorialPanel.ShowStep(4);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_tutorialPanel.CurrentStep == 4 && _upgradeTowerFilter.IsNotEmpty())
            {
                _tutorialPanel.ClearStep();
            }
        }

        public void Dispose() { }
    }
}