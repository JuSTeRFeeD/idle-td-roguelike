using System;
using System.Collections;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using UnityEngine.Playables;
using VContainer;

namespace Project.Runtime.Lobby.Shop
{
    public class ChestOpeningController : MonoBehaviour
    {
        [Inject] private LobbyPanelsManager _lobbyPanelsManager;
        [Inject] private PersistentPlayerData _persistentPlayerData;

        [SerializeField] private PlayableDirector openChestPlayableDirector;
        [Space]
        [SerializeField] private MeshFilter chestTopMeshFilter;
        [SerializeField] private MeshFilter chestBottomMeshFilter;
        [Space]
        [SerializeField] private Mesh commonChestTopMesh;
        [SerializeField] private Mesh commonChestBottomMesh;
        [SerializeField] private Mesh epicChestTopMesh;
        [SerializeField] private Mesh epicChestBottomMesh;

        private bool _isOpening;
        
        private void Start()
        {
            openChestPlayableDirector.gameObject.SetActive(false);
        }

        public void OpenChest(ChestType chestType)
        {
            if (_isOpening) return;
            _isOpening = true;

            switch (chestType)
            {
                case ChestType.Common:
                    chestTopMeshFilter.mesh = commonChestTopMesh;
                    chestBottomMeshFilter.mesh = commonChestBottomMesh;
                    break;
                case ChestType.Epic:
                    chestTopMeshFilter.mesh = epicChestTopMesh;
                    chestBottomMeshFilter.mesh = epicChestBottomMesh;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(chestType), chestType, null);
            }
            
            StartCoroutine(DoOpenChest());
        }

        private IEnumerator DoOpenChest()
        {
            _lobbyPanelsManager.SetPanel(LobbyPanelType.None);
            
            openChestPlayableDirector.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);
            
            _lobbyPanelsManager.SetPanel(LobbyPanelType.Shop);
            _isOpening = false;
            openChestPlayableDirector.gameObject.SetActive(false);
        }
    }
}