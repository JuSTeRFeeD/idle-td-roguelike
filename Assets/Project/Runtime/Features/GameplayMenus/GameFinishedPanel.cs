using Project.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Features.GameplayMenus
{
    public class GameFinishedPanel : PanelBase
    {
        [Inject] private SceneLoader _sceneLoader;
        [SerializeField] private Button nextButton;

        private void Start()
        {
            nextButton.onClick.AddListener(ToTheLobby);
            Hide();
        }

        private void ToTheLobby()
        {
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }
    }
}