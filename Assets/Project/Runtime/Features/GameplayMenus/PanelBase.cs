using UnityEngine;

namespace Project.Runtime.Features.GameplayMenus
{
    public class PanelBase : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        
        public bool IsPanelActive { get; private set; }
        
        public virtual void Show()
        {
            IsPanelActive = true;
            canvas.enabled = true;
        }
        
        public virtual void Hide()
        {
            IsPanelActive = false;
            canvas.enabled = false;
        }
    }
}