using System;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Runtime.Features.Tutorial
{
    public class TutorialPanel : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;

        [SerializeField] private GameObject showContinue;
        [SerializeField] private GameObject[] steps;

        public int CurrentStep { get; private set; }
        public event Action StepClicked; 
        
        private void Awake()
        {
            if (_persistentPlayerData.IsInGameTutorialCompleted)
            {
                Destroy(gameObject);
                return;
            }

            foreach (var step in steps)
            {
                step.SetActive(false);
            }
        }

        public void ShowStep(int stepIndex)
        {
            CurrentStep = stepIndex;
            steps[stepIndex].SetActive(true);
            showContinue.SetActive(CurrentStep is 0 or 2 or 3);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ClearStep();
            StepClicked?.Invoke();
        }

        public void ClearStep()
        {
            showContinue.SetActive(false);
            steps[CurrentStep].SetActive(false);
        }
    }
}
