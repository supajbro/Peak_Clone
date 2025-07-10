using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public List<Transform> _spawnPoints;
    private int _nextIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform start = _spawnPoints[_nextIndex];
        _nextIndex = (_nextIndex + 1) % _spawnPoints.Count;

        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
