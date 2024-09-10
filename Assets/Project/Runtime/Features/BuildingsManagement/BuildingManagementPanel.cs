using System;
using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Features.BuildingsManagement
{
    public class BuildingManagementPanel : PanelBase
    {
        [Inject] private PanelsManager _panelsManager;
        
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI panelTitleText;
        [SerializeField] private Image upgadeProgressBg;
        [SerializeField] private Image upgadeProgress;
        [Header("Widgets")]
        [SerializeField] private Transform container;
        [SerializeField] private UnitsWidget unitsWidgetPrefab;
        [SerializeField] private StorageInfoWidget storageInfoWidgetPrefab;

        private StorageInfoWidget _storageInfoWidget;
        // private readonly Dictionary<UnitType, UnitsWidget> _unitsWidgetByType = new();

        public event Action OnCloseClick;
        
        private void Start()
        {
            closeButton.onClick.AddListener(ClosePanel);
            _panelsManager.OnChangePanel += OnChangePanel;
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
            // _unitsWidgetByType.Clear();
            _storageInfoWidget = null;
            base.Hide();
        }

        public void SetTitleAndLevel(string title, int level, float upgradeProgress)
        {
            panelTitleText.SetText($"{title} <size=80%><color=yellow>lv.{level}");
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
        
//         public void AddUnitManagementWidget(UnitType unitType, Action onRemoveUnitClick, Action onAddUnitClick)
//         {
//             var widget = Instantiate(unitsWidgetPrefab, container);
//             widget.Setup(unitType);
//             widget.OnAddUnitClick += onAddUnitClick;
//             widget.OnRemoveUnitClick += onRemoveUnitClick;
//             _unitsWidgetByType.Add(unitType, widget);
//         }
//
//         public void SetUnitsWidgetValues(UnitType unitType, int usedUnits, int currentCapacity, int maxCapacity)
//         {
// #if UNITY_EDITOR
//             if (!_unitsWidgetByType.ContainsKey(unitType)) Debug.LogError($"UnitType {unitType} doesn't added to panel!");
// #endif
//             _unitsWidgetByType[unitType].SetUnits(usedUnits, currentCapacity, maxCapacity);
//         }
    }
}