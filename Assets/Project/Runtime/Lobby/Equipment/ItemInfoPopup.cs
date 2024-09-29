using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using Project.Runtime.Scriptable;
using Project.Runtime.Scriptable.Buildings;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Equipment
{
    public class ItemInfoPopup : PanelBase
    {
        [SerializeField] private SelectSlotToEquipPopup selectSlotToEquipPopup;
        [Title("Content")] 
        [SerializeField] private Image rarityImage;
        [SerializeField] private TextMeshProUGUI rarityText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image iconImage;
        [Space]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider amountSlider;
        [Space]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [Space]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [Space]
        [SerializeField] private TextMeshProUGUI upgrade0Level;
        [SerializeField] private TextMeshProUGUI upgrade0Description;
        [SerializeField] private TextMeshProUGUI upgrade1Level;
        [SerializeField] private TextMeshProUGUI upgrade1Description;
        [SerializeField] private TextMeshProUGUI upgrade2Level;
        [SerializeField] private TextMeshProUGUI upgrade2Description;
        [Title("Buttons")]
        [SerializeField] private Button equipButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button blackoutCloseButton;

        private void Start()
        {
            Hide();
            
            equipButton.onClick.AddListener(OnClickEquip);
            closeButton.onClick.AddListener(Hide);
            blackoutCloseButton.onClick.AddListener(Hide);
        }

        public void SetDeckCard(DeckCard deckCard)
        {
            var cardConfig = deckCard.CardConfig;
            var buildingConfig = cardConfig.GetBuildingConfigFromPerks();
            var rarityColor = RarityColors.GetColorByRarity(Rarity.Common);
            
            rarityImage.color = rarityColor;
            rarityText.color = rarityColor;
            rarityText.text = "Common";
            titleText.SetText(buildingConfig.Title);
            iconImage.sprite = cardConfig.Icon;
            iconImage.color = deckCard.CardSaveData.isOpen ? Color.white : new Color(0, 0, 0, 1f);
        
            levelText.SetText($"{deckCard.CardSaveData.level + 1}");
            amountSlider.value = 0f; // todo put percent of: amount / need to level

            if (buildingConfig is UpgradableTowerConfig upgradableTowerConfig)
            {
                healthText.SetText($"Здоровье {upgradableTowerConfig.Health.min} - {upgradableTowerConfig.Health.max}");
            }
             
            if (buildingConfig is AttackTowerBuildingConfig attackTowerConfig)
            {
                damageText.SetText($"Урон {attackTowerConfig.Damage.min} - {attackTowerConfig.Damage.max}");
                attackSpeedText.SetText($"Скорость атаки {attackTowerConfig.AttackCooldown.min} - {attackTowerConfig.AttackCooldown.max}");
                attackRangeText.SetText($"Радиус атаки {attackTowerConfig.AttackRange.min} - {attackTowerConfig.AttackRange.max}");
            }
        
            descriptionText.SetText("<todo put text in ItemInfoPopup.cs>");
        
            upgrade0Level.SetText("0");
            upgrade0Description.SetText("<todo put text in ItemInfoPopup.cs>");;
            upgrade1Level.SetText("0");;
            upgrade1Description.SetText("<todo put text in ItemInfoPopup.cs>");;
            upgrade2Level.SetText("0");;
            upgrade2Description.SetText("<todo put text in ItemInfoPopup.cs>");;
        }

        private void OnClickEquip()
        {
            selectSlotToEquipPopup.Show();
        }
    }
}