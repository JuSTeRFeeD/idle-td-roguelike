using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/Resource")]
    public class MapResourceConfig : BuildingConfig
    {
        public ResourceType resourceType;
        public float health = 5;
    }

    public enum ResourceType
    {
        Wood,
        Stone
    }
}