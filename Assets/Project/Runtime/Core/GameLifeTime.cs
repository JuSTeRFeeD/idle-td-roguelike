using Project.Runtime.ECS;
using Project.Runtime.Lobby.Missions;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.MapLevelConfigs;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Project.Runtime.Services.Saves.YandexSaves;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameLifeTime : ExtendedLifetime
    {
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [SerializeField] private CoroutineRunner coroutineRunner;
        
        [Title("Cards list configs")]
        [SerializeField] private ActiveCardsListConfig commonCardsList;
        [SerializeField] private ActiveCardsListConfig firstTimeCardsList;

        [Title("Global setups")] 
        [SerializeField] private GlobalDifficultSettingsConfig globalDifficultSettingsConfig;
        [SerializeField] private MapLevelConfig globalMapLevelConfig;

        [Title("Currency")]
        [SerializeField] private CurrencyConfig[] gameCurrencies;
        
        [Title("LevelFinishedDrops")]
        [SerializeField] private DropChancesConfig winConfig;
        [SerializeField] private DropChancesConfig loseConfig;
        [SerializeField] private DropChancesConfig bonusWinConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.RegisterInstance(coroutineRunner).As<ICoroutineRunner>();

            var playerData = new PersistentPlayerData(gameCurrencies);
            builder.RegisterInstance<PersistentPlayerData>(playerData);
            builder.Register<YandexSaveManager>(Lifetime.Singleton).As<ISaveManager>();

            builder.Register<BuildingsDatabase>(Lifetime.Singleton);
            builder.Register<CardsDatabase>(Lifetime.Singleton);
            builder.Register<MissionsDatabase>(Lifetime.Singleton);

            builder.RegisterInstance<PlayerDeck>(new PlayerDeck(playerData, commonCardsList, firstTimeCardsList));
        
            builder.RegisterInstance(new SceneLoader(this, loadingCanvasGroup));

            builder.RegisterInstance<GlobalDifficultSettingsConfig>(globalDifficultSettingsConfig);
            builder.RegisterInstance<MapLevelConfig>(globalMapLevelConfig);
            
            builder.Register<SceneSharedData>(Lifetime.Singleton);
            builder.RegisterInstance<LevelFinishedDrops>(new LevelFinishedDrops(winConfig, loseConfig, bonusWinConfig));
        }
    }
}
