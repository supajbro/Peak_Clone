using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void SetInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Local Player")]
    public Player LocalPlayer;

    [Header("All Players")]
    [SerializeField] private List<Player> _players = new();
    public List<Player> Players => _players;
    public void AddPlayers(Player player)
    { 
        _players.Add(player);
        OnAddPlayer?.Invoke();
    }
    public Action OnAddPlayer;

    private void Awake()
    {
        SetInstance();
    }
}
