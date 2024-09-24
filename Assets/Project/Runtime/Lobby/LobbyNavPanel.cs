using Project.Runtime.Features.Navigation;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby
{
    public class LobbyNavPanel : NavPanel
    {
        [Inject] private LobbyPanelsManager _lobbyPanelsManager;
        
        [SerializeField] private LobbyPanelType[] openTabs;
        
        protected override void Clicked()
        {
            _lobbyPanelsManager.SetPanel(openTabs[SelectedIndex]);
        }
    }
}