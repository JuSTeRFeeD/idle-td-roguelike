using System.Collections;
using Project.Runtime.Lobby.Missions;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Sirenix.OdinInspector;
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
        [Inject] private ServerTime _serverTime;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        // This injects only to initialize them onConstruct
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private CardsDatabase _cardsDatabase;
        [Inject] private MissionsDatabase _missionsDatabase;
        [Inject] private MissionsManager _missionsManager;
        
        [Title("Cards list configs")]
        [SerializeField] private ActiveCardsListConfig commonCardsList;
        [SerializeField] private ActiveCardsListConfig firstTimeCardsList;
        
        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            Time.timeScale = 1f;

            YG2.GameReadyAPI();
            while (!YG2.isSDKEnabled)
            {
                yield return null;
            }

            _playerDeck.InitializeAfterLoadSaves(commonCardsList, firstTimeCardsList);
            _serverTime.Refresh();
            _missionsManager.Initialize();
            
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}