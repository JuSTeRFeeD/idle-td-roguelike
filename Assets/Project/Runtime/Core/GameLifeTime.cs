using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.SaveLoad;
using Project.SaveLoad.SaveManagers;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameLifeTime : ExtendedLifetime
    {
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [Space]
        [SerializeField] private ActiveCardsListConfig commonCardsList;
        [SerializeField] private ActiveCardsListConfig devPlayerSelectedCards;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.Register<PersistentPlayerData>(Lifetime.Singleton);
            builder.RegisterInstance(new YandexSaveManager()).As<ISaveManager>();

            builder.Register<BuildingsDatabase>(Lifetime.Singleton);
            builder.Register<CardsDatabase>(Lifetime.Singleton);

            var playerDeck = new PlayerDeck(commonCardsList, devPlayerSelectedCards);
            builder.RegisterInstance<PlayerDeck>(playerDeck);
        
            builder.RegisterInstance(new SceneLoader(this, loadingCanvasGroup));
        }
    }
}
