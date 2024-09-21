using Project.Runtime.Player.Databases;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private SceneLoader _sceneLoader;
        
        // This injects only to initialize them
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private CardsDatabase _cardsDatabase;
        
        private void Start()
        {
            Application.targetFrameRate = 60;

            Time.timeScale = 1f;
            
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}