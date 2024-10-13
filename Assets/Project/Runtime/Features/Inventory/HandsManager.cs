using System;
using System.Collections.Generic;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.Widgets;
using Runtime.Features.Widgets;
using Sirenix.OdinInspector;
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
        [Title("SFX")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip putSound;

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
                item.OnDragCard += OnDragCardStart;
                
                _cardWidgets.Add(item);
            }
        }

        private void Clear()
        {
            foreach (var i in _cardWidgets)
            {
                i.OnDragCard -= OnDragCard;
                i.OnDragCardStart -= OnDragCardStart;
                Destroy(i.gameObject);
            }
            _cardWidgets.Clear();
        }

        private void OnDragCardStart(CardWidget cardWidget, PointerEventData eventData)
        {
            handsCardDropFrame.SetActive(true);
        }

        private void OnDragCard(CardWidget cardWidget, PointerEventData eventData)
        {
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
                SetPlacingEnabledEnabled(true);
                SetIsCardDrag(true);
            }
        }

        public void SetIsCardDrag(bool value)
        {
            IsCardDrag = value;
            if (value) placeCardPanel.Show();
            else placeCardPanel.Hide();
        }

        public void PlayPutSound()
        {
            source.PlayOneShot(putSound, 0.5f);
        }

        public void SetPlacingEnabledEnabled(bool value)
        {
            grid.SetActive(value);
            if (!value) handsCardDropFrame.SetActive(false);
        }
    }
}