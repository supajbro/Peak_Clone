using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    private NetworkManager _networkManager;

    [SerializeField] private Button _hostButton = null;

    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> _lobbyEnter;

    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized)
        {
            return;
        }

        _hostButton?.onClick.AddListener(HostLobby);

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby()
    {
        _hostButton.gameObject.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            _hostButton.gameObject.SetActive(true);
            return;
        }

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();

        _hostButton.gameObject.SetActive(false);
    }
}
