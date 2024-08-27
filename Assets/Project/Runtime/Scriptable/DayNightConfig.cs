using UnityEngine;

namespace Project.Runtime.Scriptable
{
    [CreateAssetMenu(menuName = "Game/DayNightConfig")]
    public class DayNightConfig : ScriptableObject
    {
        [SerializeField] private float dayTime;
        [SerializeField] private float nightTime;
        
        public float DayTime => dayTime;
        public float NightTime => nightTime;
    }
}