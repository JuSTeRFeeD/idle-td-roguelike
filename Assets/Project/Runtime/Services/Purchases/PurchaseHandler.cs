using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using YG;

namespace Runtime.Services.Purchases
{
    public class PurchaseHandler
    {
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly ISaveManager _saveManager;

        private const string HardCurrencyId = "4b444df0-fd59-4356-83cc-f9abe5c087a7";
        
        public PurchaseHandler(PersistentPlayerData persistentPlayerData, ISaveManager saveManager)
        {
            _persistentPlayerData = persistentPlayerData;
            _saveManager = saveManager;
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
        }

        private void OnPurchaseSuccess(string purchaseId)
        {
            if (purchaseId.Contains("crystals"))
            {
                var giveAmount = uint.Parse(purchaseId.Split("_")[1]);
                var wallet = _persistentPlayerData.GetWalletByCurrencyId(HardCurrencyId);
                wallet.Add(giveAmount);
                _saveManager.Save();
            }
        }
    }
}