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

namespace Project.Runtime.Features.BuildingsManagement
{
    public class BuildingManagementPanel : PanelBase
    {
        [SerializeField] private TextMeshProUGUI panelTitleText;
        [SerializeField] private Image upgadeProgressBg;
        [SerializeField] private Image upgadeProgress;
        [Header("Widgets")]
        [SerializeField] private Transform container;
        [SerializeField] private UnitsWidget unitsWidgetPrefab;
        [SerializeField] private StorageInfoWidget storageInfoWidgetPrefab;

        private StorageInfoWidget _storageInfoWidget;
        private Dictionary<UnitType, UnitsWidget> _unitsWidgetByType = new();

        public override void Hide()
        {
            foreach (Transform widgetTransform in container)
            {
                Destroy(widgetTransform.gameObject);
            }

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
        
        public void AddUnitManagementWidget(UnitType unitType, Action onRemoveUnitClick, Action onAddUnitClick)
        {
            var widget = Instantiate(unitsWidgetPrefab, container);
            widget.Setup(unitType);
            widget.OnAddUnitClick += onAddUnitClick;
            widget.OnRemoveUnitClick += onRemoveUnitClick;
            _unitsWidgetByType.Add(unitType, widget);
        }

        public void SetUnitsWidgetValues(UnitType unitType, int usedUnits, int currentCapacity, int maxCapacity)
        {
            _unitsWidgetByType[unitType].SetUnits(usedUnits, currentCapacity, maxCapacity);
        }
    }
}