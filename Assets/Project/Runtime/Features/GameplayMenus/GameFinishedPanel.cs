using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Project.Runtime.Core;
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

        public void Setup(bool isWin)
        {
            _isWin = isWin;
            winLoseTitleText.SetText(_isWin ? "ПОБЕДА!" : "Поражение");
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
            
            // TODO: тут нужно оставить включенными только для выпавшие предметы
            
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
    }
}