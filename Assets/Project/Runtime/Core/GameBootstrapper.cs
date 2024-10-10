using System.Collections;
using Project.Runtime.Lobby.Missions;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Services.Saves;
using UnityEngine;
using VContainer;
using YG;

namespace Project.Runtime.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private SceneLoader _sceneLoader;
        [Inject] private ISaveManager _saveManager;
        [Inject] private PlayerDeck _playerDeck;
        
        // This injects only to initialize them onConstruct
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private CardsDatabase _cardsDatabase;
        [Inject] private MissionsDatabase _missionsDatabase;
        
        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;

            Time.timeScale = 1f;

            while (!YandexGame.SDKEnabled)
            {
                yield return null;
            }
            
            _saveManager.Load();
            
            _playerDeck.InitializeAfterLoadSaves(_cardsDatabase);
            
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}