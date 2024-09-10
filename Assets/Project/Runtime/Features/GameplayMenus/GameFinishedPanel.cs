using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.GameplayMenus
{
    public class GameFinishedPanel : PanelBase
    {
        [SerializeField] private Button nextButton;

        private void Start()
        {
            nextButton.onClick.AddListener(ToTheLobby);
        }

        private void ToTheLobby()
        {
            // TODO: load lobby
        }
    }
}