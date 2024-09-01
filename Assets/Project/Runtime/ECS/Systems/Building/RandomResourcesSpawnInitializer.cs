using System.Collections.Generic;
using System.Linq;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Systems.Pathfinding;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    public class RandomResourcesSpawnInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        public void OnAwake()
        {
            
        }
        
        public void Dispose()
        {
        }
    }
}