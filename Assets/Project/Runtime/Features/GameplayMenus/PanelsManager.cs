using System;
using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Scriptable.Buildings;
using UnityEngine;

namespace Project.Runtime.Features.GameplayMenus
{
    public class PanelsManager : MonoBehaviour
    {
        [Serializable]
        public struct PanelSetup
        {
            public PanelType panelType;
            public PanelBase panel;
        }

        [SerializeField] public List<PanelSetup> panels = new();
        private readonly Dictionary<PanelType, PanelSetup> panelsByType = new();

        public PanelType ActivePanel { get; private set; }
        public event Action<PanelType> OnChangePanel;
        
        private void Start()
        {
            foreach (var panelSetup in panels)
            {
                panelsByType.Add(panelSetup.panelType, panelSetup);
                panelSetup.panel.Hide();
            }
        }

        public void SetPanel(PanelType panelType)
        {
            if (panelType == ActivePanel) return;
            
            Debug.Log($"[PanelsManager] SetPanel {panelType} | CurrentPanel {ActivePanel}");
            if (panelsByType.TryGetValue(ActivePanel, out var oldPanel))
            {
                oldPanel.panel.Hide();
            }
            
            ActivePanel = panelType;
            
            if (panelsByType.TryGetValue(ActivePanel, out var newPanel))
            {
                newPanel.panel.Show();
            }
            
            OnChangePanel?.Invoke(ActivePanel);
        }
    }

    public enum PanelType
    {
        None = 0,
        TowerManagement = 1,
    }
}