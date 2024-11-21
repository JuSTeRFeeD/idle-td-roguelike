using System.Collections;
using System.Collections.Generic;
using Project.Runtime.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevGetCurrencyItem : MonoBehaviour
{
    public Image icon;
    public Button[] buttons;
    public int[] giveAmount = new []{50, 250, 500, 1000};
    
    public void Setup(Wallet wallet)
    {
        icon.sprite = wallet.CurrencyConfig.Icon;

        for (var i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().SetText($"Add {giveAmount[i]}");
            buttons[i].onClick.AddListener(() =>
            {
                wallet.Add((uint)giveAmount[index]);
            });
        }
    }
}
