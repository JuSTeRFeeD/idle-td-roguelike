using System;
using System.Collections.Generic;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.Features.Inventory
{
    public class HandsManager : MonoBehaviour
    {
        [Inject] private InventoryStorage _inventoryStorage;

        [SerializeField] private GameObject handsCardDropFrame;
        [SerializeField] private CardWidget inventoryCardWidgetPrefab;
        [SerializeField] private RectTransform handsCardsContainer;
        [Space]
        [SerializeField] private PlaceCardPanel placeCardPanel;
        [SerializeField] private GameObject grid;

        private readonly List<CardWidget> _cardWidgets = new();

        public event Action<CardWidget> OnCardUseStart;
        public event Action OnCardUseCancel;
        public bool IsCardDrag { get; private set; }
        
        private void Start()
        {
            SetIsCardDrag(false);
            Clear();
            placeCardPanel.OnCardUseCancel += OnCancel;
            _inventoryStorage.OnCardsChange += OnChange;
        }
        
        private void OnDestroy()
        {
            placeCardPanel.OnCardUseCancel -= OnCancel;
            _inventoryStorage.OnCardsChange += OnChange;
        }


        private void OnCancel() => OnCardUseCancel?.Invoke();

        private void OnChange()
        {
            Clear();
            
            foreach (var buildingConfig in _inventoryStorage.GetBuildingsList())
            {
                var item = Instantiate(inventoryCardWidgetPrefab, handsCardsContainer);
                
                item.SetConfig(buildingConfig);
                item.HidePoints();
                item.OnDragCard += OnDragCard;
                
                _cardWidgets.Add(item);
            }
        }

        private void Clear()
        {
            foreach (var i in _cardWidgets)
            {
                i.OnDragCard -= OnDragCard;
                Destroy(i.gameObject);
            }
            _cardWidgets.Clear();
        }

        private void OnDragCard(CardWidget cardWidget, PointerEventData eventData)
        {
            SetPlacingEnabledEnabled(true);
            
            // No building or spell
            if (!cardWidget.CardConfig.IsBuilding)
            {
                return;
            }

            // Waiting drag over Hands Card Drop Frame
            if (!eventData.pointerCurrentRaycast.gameObject)
            {
                return;
            }
            
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("HandsCardDropFrame"))
            {
                OnCardUseStart?.Invoke(cardWidget);
                SetIsCardDrag(true);
            }
        }

        public void SetIsCardDrag(bool value)
        {
            IsCardDrag = value;
            if (value) placeCardPanel.Show();
            else placeCardPanel.Hide();
        }

        public void SetPlacingEnabledEnabled(bool value)
        {
            grid.SetActive(value);
            handsCardDropFrame.SetActive(value);
        }
    }
}