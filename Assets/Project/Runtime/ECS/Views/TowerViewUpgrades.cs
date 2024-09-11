using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class TowerViewUpgrades : MonoBehaviour
    {
        [SerializeField] private List<GameObject> views = new();

        public void SetLevel(int level)
        {
            HideAll();
            if (level < 0)
            {
                views[0].SetActive(true);
                return;
            }
            if (level >= views.Count)
            {
               views[^1].SetActive(true);
               return;
            }
            views[level].SetActive(true);
        }

        private void HideAll()
        {
            foreach (var view in views)
            {
                view.SetActive(false);
            }
        }
        
#if UNITY_EDITOR
        [Button]
        private void One() => SetLevel(0);
        [Button]
        private void Two() => SetLevel(1);
        [Button]
        private void Three() => SetLevel(2);
        [Button]
        private void Four() => SetLevel(3);
        [Button]
        private void Five() => SetLevel(4);
        [Button]
        private void Six() => SetLevel(5);
#endif
    }
}