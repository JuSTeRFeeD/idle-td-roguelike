using Project.Runtime.Features.Databases;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Core
{
    public class GameLifeTime : ExtendedLifetime
    {
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.Register<BuildingsDatabase>(Lifetime.Singleton);
            builder.RegisterInstance(new SceneLoader(this, loadingCanvasGroup));
        }
    }
}
