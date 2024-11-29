using System;
using System.Collections.Generic;
using Project.Runtime.Features.GameplayMenus;
using UnityEngine;

namespace Project.Runtime.Lobby
{
    public class LobbyPanelsManager : MonoBehaviour
    {
        [Serializable]
        public struct LobbyPanelSetup
        {
            public LobbyPanelType panelType;
            public PanelBase panel;
        }

        [SerializeField] private PanelBase navigationPanel;
        [SerializeField] private PanelBase headerPanel;
        [SerializeField] private Canvas headerCurrencyPanel;
        [Space]
        [SerializeField] private List<LobbyPanelSetup> panels;
        private readonly Dictionary<LobbyPanelType, LobbyPanelSetup> _panelsByType = new();

        private LobbyPanelType _currentPanel;
        
        private void Awake()
        {
            foreach (var lobbyPanelSetup in panels)
            {
                _panelsByType.Add(lobbyPanelSetup.panelType, lobbyPanelSetup);
                lobbyPanelSetup.panel.gameObject.SetActive(true);
                lobbyPanelSetup.panel.Hide();
            }

            navigationPanel.gameObject.SetActive(true);
            headerPanel.gameObject.SetActive(true);
            headerCurrencyPanel.gameObject.SetActive(true);
            
            SetPanel(LobbyPanelType.Map);
        }

        public void SetPanel(LobbyPanelType panelType)
        {
            if (_panelsByType.TryGetValue(_currentPanel, out var value))
            {
                value.panel.Hide();
            }
            
            _currentPanel = panelType;

            if (_panelsByType.TryGetValue(_currentPanel, out var newPanel))
            {
                newPanel.panel.Show();
            }
            
            if (panelType is LobbyPanelType.None)
            {
                navigationPanel.Hide();
                headerPanel.Hide();
                headerCurrencyPanel.enabled = false;
            }
            else
            {
                navigationPanel.Show();
                headerPanel.Show();
                headerCurrencyPanel.enabled = true;
            }
        }
    }

    public enum LobbyPanelType
    {
        None,
        Map,
        Cards,
        Shop,
        Challenge,
        MetaProgress,
    }
}