using System;
using Project.Runtime.Features.GameplayMenus;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Building
{
    public class PlaceBuildingPanel : PanelBase
    {
        [SerializeField] private Button placeBuildingButton;
        [SerializeField] private Button cancelBuildingButton;

        public event Action OnPlaceBuilding;
        public event Action OnCancelBuilding;
        
        private void Start()
        {
            placeBuildingButton.onClick.AddListener(PlaceBuilding);
            cancelBuildingButton.onClick.AddListener(CancelBuilding);
        }

        private void PlaceBuilding()
        {
            OnPlaceBuilding?.Invoke();
        }

        private void CancelBuilding()
        {
            OnCancelBuilding?.Invoke();
        }
    }
}