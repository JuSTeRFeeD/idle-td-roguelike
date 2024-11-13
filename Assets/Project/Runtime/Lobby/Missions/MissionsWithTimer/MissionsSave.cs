using System;
using System.Linq;

namespace Project.Runtime.Lobby.Missions.MissionsWithTimer
{
    [Serializable]
    public class MissionsSave
    {
        public long startTime;
        public string[] missionIds;
        public long[] valueAtStart;
        public bool[] completed;
        public bool[] rewarded;
        
        public void SortByCompletionAndRewardStatus()
        {
            var indices = Enumerable.Range(0, missionIds.Length).ToArray();
            Array.Sort(indices, (i1, i2) =>
            {
                var completed1 = completed[i1];
                var rewarded1 = rewarded[i1];

                var completed2 = completed[i2];
                var rewarded2 = rewarded[i2];

                // 1. completed == true && rewarded == false
                // 2. completed == false && rewarded == false
                // 3. completed == true && rewarded == true
                if (completed1 && rewarded1 == false && (completed2 != true || rewarded2))
                    return -1;
                if (completed2 && rewarded2 == false && (completed1 != true || rewarded1))
                    return 1;
                if (completed1 == false && rewarded1 == false && (completed2 || rewarded2))
                    return -1;
                if (completed2 == false && rewarded2 == false && (completed1 || rewarded1))
                    return 1;
                if (completed1 && rewarded1 && completed2 != true)
                    return 1;
                if (completed2 && rewarded2 && completed1 != true)
                    return -1;

                return 0;
            });

            // Применяем сортировку к каждому массиву на основе отсортированных индексов
            missionIds = indices.Select(i => missionIds[i]).ToArray();
            valueAtStart = indices.Select(i => valueAtStart[i]).ToArray();
            completed = indices.Select(i => completed[i]).ToArray();
            rewarded = indices.Select(i => rewarded[i]).ToArray();
        }
    }
}