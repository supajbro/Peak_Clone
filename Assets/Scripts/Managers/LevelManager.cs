using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private GameManager _manager;

    [Header("Skyscrapers")]
    [SerializeField] private GameObject _skyscraperOne;
    [SerializeField] private GameObject _skyscraperTwo;
    [SerializeField] private GameObject _skyscraperThree;

    [Header("Moving Platforms")]
    [SerializeField] private GameObject _movingPlatformOne;
    [SerializeField] private GameObject _movingPlatformTwo;
    public GameObject MovingPlatformOne => _movingPlatformOne;
    public GameObject MovingPlatformTwo => _movingPlatformTwo;

    [Header("End Goal")]
    [SerializeField] private GameObject _endGoal;
    public GameObject EndGoal => _endGoal;

    [Header("Prefabs")]
    [SerializeField] private Image _playerHeadPrefab;
    private Image _tracker;
    public void SetTracker(Image image)
    {
        _tracker = image;
    }

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

    public void SpawnPlayerHead(Player player)
    {
        Debug.Log("[UI Spawn] Head spawn");
        var head = Instantiate(_playerHeadPrefab, _tracker.transform);
        head.GetComponent<PlayerHead>().Init(player, _tracker, _endGoal);
    }
}
