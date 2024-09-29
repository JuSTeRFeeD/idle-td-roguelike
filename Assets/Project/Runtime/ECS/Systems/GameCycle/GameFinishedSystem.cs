using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Lobby.Map;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class GameFinishedSystem : ISystem
    {
        [Inject] private GameFinishedPanel _gameFinishedPanel;
        [Inject] private CameraController _cameraController;
        [Inject] private SceneSharedData _sceneSharedData;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;

        private Filter _filter;
        private bool _done;
        public World World { get; set; }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<GameFinishedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var isWin = entity.Has<GameWinTag>();

                if (isWin)
                {
                    // Save point as completed
                    _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex].IsCompleted = true;
                    _persistentPlayerData.MapData = LevelMapGenerator.Serialize(_sceneSharedData.MapPoints);
                }
                
                _saveManager.Save();
                _gameFinishedPanel.Setup(isWin);
                _gameFinishedPanel.Show();

                _cameraController.SetPosition(new Vector3(31, 0, 31));
                World.UpdateByUnity = false;
                TimeScale.OverrideNormalTimeScale(1f);
                TimeScale.SetTimeScale(1f);
            }
        }

        public void Dispose()
        {
        }
    }
}