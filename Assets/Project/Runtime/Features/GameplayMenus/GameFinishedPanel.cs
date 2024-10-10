using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Project.Runtime.Core;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Features.GameplayMenus
{
    public class GameFinishedPanel : PanelBase, IPointerClickHandler
    {
        [Inject] private SceneLoader _sceneLoader;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Animator animator;
        [Space]
        [SerializeField] private TextMeshProUGUI winLoseTitleText;
        [Title("Inv")] 
        [SerializeField] private GridLayoutGroup itemsGridLayoutGroup;
        [SerializeField] private List<InventoryItemView> items;
        [Title("Statistics")]
        [SerializeField] private TextMeshProUGUI totalPlacedTowersStat;
        [SerializeField] private TextMeshProUGUI totalDealtDamageStat;
        [SerializeField] private TextMeshProUGUI totalKilledEnemiesStat;
        [Title("Buttons")]
        [SerializeField] private Button nextButton;

        private bool _isWin;

        private Coroutine _animRoutine;
        private Sequence _itemsSequence;
        private static readonly int Play = Animator.StringToHash("play");
        private static readonly int Skip = Animator.StringToHash("skip");

        private void Start()
        {
            nextButton.onClick.AddListener(ToTheLobby);
            Hide();
        }

        public void SetIsWin(bool isWin)
        {
            _isWin = isWin;
            winLoseTitleText.SetText(_isWin ? "Победа!" : "Поражение");
        }
        
        public override void Show()
        {
            itemsGridLayoutGroup.enabled = false;
            foreach (var inventoryItemView in items)
            {
                inventoryItemView.transform.localScale = Vector3.zero;
            }
            
            base.Show();
            _animRoutine = StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            canvasGroup.alpha = 0f;
            var delay = _isWin ? 0f : 2f;
            canvasGroup
                .DOFade(1f, 0.35f)
                .SetDelay(delay)
                .SetLink(gameObject);

            animator.SetTrigger(Play);
            
            yield return new WaitForSeconds(delay + 0.8f);
            
            _itemsSequence = DOTween.Sequence().SetLink(gameObject);
            for (var index = 0; index < items.Count; index++)
            {
                var inventoryItemView = items[index];
                _itemsSequence.Join(inventoryItemView.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack));
                _itemsSequence.AppendInterval(index * 0.02f);
            }
        }

        private void ToTheLobby()
        {
            StartCoroutine(_sceneLoader.LoadSceneAsync("Lobby"));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (canvasGroup.alpha < 1f) return; // panel not showed yet
            SkipAnimation();
        }
        
        private void SkipAnimation()
        {
            StopCoroutine(_animRoutine);
            _itemsSequence?.Complete();
            foreach (var inventoryItemView in items)
            {
                inventoryItemView.transform.localScale = Vector3.one;
            }
            animator.SetTrigger(Skip);
        }

        public void SetStatistics(int totalPlacedTowers, int totalDamageDealt, int totalKilledEnemies)
        {
            totalPlacedTowersStat.SetText("Установлено строений: " + totalPlacedTowers);
            totalDealtDamageStat.SetText("Нанесено урона: " + totalDamageDealt);
            totalKilledEnemiesStat.SetText("Враго повержено: " + totalKilledEnemies);
        }

        public void SetDrops(List<CurrencyTuple> currencyDrops, CardConfig randomCardDrop)
        {
            var idx = 0;
            
            foreach (var currencyDrop in currencyDrops)
            {
                items[idx].gameObject.SetActive(true);
                items[idx].SetCurrencyData(currencyDrop.currencyConfig, currencyDrop.amount);
                idx++;
            }

            if (randomCardDrop != null)
            {
                items[idx].gameObject.SetActive(true);
                items[idx].SetDeckCardData(new DeckCard
                {
                    CardConfig = randomCardDrop,
                    CardSaveData = new CardSaveData
                    {
                        amount = 1
                    }
                });
            }

            while (idx < items.Count)
            {
                items[idx++].gameObject.SetActive(false);
            }
        }
    }
}