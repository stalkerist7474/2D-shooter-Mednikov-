using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyBalance : MonoBehaviour
{
    [SerializeField] private TMP_Text _money;
    private Player _player;

    private void OnEnable()
    {
       
        _money.text = Player.LocalPlayer.Money.ToString();
        Player.LocalPlayer.MoneyChanged += OnMoneyChaged;
    }

    private void OnDisable()
    {
        Player.LocalPlayer.MoneyChanged -= OnMoneyChaged;
    }

    private void OnMoneyChaged(int money)
    {
        _money.text = money.ToString();
    }


}
