using NTC.Pool;
using Runtime.Features.Sound;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class ProjectileTrailView : EntityView
    {
        [SerializeField] private TrailRenderer[] trailRenderers;
        [SerializeField] private ParticleSystem particle;

        private void OnValidate()
        {
            trailRenderers = GetComponentsInChildren<TrailRenderer>();
            particle = GetComponentInChildren<ParticleSystem>();
        }

        private void Awake()
        {
            ClearParticles();
        }

        protected override void OnSetEntity()
        {
            ClearParticles();
        }

        private void OnEnable()
        {
            ClearParticles();
        }

        private void OnDisable()
        {
            ClearParticles();
        }

        private void ClearParticles()
        {
            if (particle)
            {
                particle.Simulate(0.0f, true, true);
                particle.Play();
            }

            if (trailRenderers != null)
            {
                foreach (var trailRenderer in trailRenderers)
                {
                    trailRenderer.Clear();
                }
            }
        }
    }
}