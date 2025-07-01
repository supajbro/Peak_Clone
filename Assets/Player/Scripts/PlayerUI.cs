using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player _player;

    [SerializeField] private TextMeshProUGUI _playerState;
    [SerializeField] private Slider _stamineSlider;

    public void InitUI(Player thisPlayer)
    {
        _player = thisPlayer;
    }

    private void Start()
    {
        _player.OnPlayerStateChanged += UpdatePlayerStateText;
        _player.MyStamina.OnStaminaChanged += UpdateStaminaSlider;
    }

    private void UpdatePlayerStateText(IPlayerState.PlayerState newState)
    {
        _playerState.text = newState.ToString();
    }

    private void UpdateStaminaSlider(float value)
    {
        _stamineSlider.value = value;
    }
}
