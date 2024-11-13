using System;
using System.Collections;
using System.Collections.Generic;
using NTC.Pool;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Lobby.Missions.MissionsWithTimer;
using Project.Runtime.Scriptable.Missions;
using Project.Runtime.Services.PlayerProgress;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionsPanel : PanelBase
    {
        [Inject] private ServerTime _serverTime;
        [Inject] private MissionTimer _missionTimer;
        [Inject] private MissionsManager _missionsManager;
        [Inject] private PersistentPlayerData _persistentPlayerData;

        [Title("Missions panel setup")]
        [SerializeField] private TextMeshProUGUI updateInText;
        [Space]
        [SerializeField] private Button dailyBtn;
        [SerializeField] private Button weeklyBtn;
        [Space]
        [SerializeField] private MissionItemView missionItemViewPrefab;
        [SerializeField] private RectTransform container;

        private readonly List<MissionItemView> _items = new();

        private TimedMissionsType _showMissionsType = TimedMissionsType.Daily;
        
        private void Start()
        {
            _missionsManager.OnRefreshed += Refreshed;
            _missionsManager.Refresh();
            
            dailyBtn.onClick.AddListener(ToDailyMissions);
            weeklyBtn.onClick.AddListener(ToWeeklyMissions);
            
            dailyBtn.interactable = false;
            weeklyBtn.interactable = true;

            StartCoroutine(RefreshCycle());
        }

        private void OnDestroy()
        {
            _missionsManager.OnRefreshed -= Refreshed;
            Clear();
        }

        private IEnumerator RefreshCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                _missionsManager.Refresh();
            }
        }
        
        private void ToDailyMissions()
        {
            _showMissionsType = TimedMissionsType.Daily;
            dailyBtn.interactable = false;
            weeklyBtn.interactable = true;
            Refreshed();
        }

        private void ToWeeklyMissions()
        {
            _showMissionsType = TimedMissionsType.Weekly;
            dailyBtn.interactable = true;
            weeklyBtn.interactable = false;
            Refreshed();
        }

        private void LateUpdate()
        {
            if (_showMissionsType is TimedMissionsType.Daily)
            {
                var curDate = _serverTime.GetServerDateTime();
                var nextUpdateDate = ServerTime.UnixTimeStampToDateTime(_missionTimer.GetNextDailyMissionUpdateUnixTime(_missionsManager.DailyMissionsManager.GetSave().startTime));
                var remaining = nextUpdateDate - curDate;
                updateInText.SetText($"Обновятся через: {remaining.FormatTimeRemaining()} \nCurTime: {_serverTime.GetServerDateTime()}");
            }
            else
            {
                var curDate = _serverTime.GetServerDateTime();
                var nextUpdateDate = ServerTime.UnixTimeStampToDateTime(_missionTimer.GetNextWeeklyMissionUpdateUnixTime(_missionsManager.WeeklyMissionsManager.GetSave().startTime));
                var remaining = nextUpdateDate - curDate;
                updateInText.SetText($"Обновятся через: {remaining.FormatTimeRemaining()} \nCurTime: {_serverTime.GetServerDateTime()}");
            }
        }
        
        private void Refreshed()
        {
            Clear();

            List<MissionConfig> configs;
            MissionsSave save;
            switch (_showMissionsType)
            {
                case TimedMissionsType.Daily:
                    configs = _missionsManager.DailyMissionsManager.GetMissionsConfigs(); 
                    save = _missionsManager.DailyMissionsManager.GetSave();
                    break;
                case TimedMissionsType.Weekly:
                    configs = _missionsManager.WeeklyMissionsManager.GetMissionsConfigs();
                    save = _missionsManager.WeeklyMissionsManager.GetSave();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            for (var i = 0; i < configs.Count; i++)
            {
                var item = NightPool.Spawn(missionItemViewPrefab, container);
                item.Setup(
                    configs[i], 
                    _persistentPlayerData.PlayerStatistics.GetStatistic(configs[i].MissionType), 
                    save.valueAtStart[i],
                    save.completed[i], 
                    save.rewarded[i]);
                item.OnClickCollect += OnClickCollect;
                _items.Add(item);
            }
        }

        private void Clear()
        {
            foreach (var missionItemView in _items)
            {
                if (missionItemView && missionItemView.gameObject)
                {
                    missionItemView.OnClickCollect -= OnClickCollect;
                    NightPool.Despawn(missionItemView.gameObject);
                }
            }

            _items.Clear();
        }

        private void OnClickCollect(string id)
        {
            _missionsManager.Collect(TimedMissionsType.Daily, id);
        }
    }
}