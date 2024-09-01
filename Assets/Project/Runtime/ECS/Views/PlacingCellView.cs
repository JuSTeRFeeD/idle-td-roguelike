using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.ECS.Views
{
    public class PlacingCellView : EntityView
    {
        [SerializeField] private Image cellImage;
        [SerializeField] private Color collidedColor;
        [SerializeField] private Color baseColor;

        public void SetSize(Vector2Int size)
        {
            transform.localScale = new Vector3(0.01f * size.x, 0.01f * size.y, 1f);
        }
        
        public void SetCollided(bool value)
        {
            cellImage.color = value ? collidedColor : baseColor;
        }

        private void Update()
        {
            ref readonly var owner = ref Entity.Owner();
            var targetPos = owner.ViewTransform().position;
            targetPos.y = 0.1f;
            transform.position = targetPos;
            SetCollided(owner.GetComponent<PlacingBuildingCard>().IsCollisionDetected);
        }
    }
}