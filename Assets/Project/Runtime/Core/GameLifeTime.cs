using Ads;
using Project.Runtime.ECS;
using Project.Runtime.Lobby.Missions;
using Project.Runtime.Lobby.Missions.MissionsWithTimer;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
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

        [Title("Global setups")] 
        [SerializeField] private GlobalDifficultSettingsConfig globalDifficultSettingsConfig;
        [SerializeField] private MapLevelConfig globalMapLevelConfig;

        [Title("Currency")]
        [SerializeField] private CurrencyConfig[] gameCurrencies;
        
        [Title("LevelFinishedDrops")]
        [SerializeField] private DropChancesConfig winConfig;
        [SerializeField] private DropChancesConfig loseConfig;
        [SerializeField] private DropChancesConfig bonusWinConfig;
        
        [Title("Other")]
        [SerializeField] private SoundVolume soundVolume;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.RegisterInstance(coroutineRunner).As<ICoroutineRunner>();

            builder.RegisterInstance<SoundVolume>(soundVolume);
            
            builder.Register<PersistentPlayerData>(Lifetime.Singleton).WithParameter<CurrencyConfig[]>(gameCurrencies);
            builder.Register<YandexSaveManager>(Lifetime.Singleton).As<ISaveManager>();

            builder.Register<BuildingsDatabase>(Lifetime.Singleton);
            builder.Register<CardsDatabase>(Lifetime.Singleton);
            builder.Register<MissionsDatabase>(Lifetime.Singleton);

            builder.Register<PlayerDeck>(Lifetime.Singleton);

            builder.Register<ServerTime>(Lifetime.Singleton);
            builder.Register<MissionTimer>(Lifetime.Singleton);
            builder.Register<MissionsManager>(Lifetime.Singleton);
            
            builder.RegisterInstance(new SceneLoader(this, loadingCanvasGroup));

            builder.RegisterInstance<GlobalDifficultSettingsConfig>(globalDifficultSettingsConfig);
            builder.RegisterInstance<MapLevelConfig>(globalMapLevelConfig);
            
            builder.Register<SceneSharedData>(Lifetime.Singleton);
            builder.RegisterInstance<LevelFinishedDrops>(new LevelFinishedDrops(winConfig, loseConfig, bonusWinConfig));
        }
    }
}
