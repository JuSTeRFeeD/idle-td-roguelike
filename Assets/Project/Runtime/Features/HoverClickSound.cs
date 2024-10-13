using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Runtime.Features
{
    [RequireComponent(typeof(AudioSource))]
    public class HoverClickSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip pointerClick;
        [SerializeField] private AudioClip pointerEnter;
        
        private AudioSource _source;
        
        private void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (pointerClick) _source.PlayOneShot(pointerClick, 0.5f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (pointerEnter) _source.PlayOneShot(pointerEnter, 0.1f);
        }
    }
}