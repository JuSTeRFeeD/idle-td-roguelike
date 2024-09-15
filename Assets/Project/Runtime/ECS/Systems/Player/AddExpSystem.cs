using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AddExpSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        
        public World World { get; set; }

        private Filter _addExpFilter;
        private Filter _playerLevelFilter;
        
        public void OnAwake()
        {
            _addExpFilter = World.Filter
                .With<PlayerAddExp>()
                .Build();

            _playerLevelFilter = World.Filter
                .With<PlayerLevel>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _addExpFilter)
            {
                foreach (var playerDataEntity in _playerLevelFilter)
                {
                    ref var data = ref playerDataEntity.GetComponent<PlayerLevel>();
                    data.CurrentExp += entity.GetComponent<PlayerAddExp>().Value;

                    while (data.CurrentExp >= data.TargetExp)
                    {
                        data.CurrentExp -= data.TargetExp;
                        data.Level++;
                        data.TargetExp = data.ExpByLevel[data.Level];

                        if (playerDataEntity.Has<LevelUp>())
                        {
                            playerDataEntity.SetComponent(new LevelUp
                            {
                                LevelUpsCount = playerDataEntity.GetComponent<LevelUp>().LevelUpsCount + 1
                            });
                        }
                        else
                        {
                            playerDataEntity.SetComponent(new LevelUp
                            {
                                LevelUpsCount = 1
                            });
                        }
                        
                        _headerUI.SetLevel(data.Level);
                    }
                    
                    _headerUI.SetLevelExp(data.CurrentExp, data.TargetExp);
                }  
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}