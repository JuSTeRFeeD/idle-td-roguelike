using UnityEngine;

namespace Project.Runtime.Features.Navigation
{
    public class NavPanel : MonoBehaviour
    {
        [SerializeField] private int selectOnStartItemIndex;
        [SerializeField] private NavItem[] navItems;

        protected int SelectedIndex { get; private set; }
        
        private void Start()
        {
            for (var index = 0; index < navItems.Length; index++)
            {
                var navItem = navItems[index];
                navItem.Setup(index);
                navItem.SetSelected(false);
                navItem.OnClick += OnClickNavItem;
            }

            SelectedIndex = selectOnStartItemIndex;
            navItems[SelectedIndex].SetSelected(true);
        }

        private void OnClickNavItem(int index)
        {
            navItems[SelectedIndex].SetSelected(false);
            SelectedIndex = index;
            Clicked();
            navItems[SelectedIndex].SetSelected(true);
        }

        protected virtual void Clicked() { }
    }
}