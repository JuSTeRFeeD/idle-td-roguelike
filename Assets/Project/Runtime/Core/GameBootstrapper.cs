using Project.Runtime.Features.Databases;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private SceneLoader _sceneLoader;
        
        // This injects only to initialize them
        [Inject] private BuildingsDatabase _buildingsDatabase;
        
        private void Start()
        {
            Application.targetFrameRate = 60;

            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}