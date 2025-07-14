using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager _manager;

    private void Start()
    {
        _manager = GameManager.Instance;
    }

    public Player FindHighestPlayer()
    {
        if (_manager == null)
        {
            return null;
        }

        if (_manager.Players.Count == 0)
        {
            return null;
        }

        Player highestPlayer = null;
        foreach (var player in _manager.Players)
        {
            if (highestPlayer == player)
            {
                continue;
            }

            if (highestPlayer == null || player.transform.position.y > highestPlayer.transform.position.y)
            {
                highestPlayer = player;
            }
        }
        return highestPlayer;
    }
}
