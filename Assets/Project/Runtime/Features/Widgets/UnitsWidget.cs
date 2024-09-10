using System;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class UnitsWidget : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private UnitIconWidget[] unitIconWidget;
        [Space]
        [SerializeField] private Button addUnitButton;
        [SerializeField] private Button removeUnitButton;
        [Space]
        [SerializeField] private Sprite lumberjackIcon;
        [SerializeField] private Sprite minerIcon;

        public event Action OnAddUnitClick;
        public event Action OnRemoveUnitClick;

        private void Start()
        {
            addUnitButton.onClick.AddListener(AddUnitClick);
            removeUnitButton.onClick.AddListener(RemoveUnitClick);
        }

        private void OnDestroy()
        {
            addUnitButton.onClick.RemoveListener(AddUnitClick);
            addUnitButton.onClick.RemoveListener(AddUnitClick);
        }

        private void AddUnitClick() => OnAddUnitClick?.Invoke();
        private void RemoveUnitClick() => OnRemoveUnitClick?.Invoke();
        
        // public void Setup(UnitType unitType)
        // {
        //     titleText.SetText(unitType.ToString());
        //     
        //     switch (unitType)
        //     {
        //         case UnitType.Lumberjack:
        //             iconImage.sprite = lumberjackIcon;
        //             break;
        //         case UnitType.Miner:
        //             iconImage.sprite = minerIcon;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
        //     }
        //
        //     // just clear views
        //     SetUnits(0, 5, 5);
        // }

        public void SetUnits(int usedUnits, int currentCapacity, int maxCapacity)
        {
            for (var i = 0; i < unitIconWidget.Length; i++)
            {
                var iconWidget = unitIconWidget[i];

                if (i >= maxCapacity)
                {
                    iconWidget.SetType(UnitIconWidget.UnitWidgetType.Hidden);
                }
                else if (i >= currentCapacity && currentCapacity < maxCapacity)
                {
                    iconWidget.SetType(UnitIconWidget.UnitWidgetType.Locked);
                }
                else if (i < usedUnits)
                {
                    iconWidget.SetType(UnitIconWidget.UnitWidgetType.Filled);
                } 
                else
                {
                    iconWidget.SetType(UnitIconWidget.UnitWidgetType.Unfilled);
                }
            }
        }
    }
}