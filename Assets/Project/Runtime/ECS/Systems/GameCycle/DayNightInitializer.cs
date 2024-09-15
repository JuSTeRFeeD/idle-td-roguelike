using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class DayNightInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        public void OnAwake()
        {
            var entity = World.CreateEntity();
            entity.SetComponent(new DayNight
            {
                DayTime = _worldSetup.DayNightConfig.DayTime,
                NightTime = _worldSetup.DayNightConfig.NightTime,
                DayNumber = 1,
                EstimateTime = _worldSetup.DayNightConfig.DayTime
            });
            entity.SetComponent(new IsDayTimeTag());
        }

        public void Dispose()
        {
        }
    }
}