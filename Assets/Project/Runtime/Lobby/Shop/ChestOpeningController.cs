using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Runtime.Features;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Shop
{
    public class ChestOpeningController : MonoBehaviour
    {
        [Inject] private LobbyPanelsManager _lobbyPanelsManager;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;

        [SerializeField] private PlayableDirector openChestPlayableDirector;
        [Space]
        [SerializeField] private MeshFilter chestTopMeshFilter;
        [SerializeField] private MeshFilter chestBottomMeshFilter;
        [Space]
        [SerializeField] private Mesh commonChestTopMesh;
        [SerializeField] private Mesh commonChestBottomMesh;
        [SerializeField] private Mesh epicChestTopMesh;
        [SerializeField] private Mesh epicChestBottomMesh;
        [Title("UI")]
        [SerializeField] private ChestOpenPanel openChestPanel;
        [SerializeField] private TextMeshProUGUI continueText;
        [SerializeField] private List<DropTuple> collectedItems;
        
        [Serializable]
        public struct DropTuple
        {
            public RectTransform container;
            public Image previewBrightImage;
            public InventoryItemView previewLootItem;
        }
        
        private bool _isOpening;
        private bool _isWaitClickToOpen;
        private bool _isWaitEndClick;

        private readonly List<CardConfig> _drops = new();
        
        private void Start()
        {
            openChestPlayableDirector.gameObject.SetActive(false);
            
            openChestPanel.gameObject.SetActive(true);
            openChestPanel.Hide();
            openChestPanel.OnClick += OnClickUI;

            continueText.enabled = false;
            HideAllDrops();
        }

        private void HideAllDrops()
        {
            foreach (var collectedItem in collectedItems)
            {
                collectedItem.container.gameObject.SetActive(false);
            }
        }

        public void OpenChest(ChestType chestType, DropChancesConfig dropChancesConfig, int amount)
        {
            if (_isOpening) return;
            _isOpening = true;

            _drops.Clear();
            for (var i = 0; i < amount; i++)
            {
                var droppedCardConfig = dropChancesConfig.GetRandomCard();
                var invDeckCard = _persistentPlayerData.InventoryCards.Find(cardSave => cardSave.id == droppedCardConfig.uniqueID);
                invDeckCard.isOpen = true;
                invDeckCard.amount++;
                _drops.Add(droppedCardConfig);
            }
            _saveManager.Save();

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
            
            _lobbyPanelsManager.SetPanel(LobbyPanelType.None);
            openChestPlayableDirector.Play();
            openChestPlayableDirector.gameObject.SetActive(true);
        }

        public void PauseAnim()
        {
            openChestPlayableDirector.Pause();
            openChestPanel.Show();
            _isWaitClickToOpen = true;
            continueText.enabled = true;
        }

        private void OnClickUI()
        {
            if (_isWaitClickToOpen)
            {
                ContinueAnim();
            }

            if (_isWaitEndClick)
            {
                EndAnim();
            }
        }

        private void ContinueAnim()
        {
            _isWaitClickToOpen = false;
            openChestPlayableDirector.Resume();
            continueText.enabled = false;
        }

        public void OnChestOpened()
        {
            var i = 0;
            for (; i < _drops.Count; i++)
            {
                var dropTuple = collectedItems[i];
                dropTuple.container.gameObject.SetActive(true);
                dropTuple.previewLootItem.transform.localScale = Vector3.zero;
                DOTween.Sequence()
                    .Append(dropTuple.previewLootItem.transform.DOScale(1f, 0.35f))
                    .SetEase(Ease.OutBack)
                    .SetLink(dropTuple.previewLootItem.gameObject)
                    .SetDelay(0.1f * i)
                    .OnComplete(() =>
                    {
                        continueText.enabled = true;
                        _isWaitEndClick = true;
                    });
            
                dropTuple.previewLootItem.SetDeckCardData(new DeckCard
                {
                    CardConfig = _drops[i],
                    CardSaveData = new CardSaveData { amount = 1, isOpen = true }
                });
            }

            for (; i < collectedItems.Count; i++)
            {
                collectedItems[i].container.gameObject.SetActive(false);
            }
        }

        private void EndAnim()
        {
            _lobbyPanelsManager.SetPanel(LobbyPanelType.Shop);
            
            openChestPanel.Hide();
            openChestPlayableDirector.gameObject.SetActive(false);
            
            _isOpening = false;
            _isWaitClickToOpen = false;
            _isWaitEndClick = false;

            HideAllDrops();
        }
    }
}