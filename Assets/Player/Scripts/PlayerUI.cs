using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private Player _player;

    [SerializeField] private TextMeshProUGUI _playerState;

    public void InitUI(Player thisPlayer)
    {
        _player = thisPlayer;
    }

    private void Start()
    {
        _player.OnPlayerStateChanged += UpdatePlayerStateText;
    }

    private void UpdatePlayerStateText(IPlayerState.PlayerState newState)
    {
        _playerState.text = newState.ToString();
    }
}
