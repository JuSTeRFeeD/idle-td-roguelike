using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class ProjectileTrailView : EntityView
    {
        [SerializeField] private TrailRenderer trailRenderer;

        protected override void OnSetEntity()
        {
            trailRenderer.Clear();
        }
    }
}