using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
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
        [Space]
        [SerializeField] private int dropItemsCapacity;
        [SerializeField] private ChestDropItemView chestDropItemViewPrefab;
        [SerializeField] private RectTransform itemsContainer;
        
        private readonly List<ChestDropItemView> _dropItems = new();
        private bool _dropItemsInitialized;
        private Sequence _showItemsSequence;

        private State _state = State.None;

        private enum State
        {
            None,
            Animating,
            WaitClickToOpen,
            WaitClickToClose,
        }

        private int _totalDroppedItemsCount = 0;
        
        private void Start()
        {
            openChestPlayableDirector.gameObject.SetActive(false);
            
            openChestPanel.gameObject.SetActive(true);
            openChestPanel.Hide();
            openChestPanel.OnClick += OnClickUI;

            continueText.enabled = false;
        }

        private void InitializeDropItems()
        {
            if (_dropItemsInitialized) return;
            _dropItemsInitialized = true;
            for (var i = 0; i < dropItemsCapacity; i++)
            {
                _dropItems.Add(Instantiate(chestDropItemViewPrefab, itemsContainer));
            }
        }

        private void HideItems()
        {
            foreach (var item in _dropItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void OpenChest(ChestType chestType, DropChancesConfig dropChancesConfig, int amount)
        {
            if (_state != State.None) return;
            _state = State.Animating;

            InitializeDropItems();
            HideItems();
            
            _totalDroppedItemsCount = 0;
            
            // Currency
            var amountByCurrency = new Dictionary<CurrencyConfig, int>(10);
            for (var i = 0; i < amount; i++)
            {
                var droppedCurrencies = dropChancesConfig.GetRandomCurrencyDrops();
                foreach (var currencyDrop in droppedCurrencies)
                {
                    if (amountByCurrency.ContainsKey(currencyDrop.currencyConfig))
                    {
                        amountByCurrency[currencyDrop.currencyConfig] += currencyDrop.amount;
                    }
                    else
                    {
                        amountByCurrency.Add(currencyDrop.currencyConfig, currencyDrop.amount);
                    }
                }
            }
            foreach (var (currencyConfig, value) in amountByCurrency)
            {
                // saving
                _persistentPlayerData.WalletByCurrency[currencyConfig].Add((ulong)value);
                    
                // preview set
                _dropItems[_totalDroppedItemsCount].inventoryItemView.SetCurrencyData(currencyConfig, value);

                _totalDroppedItemsCount++;
            }

            // Cards 
            for (var i = 0; i < amount; i++)
            {
                var droppedCardConfig = dropChancesConfig.GetRandomCard();
                
                // saving
                var invDeckCard = _persistentPlayerData.InventoryCards.Find(cardSave => cardSave.id == droppedCardConfig.uniqueID);
                invDeckCard.amount++;
                invDeckCard.isOpen = true;

                // preview set
                _dropItems[_totalDroppedItemsCount].inventoryItemView.SetDeckCardData(new DeckCard
                {
                    CardConfig = droppedCardConfig,
                    CardSaveData = new CardSaveData
                    {
                        amount = 1,
                        isOpen = true
                    }
                });
                _totalDroppedItemsCount++;
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
            openChestPlayableDirector.gameObject.SetActive(true);
            openChestPlayableDirector.time = 0;
            openChestPlayableDirector.Play();
        }

        public void PauseAnim()
        {
            openChestPlayableDirector.Pause();
            openChestPanel.Show();
            continueText.enabled = true;
            _state = State.WaitClickToOpen;
        }

        private void OnClickUI()
        {
            if (_state == State.WaitClickToOpen)
            {
                ContinueAnim();
            }

            if (_state == State.WaitClickToClose)
            {
                EndAnim();
            }
        }

        private void ContinueAnim()
        {
            _state = State.Animating;
            openChestPlayableDirector.Resume();
            continueText.enabled = false;
        }

        public void OnChestOpened()
        {
            _showItemsSequence = DOTween.Sequence();
            var i = 0;
            for (; i < _totalDroppedItemsCount; i++)
            {
                var item = _dropItems[i];
                item.gameObject.SetActive(true);
                item.inventoryItemView.transform.localScale = Vector3.zero;
                _showItemsSequence.Join(
                    item.inventoryItemView.transform.DOScale(1f, 0.35f)
                        .SetEase(Ease.OutBack)
                        .SetDelay(0.1f * i)
                )
                .SetLink(item.inventoryItemView.gameObject);
            }
            _showItemsSequence.OnComplete(() =>
            {
                continueText.enabled = true;
                _state = State.WaitClickToClose;
            });
        }

        private void EndAnim()
        {
            _lobbyPanelsManager.SetPanel(LobbyPanelType.Shop);
            
            openChestPanel.Hide();
            openChestPlayableDirector.Stop();
            openChestPlayableDirector.gameObject.SetActive(false);
            _showItemsSequence?.Kill(true);

            _state = State.None;

            HideItems();
        }
    }
}