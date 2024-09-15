using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class RadiusView : EntityView
    {
        [SerializeField] private Transform canvasTransform;
        
        private const float OneRad = 0.02f;

        public void SetRadius(float radius)
        {
            canvasTransform.localScale = Vector3.one * (OneRad * radius);
        }

        private void Update()
        {
            var owner = Entity.Owner();
            var ownerPos = owner.ViewPosition();
            ownerPos.y = 0.1f;
            transform.position = ownerPos;

            if (owner.Has<AttackRangeRuntime>())
            {
                SetRadius(owner.GetComponent<AttackRangeRuntime>().Value);
            }
        }
    }
}