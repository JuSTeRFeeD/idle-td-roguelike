using Project.Runtime.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.Lobby
{
    public class TempToTheGame : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private SceneLoader _sceneLoader;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(_sceneLoader.LoadSceneAsync("Game"));
        }
    }
}