using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Services.Saves;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private SceneLoader _sceneLoader;
        [Inject] private ISaveManager _saveManager;
        
        // This injects only to initialize them onConstruct
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private CardsDatabase _cardsDatabase;
        [Inject] private PlayerDeck _playerDeck;
        
        private void Start()
        {
            Application.targetFrameRate = 60;

            Time.timeScale = 1f;
            
            _saveManager.Load();
            
            _playerDeck.InitializeAfterLoadSaves(_cardsDatabase);
            
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}