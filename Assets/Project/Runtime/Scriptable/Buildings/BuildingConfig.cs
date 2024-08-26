using Project.Runtime.ECS.Views;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/Building")]
    public class BuildingConfig : UniqueConfig
    {
        [Title("Building")]
        [SerializeField] private string title;
        [SerializeField] private EntityView prefab;
        [SerializeField] private Vector2Int size = Vector2Int.one;
        

        public string Title => title;
        public EntityView Prefab => prefab;
        public Vector2Int Size => size;
    }
}