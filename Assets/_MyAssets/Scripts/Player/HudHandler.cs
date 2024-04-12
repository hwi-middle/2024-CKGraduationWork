using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    private Player _player;

    private void Awake()
    {
        _player = Player.Instance;
    }

    private void Update()
    {
        _hpBar.fillAmount = (float)_player.Hp / _player.PlayerData.playerHp;
    }
}
