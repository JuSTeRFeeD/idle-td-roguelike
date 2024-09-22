using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class BombTowerView : AttackTowerView
    {
        [SerializeField] private Material basicMaterial;
        [SerializeField] private Material destroyedMaterial;
        [SerializeField] private MeshRenderer[] renderers;

        private void Update()
        {
            foreach (var meshRenderer in renderers)
            {
                meshRenderer.material = Entity.Has<DestroyedTag>() ? destroyedMaterial : basicMaterial;
            }
        }
    }
}
