using NTC.Pool;
using UnityEngine;

namespace Project.Runtime.ECS
{
    public static class VfxPool
    {
        public static void Spawn(ParticleSystem particleSystem, Vector3 position)
        {
            var vfx = NightPool.Spawn(particleSystem, position, Quaternion.identity);
            NightPool.Despawn(vfx.gameObject, particleSystem.main.duration);
        }
    }
}