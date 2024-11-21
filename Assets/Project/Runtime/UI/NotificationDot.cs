using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class NotificationDot : MonoBehaviour
    {
        [SerializeField] private Image dot;

        private void Awake()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            dot.enabled = active;
        }
    }
}
