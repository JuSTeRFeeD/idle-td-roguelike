using System;
using System.Collections.Generic;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Services.PlayerProgress;
using Scellecs.Morpeh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Features.BuildingsManagement
{
    public class BuildingManagementPanel : PanelBase
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private PanelsManager _panelsManager;
        
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI panelTitleText;
        [SerializeField] private Image upgadeProgressBg;
        [SerializeField] private Image upgadeProgress;

        [Header("Auto upgrade")] 
        [SerializeField] private Toggle autoUpgradeToggle;
        
        [Header("Widgets")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform container;
        [SerializeField] private UnitsWidget unitsWidgetPrefab;
        [SerializeField] private StorageInfoWidget storageInfoWidgetPrefab;
        [SerializeField] private TowerWidget towerWidgetPrefab;
        [SerializeField] private StatsWidget statsWidgetPrefab;

        private StorageInfoWidget _storageInfoWidget;
        private UnitsWidget _unitsWidget;
        private TowerWidget _towerWidget;
        private readonly StatsWidget[] _statsWidgets = new StatsWidget[2];

        public event Action OnCloseClick;
        
        private void Start()
        {
            closeButton.onClick.AddListener(ClosePanel);
            _panelsManager.OnChangePanel += OnChangePanel;

            autoUpgradeToggle.isOn = _persistentPlayerData.AutoUpgradeTowersChecked;
            autoUpgradeToggle.onValueChanged.AddListener(OnAutoUpgradeToggleChanged);
        }

        private void OnAutoUpgradeToggleChanged(bool value)
        {
            _persistentPlayerData.AutoUpgradeTowersChecked = value;
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(ClosePanel);
            _panelsManager.OnChangePanel -= OnChangePanel;
        }

        private void OnChangePanel(PanelType panelType)
        {
            if (panelType == PanelType.TowerManagement) return;
            ClosePanel();
        }

        private void ClosePanel()
        {
            OnCloseClick?.Invoke();
        }

        public override void Hide()
        {
            Debug.Log("Clear widgets");
            foreach (Transform widgetTransform in container)
            {
                Destroy(widgetTransform.gameObject);
            }
            _unitsWidget = null;
            _storageInfoWidget = null;
            _statsWidgets[0] = null;
            _statsWidgets[1] = null;
            
            base.Hide();
        }

        public void SetTitleAndLevel(string title, int level, float upgradeProgress)
        {
            if (!_towerWidget) return;
            
            if (upgradeProgress >= 1f)
            {
                panelTitleText.SetText($"{title} <size=80%><color=yellow>ур.{level + 1} <color=orange>МАКС.");    
            }
            else
            {
                panelTitleText.SetText($"{title} <size=80%><color=yellow>ур.{level + 1}");
            }
            
            upgadeProgress.fillAmount = upgradeProgress;
            upgadeProgress.enabled = true;
            upgadeProgressBg.enabled = true;
        }
        
        public void SetTitle(string title)
        {
            panelTitleText.SetText($"{title}");
            upgadeProgress.enabled = false;
            upgadeProgressBg.enabled = false;
        }
        
        public void AddStorageInfoWidget(ResourceType resourceType, in Entity entity)
        {
            if (!_storageInfoWidget)
            {
                _storageInfoWidget = Instantiate(storageInfoWidgetPrefab, container);
            }
            _storageInfoWidget.AddStorageType(resourceType, entity);
        }
        
        #region Upgrade Tower Widget
        
        public void AddTowerWidget(Action onClickUpgrade)
        {
            _towerWidget = Instantiate(towerWidgetPrefab, container);
            _towerWidget.OnClickUpgrade += onClickUpgrade;
        }
        public void SetUpgradeTowerWidgetPrices(int woodUpgradePrice, int stoneUpgradePrice)
        {
            _towerWidget.SetPrices(woodUpgradePrice, stoneUpgradePrice);
        }
        public void SetUpgradeTowerWidgetTotalResourcesAmount(int woodAmount, int stoneAmount)
        {
            if (!_towerWidget) return;
            _towerWidget.SetResourcesInStorages(woodAmount, stoneAmount);
        }
        public void DestroyUpgradeTowerWidget()
        {
            Destroy(_towerWidget.gameObject);
            _towerWidget = null;
        }
        
        #endregion

        #region Stats Widget

        public void AddStatsWidget(int statsWidgetNumber = 0)
        {
#if UNITY_EDITOR
            if (_statsWidgets[statsWidgetNumber]) Debug.LogError($"_statsWidget {statsWidgetNumber} ALREADY added to panel!");
#endif
            _statsWidgets[statsWidgetNumber] = Instantiate(statsWidgetPrefab, container);
        }

        public void SetStatsWidgetText(List<string> stats, int statsWidgetNumber = 0)
        {
#if UNITY_EDITOR
            if (!_statsWidgets[statsWidgetNumber]) Debug.LogError("_statsWidget doesn't added to panel!");
#endif
            _statsWidgets[statsWidgetNumber].SetStats(stats);
        }

        #endregion
        
        public void AddUnitManagementWidget()
        {
            _unitsWidget = Instantiate(unitsWidgetPrefab, container);
        }
        public void SetUnitsWidgetValues(int usedUnits, int currentCapacity, int maxCapacity)
        {
#if UNITY_EDITOR
            if (!_unitsWidget) Debug.LogError($"_unitsWidget doesn't added to panel!");
#endif
            _unitsWidget.SetUnits(usedUnits, currentCapacity, maxCapacity);
        }

        public void ResetScroll()
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}