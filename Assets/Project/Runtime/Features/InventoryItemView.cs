using System;
using Project.Runtime.Player;
using Project.Runtime.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features
{
    public class InventoryItemView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image towerIconImage;
        [SerializeField] private Image rarityImage;
        [SerializeField] private TextMeshProUGUI amountText;

        public event Action<InventoryItemView> OnClick;

        public DeckCard DeckCard { get; private set; }
        
        public void SetDeckCardData(DeckCard deckCard)
        {
            DeckCard = deckCard;
            
            iconImage.sprite = deckCard.CardConfig.Icon;
            towerIconImage.sprite = deckCard.CardConfig.Icon;
            iconImage.enabled = !deckCard.CardConfig.IsBuilding;
            towerIconImage.enabled = deckCard.CardConfig.IsBuilding;
            
            var clr = deckCard.CardSaveData.isOpen ? Color.white : Color.black;
            iconImage.color = towerIconImage.color = clr; 
            
            rarityImage.color = RarityColors.GetColorByRarity(Rarity.Common);
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
    }
}