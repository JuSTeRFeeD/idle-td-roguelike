using System;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.ECS.Views
{
    public class WorldProgressBarView : EntityView
    {
        [SerializeField] private Image fill;

        public void SetProgress(float percent)
        {
            fill.fillAmount = percent;
        }

        private void Update()
        {
            ref readonly var unit = ref Entity.GetComponent<Owner>().Entity;
            var currentTime = unit.GetComponent<Gathering>().CurrentTime;
            var time = unit.GetComponent<GatheringTime>().Time;
            SetProgress(currentTime / time);
        }
    }
}