using System.Runtime.CompilerServices;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Extensions
{
    public static class SystemGroupExtension
    {
        private static IObjectResolver _container;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetResolver(IObjectResolver container)
        {
            _container = container;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInitializer<TInitializer>(this SystemsGroup systemsGroup)
            where TInitializer : class, IInitializer, new()
        {
            var system = new TInitializer();
            _container.Inject(system);
            systemsGroup.AddInitializer(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSystem<TSystem>(this SystemsGroup systemsGroup, bool enabled = true)
            where TSystem : class, ISystem, new()
        {
            var system = new TSystem();
            _container.Inject(system);
            systemsGroup.AddSystem(system, enabled);
        }
    }
}