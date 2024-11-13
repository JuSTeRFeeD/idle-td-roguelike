using System.Collections;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Player
{
    public class PlaytimeStatistic : MonoBehaviour
    { 
        [Inject] private PersistentPlayerData _persistentPlayerData;

        private void Start()
        {
            StartCoroutine(AddPlayTime());
        }

        private IEnumerator AddPlayTime()
        {
            var wait = new WaitForSeconds(1f);
            while (true)
            {
                yield return wait;
                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.PlayedTimeSeconds, 1);
            }
        }
    }
}