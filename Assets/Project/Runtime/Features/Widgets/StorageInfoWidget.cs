using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Features.Widgets
{
    public class StorageInfoWidget : MonoBehaviour
    {
        [SerializeField] private AmountWidget woodAmountWidget;
        [SerializeField] private AmountWidget stoneAmountWidget;

        private Entity _entity;
        
        public void AddStorageType(ResourceType resourceType, in Entity buildingEntity)
        {
            _entity = buildingEntity;
            
            switch (resourceType)
            {
                case ResourceType.Wood:
                    woodAmountWidget.gameObject.SetActive(true);
                    break;
                case ResourceType.Stone:
                    stoneAmountWidget.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private void Update()
        {
            if (_entity.IsNullOrDisposed()) return;
            
            if (woodAmountWidget)
            {
                woodAmountWidget.SetText(_entity.GetComponent<WoodStorage>());
            }

            if (stoneAmountWidget)
            {
                stoneAmountWidget.SetText(_entity.GetComponent<StoneStorage>());
            }
        }
    }
}