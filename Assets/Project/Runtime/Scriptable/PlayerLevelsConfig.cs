using UnityEngine;

namespace Project.Runtime.Scriptable
{
    [CreateAssetMenu(menuName = "Game/PlayerLevelsConfig")]
    public class PlayerLevelsConfig : ScriptableObject
    {
        [SerializeField] private int[] expByLevel;

        public int[] ExpByLevel => expByLevel;
    }
}