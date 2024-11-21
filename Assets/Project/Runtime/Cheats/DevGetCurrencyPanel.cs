using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using VContainer;

namespace Runtime.Cheats
{
    public class DevGetCurrencyPanel : PanelBase
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;

        public DevGetCurrencyItem getCurrencyItem;
        public RectTransform container;
        
        private void Start()
        {
            Hide();
            foreach (var (key, value) in _persistentPlayerData.WalletByCurrency)
            {
                Instantiate(getCurrencyItem, container).Setup(value);
            }

        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F1)) return;
            if (IsPanelActive) Hide();
            else Show();
        }
    }
}