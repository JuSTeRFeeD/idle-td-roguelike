using System;
using DG.Tweening;
using Project.Runtime.Player;
using Project.Runtime.Scriptable;
using Project.Runtime.Scriptable.Currency;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features
{
    public class InventoryItemView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image towerIconImage;
        [SerializeField] private Image rarityImage;
        [SerializeField] private TextMeshProUGUI amountText;

        private Vector3 _initIconScale;
        private Vector3 _initIconTowerScale;
        public event Action<InventoryItemView> OnClick;

        public DeckCard DeckCard { get; private set; }

        private void Start()
        {
            _initIconScale = iconImage.transform.localScale;
            _initIconTowerScale = towerIconImage.transform.localScale;
        }

        public void SetCurrencyData(CurrencyConfig currencyConfig, int amount)
        {
            towerIconImage.enabled = false;
            iconImage.enabled = true;
            iconImage.sprite = currencyConfig.Icon;
            amountText.SetText($"{amount}");
            rarityImage.color = RarityExt.GetColorByRarity(currencyConfig.Rarity);
        }
        
        public void SetDeckCardData(DeckCard deckCard)
        {
            DeckCard = deckCard;
            
            iconImage.sprite = deckCard.CardConfig.Icon;
            towerIconImage.sprite = deckCard.CardConfig.Icon;
            iconImage.enabled = !deckCard.CardConfig.IsBuilding;
            towerIconImage.enabled = deckCard.CardConfig.IsBuilding;
            
            var clr = deckCard.CardSaveData.isOpen ? Color.white : Color.black;
            iconImage.color = towerIconImage.color = clr; 
            
            rarityImage.color = RarityExt.GetColorByRarity(deckCard.CardConfig.Rarity);
            if (deckCard.CardSaveData.isOpen)
            {
                amountText.SetText($"{deckCard.CardSaveData.level + 1}");
            }
            else
            {
                amountText.SetText(string.Empty);
            }
        }
        
        public void Clear()
        {
            iconImage.enabled = false;
            rarityImage.enabled = false;
            amountText.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            iconImage.transform.DOKill(true);
            towerIconImage.transform.DOKill(true);
            iconImage.transform.DOScale(_initIconScale * 1.2f, 0.1f).SetLink(iconImage.gameObject);
            towerIconImage.transform.DOScale(_initIconTowerScale * 1.2f, 0.1f).SetLink(iconImage.gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            iconImage.transform.DOKill(true);
            towerIconImage.transform.DOKill(true);
            iconImage.transform.DOScale(_initIconScale, 0.1f).SetLink(iconImage.gameObject);
            towerIconImage.transform.DOScale(_initIconTowerScale, 0.1f).SetLink(iconImage.gameObject);
        }
    }
}