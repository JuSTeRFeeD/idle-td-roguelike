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

            SetPanel(LobbyPanelType.Map);
        }

        public void SetPanel(LobbyPanelType panelType)
        {
            if (_panelsByType.TryGetValue(_currentPanel, out var value))
            {
                value.panel.Hide();
            }
            
            _currentPanel = panelType;
            
            _panelsByType[_currentPanel].panel.Show();
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