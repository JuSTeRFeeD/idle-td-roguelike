using UnityEngine;

namespace Project.Runtime.Lobby.Map
{
    public class LevelMapManager : MonoBehaviour
    {
        [SerializeField] private LevelMapGenerator levelMapGenerator;

        private void Start()
        {
            levelMapGenerator.GenerateMap();
        }
    }
}