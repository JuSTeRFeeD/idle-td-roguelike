using System.Collections.Generic;
using Project.Runtime.Lobby.Map;
using Project.Runtime.Scriptable.Enemies;

namespace Project.Runtime.ECS
{
    public class SceneSharedData
    {
        public NightWavesConfig NightWavesConfig;
        public List<MapPoint> MapPoints = new();
    }
}