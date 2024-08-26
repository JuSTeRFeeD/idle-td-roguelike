using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class EntityView : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        public void SetEntity(Entity e)
        {
            Entity = e;   
        }
    }
}