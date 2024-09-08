using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.ECS.Views
{
    public class WorldHealthBarView : EntityView
    {
        [SerializeField] private Image fill;

        private void Update()
        {
            ref readonly var owner = ref Entity.Owner();

            transform.position = owner.ViewPosition();
                
            ref readonly var hp = ref owner.GetComponent<HealthCurrent>().Value;
            ref readonly var hpDefault = ref owner.GetComponent<HealthDefault>().Value;
            fill.fillAmount = hp / hpDefault;
        }
    }
}
