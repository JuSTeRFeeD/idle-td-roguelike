using DG.Tweening;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using Project.Runtime.Scriptable;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Equipment
{
    public class ItemInfoPopup : PanelBase
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;
        [Inject] private PlayerDeck _playerDeck;
        
        [SerializeField] private CurrencyConfig softCurrency;
        [SerializeField] private CurrencyConfig hexCurrency;
        [SerializeField] private SelectSlotToEquipPopup selectSlotToEquipPopup;
        
        [Title("Common")] 
        [SerializeField] private Image rarityImage;
        [SerializeField] private TextMeshProUGUI rarityText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [Title("Level")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider amountSlider;
        [SerializeField] private TextMeshProUGUI amountText;
        [Title("Upgrade")]
        [SerializeField] private RectTransform upgradeFrame;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI softCurrencyUpgradeCostText;
        [SerializeField] private TextMeshProUGUI hexCurrencyUpgradeCostText;
        [SerializeField] private AudioClip upgradeSound;
        [SerializeField] private AudioSource audioSource;
        [Title("Stats")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [Title("Upgrades")]
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

        private DeckCard _deckCard;
        
        private void Start()
        {
            Hide();
            
            equipButton.onClick.AddListener(OnClickEquip);
            closeButton.onClick.AddListener(Hide);
            blackoutCloseButton.onClick.AddListener(Hide);
            upgradeButton.onClick.AddListener(Upgrade);

            _playerDeck.OnChangeEquipment += Hide;
        }

        private void OnDestroy()
        {
            _playerDeck.OnChangeEquipment -= Hide;
        }

        private void Upgrade()
        {
            var amountToUpgrade = UpgradeConstants.GetCardAmountToUpgrade(_deckCard);
            var softCurrencyCost = UpgradeConstants.GetUpgradeCostSoftCurrency(_deckCard);
            var hexCurrencyCost = UpgradeConstants.GetUpgradeCostHexCurrency(_deckCard);
            if (_deckCard.CardSaveData.amount >= amountToUpgrade &&
                _persistentPlayerData.WalletByCurrency[softCurrency].Take((ulong)softCurrencyCost) &&
                _persistentPlayerData.WalletByCurrency[hexCurrency].Take((ulong)hexCurrencyCost)
            ) {
                _deckCard.CardSaveData.level++;
                _deckCard.CardSaveData.amount -= amountToUpgrade;
                SetDeckCard(_deckCard);
                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.UpgradedTowers);
                _saveManager.Save();
                
                levelText.transform.DOKill(true);
                levelText.transform.DOPunchScale(Vector3.one * 1.2f, 1f, 1).SetLink(levelText.gameObject);

                audioSource.PlayOneShot(upgradeSound, 0.7f);
            }
        }

        public void SetDeckCard(DeckCard deckCard)
        {
            _deckCard = deckCard;
            
            var cardConfig = deckCard.CardConfig;
            var buildingConfig = cardConfig.GetBuildingConfigFromPerks();
            var rarityColor = RarityExt.GetColorByRarity(cardConfig.Rarity);
            
            rarityImage.color = rarityColor;
            rarityText.color = rarityColor;
            rarityText.text = RarityExt.GetRarityName(cardConfig.Rarity);
            titleText.SetText(buildingConfig.Title);
            iconImage.sprite = cardConfig.Icon;
            iconImage.color = deckCard.CardSaveData.isOpen ? Color.white : new Color(0, 0, 0, 1f);
            descriptionText.SetText(buildingConfig.Description);
            
            // Level & Upgrade & Level
            levelText.SetText($"{deckCard.CardSaveData.level + 1}");
            
            InitUpgrade(deckCard);

            // Stats
            healthText.SetText("Здоровье -");
            damageText.SetText("Урон -");
            attackSpeedText.SetText("Скорость атаки -");
            attackRangeText.SetText("Радиус атаки -");
            if (buildingConfig is UpgradableTowerConfig upgradableTowerConfig)
            {
                healthText.SetText($"Здоровье {upgradableTowerConfig.Health.min} - {upgradableTowerConfig.Health.max}");
            }
            if (buildingConfig is AttackTowerBuildingConfig attackTowerConfig)
            {
                damageText.SetText($"Урон {attackTowerConfig.Damage.min} - {attackTowerConfig.Damage.max}");
                attackSpeedText.SetText($"Атак в сек. {1f / attackTowerConfig.AttackCooldown.min:#0.#} - {1f / attackTowerConfig.AttackCooldown.max:#0.#}");
                attackRangeText.SetText($"Радиус атаки {attackTowerConfig.AttackRange.min} - {attackTowerConfig.AttackRange.max}");
            }
            
            // Upgrades
            upgrade0Level.SetText("0");
            upgrade0Description.SetText("<todo put text in ItemInfoPopup.cs>");
            upgrade1Level.SetText("0");
            upgrade1Description.SetText("<todo put text in ItemInfoPopup.cs>");
            upgrade2Level.SetText("0");
            upgrade2Description.SetText("<todo put text in ItemInfoPopup.cs>");

            equipButton.interactable = deckCard.CardSaveData.isOpen && deckCard.CardSaveData.equippedAtSlot < 0;
        }

        private void InitUpgrade(DeckCard deckCard)
        {
            var upgradeSoftCurrencyCost = UpgradeConstants.GetUpgradeCostSoftCurrency(_deckCard);
            var upgradeHexCurrencyCost = UpgradeConstants.GetUpgradeCostHexCurrency(_deckCard);
            
            var amountToUpgrade = UpgradeConstants.GetCardAmountToUpgrade(deckCard);
            amountSlider.value = (float)deckCard.CardSaveData.amount / amountToUpgrade;
            amountText.SetText($"{deckCard.CardSaveData.amount}<size=80%>/{amountToUpgrade}");
            upgradeFrame.gameObject.SetActive(deckCard.CardSaveData.amount >= amountToUpgrade);
            
            upgradeButton.gameObject.SetActive(deckCard.CardSaveData.amount >= amountToUpgrade);
            
            softCurrencyUpgradeCostText.SetText($"{upgradeSoftCurrencyCost}");
            hexCurrencyUpgradeCostText.SetText($"{upgradeHexCurrencyCost}");
            upgradeButton.interactable = _persistentPlayerData.WalletByCurrency[softCurrency].Has((ulong)upgradeSoftCurrencyCost) && 
                                         _persistentPlayerData.WalletByCurrency[hexCurrency].Has((ulong)upgradeHexCurrencyCost);
        }

        private void OnClickEquip()
        {
            selectSlotToEquipPopup.Show(_deckCard);
        }
    }
}