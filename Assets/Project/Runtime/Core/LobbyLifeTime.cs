using Project.Runtime.Lobby;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class LobbyLifeTime : ExtendedLifetime
    {
        [SerializeField] private LobbyPanelsManager lobbyPanelsManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<LobbyPanelsManager>(lobbyPanelsManager);
        }
    }
}