using Project.Runtime.Lobby;
using Project.Runtime.Lobby.Shop;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class LobbyLifeTime : ExtendedLifetime
    {
        [SerializeField] private LobbyPanelsManager lobbyPanelsManager;
        [SerializeField] private ChestOpeningController chestOpeningController;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<LobbyPanelsManager>(lobbyPanelsManager);
            builder.RegisterInstance<ChestOpeningController>(chestOpeningController);
        }
    }
}