using Project.Runtime.ECS.Views;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/Environment Building")]
    public class BuildingConfig : UniqueConfig
    {
        [Title("Building")]
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private EntityView prefab;
        [MinValue(1)]
        [SerializeField] private int size = 1;
        
        public string Title => title;
        public string Description => description;
        public EntityView Prefab => prefab;
        public int Size => size;
    }
}