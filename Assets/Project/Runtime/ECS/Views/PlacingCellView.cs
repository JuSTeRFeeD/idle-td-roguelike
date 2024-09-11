using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.ECS.Views
{
    public class PlacingCellView : EntityView
    {
        [SerializeField] private Image cellImage;
        [SerializeField] private Color collidedColor;
        [SerializeField] private Color mergeColor;
        [SerializeField] private Color baseColor;

        public void SetSize(int size)
        {
            transform.localScale = new Vector3(
                GridUtils.CellSize * 0.01f * size, 
                GridUtils.CellSize * 0.01f * size, 
                1f);
        }

        private void SetCollided(bool collision, bool mergeCollision)
        {
            if (collision)
            {
                cellImage.color = mergeCollision ? mergeColor : collidedColor;
            }
            else
            {
                cellImage.color = baseColor;
            }
        }

        private void Update()
        {
            ref readonly var owner = ref Entity.Owner();
            var targetPos = owner.ViewTransform().position;
            targetPos.y = 0.1f;
            transform.position = targetPos;
            ref readonly var placing = ref owner.GetComponent<PlacingBuildingCard>();
            SetCollided(placing.IsCollisionDetected, placing.IsMergeCollisionDetected);
        }
    }
}