using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    public class TotalUnitsCountSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        
        public World World { get; set; }

        private Filter _dataFilter;
        private Filter _unitsFilter;
        private Filter _chillingUnitsFilter;
        
        public void OnAwake()
        {
            _dataFilter = World.Filter
                .With<TotalUnitsData>()
                .Build();
            
            _unitsFilter = World.Filter
                .With<UnitTag>()
                .Build();

            _chillingUnitsFilter = World.Filter
                .With<UnitTag>()
                .Without<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // TODO: производить пересчет при изменении кол-ва юнитов в мире
            
            foreach (var entity in _dataFilter)
            {
                ref var data = ref entity.GetComponent<TotalUnitsData>();
                
                var totalUnits = _unitsFilter.GetLengthSlow();
                var chillingUnits = _chillingUnitsFilter.GetLengthSlow();

                data.UsedUnitsAmount = totalUnits - chillingUnits;
                data.TotalUnitsAmount = totalUnits;
                
                _headerUI.SetUnitsAmount(data);
            }
        }

        public void Dispose()
        {
        }
    }
}