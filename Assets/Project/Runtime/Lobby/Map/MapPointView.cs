using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Map
{
    public class MapPointView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image pointImage;
        [SerializeField] private Sprite completedPointSprite;
        [SerializeField] private Sprite selectedPointSprite;
        [SerializeField] private Sprite defaultPointSprite;


        public void OnPointerClick(PointerEventData eventData)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    pointImage.sprite = defaultPointSprite;
                    break;
                case 1:
                    pointImage.sprite = selectedPointSprite;
                    break;
                case 2:
                    pointImage.sprite = completedPointSprite;
                    break;
            }

        }
    }
}