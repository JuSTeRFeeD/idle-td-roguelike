using UnityEngine;

namespace Project.Runtime.Features.GameplayMenus
{
    public class PanelBase : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        
        public virtual void Show()
        {
            canvas.enabled = true;
        }
        
        public virtual void Hide()
        {
            canvas.enabled = false;
        }
    }
}