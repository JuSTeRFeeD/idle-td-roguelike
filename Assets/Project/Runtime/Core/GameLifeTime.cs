using Project.Runtime.ECS;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.MapLevelConfigs;
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
        
        [Title("Cards list configs1")]
        [SerializeField] private ActiveCardsListConfig commonCardsList;
        [SerializeField] private ActiveCardsListConfig firstTimeCardsList;

        [Title("Global setups")] 
        [SerializeField] private GlobalDifficultSettingsConfig globalDifficultSettingsConfig;
        [SerializeField] private MapLevelConfig globalMapLevelConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.RegisterInstance(coroutineRunner).As<ICoroutineRunner>();

            var playerData = new PersistentPlayerData();
            builder.RegisterInstance<PersistentPlayerData>(playerData);
            builder.Register<YandexSaveManager>(Lifetime.Singleton).As<ISaveManager>();

            builder.Register<BuildingsDatabase>(Lifetime.Singleton);
            builder.Register<CardsDatabase>(Lifetime.Singleton);

            builder.RegisterInstance<PlayerDeck>(new PlayerDeck(playerData, commonCardsList, firstTimeCardsList));
        
            builder.RegisterInstance(new SceneLoader(this, loadingCanvasGroup));

            builder.RegisterInstance<GlobalDifficultSettingsConfig>(globalDifficultSettingsConfig);
            builder.RegisterInstance<MapLevelConfig>(globalMapLevelConfig);
            builder.Register<SceneSharedData>(Lifetime.Singleton);
        }
    }
}
