using Project.Runtime.Scriptable.Enemies;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Components.Enemies
{
    public struct EnemyTag : IComponent
    {
    }

    public struct SpawnEnemyRequest : IComponent
    {
        public Vector3 Position;
        public EnemyConfig EnemyConfig;
    }
}