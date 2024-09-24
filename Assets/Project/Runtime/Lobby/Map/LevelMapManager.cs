using Project.SaveLoad;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby.Map
{
    public class LevelMapManager : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        [SerializeField] private LevelMapGenerator levelMapGenerator;

        private void Start()
        {
            InitMap();
        }

        private void InitMap()
        {
            if (!string.IsNullOrEmpty(_persistentPlayerData.MapData))
            {
                levelMapGenerator.LoadMap(_persistentPlayerData.MapData);
                levelMapGenerator.Path[_persistentPlayerData.CurMapPointIndex].MapPointView.SetSelected(true);
                return;
            }
            GenerateNewMap();
        }

        private void GenerateNewMap()
        {
            levelMapGenerator.GenerateMap();
            levelMapGenerator.Path[0].MapPointView.SetSelected(true);
        }
    }
}