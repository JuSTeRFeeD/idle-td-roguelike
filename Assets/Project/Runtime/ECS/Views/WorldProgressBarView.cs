using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.ECS.Views
{
    public class WorldProgressBarView : EntityView
    {
        [SerializeField] private Image fill;

        private void SetProgress(float percent)
        {
            fill.fillAmount = percent;
        }

        private void Update()
        {
            ref readonly var unit = ref Entity.Owner();
            if (unit.Has<Gathering>())
            {
                var currentTime = unit.GetComponent<Gathering>().CurrentTime;
                var time = unit.GetComponent<GatheringTime>().Time;
                SetProgress(currentTime / time);
            }
            if (unit.Has<UnitRepairingTower>())
            {
                ref readonly var repairingTower = ref unit.GetComponent<UnitRepairingTower>().Progress;
                SetProgress(repairingTower);
            }
        }
    }
}