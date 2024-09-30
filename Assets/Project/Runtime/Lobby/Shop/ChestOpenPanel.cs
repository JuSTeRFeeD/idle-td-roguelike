using System;
using Project.Runtime.Features.GameplayMenus;
using UnityEngine.EventSystems;

namespace Project.Runtime.Lobby.Shop
{
    public class ChestOpenPanel : PanelBase, IPointerClickHandler
    {
        public event Action OnClick; 
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}