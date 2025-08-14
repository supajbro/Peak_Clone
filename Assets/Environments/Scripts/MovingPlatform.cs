using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour
{
    [Header("Main")]
    [SerializeField] private List<Transform> _points;
    [SerializeField] private GameObject _mesh;
    private LevelManager _manager = null;

    [Header("Speed")]
    [SerializeField] private float _speed = 10f;

    [Header("Arrow")]
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Vector3 _scaleUpSize = Vector3.one;
    [SerializeField] private float _scaleTime = .5f;
    [SerializeField] private Ease _ease;

    [Header("Checks")]
    [SerializeField] private bool _active = false;
    [SerializeField] private bool _activeOnStart = false;
    [SerializeField] private EnableNextSkyscraper _skyscraper;

    private int _index = -1;
    private Vector3 _nextPath = Vector3.zero;

    [SyncVar, Tooltip("Ensure position is same across all clients")] private Vector3 _syncedPosition;

    private void Start()
    {
        ResetPath();
        _manager = FindObjectOfType<LevelManager>();
        _nextPath = transform.position;
        _arrow.transform.localScale = Vector3.zero;
    }

    private Player _highestPlayer;
    private void OnTriggerEnter(Collider collision)
    {
        _active = true;
        _highestPlayer = _manager.FindHighestPlayer();
    }

    private void Update()
    {
        PositionUpdate();
        
        // Sync position
        if (isServer)
        {
            PositionUpdate();
            _syncedPosition = transform.position;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _syncedPosition, Time.deltaTime * _speed);
        }
    }

    /// <summary>
    /// Moves the platform based on the height of the highest player
    /// </summary>
    private void PositionUpdate()
    {
        // Decide if mesh should be enabled
        if (!_skyscraper.ActivateNextSkyscraper && !_activeOnStart)
        {
            _mesh.SetActive(false);
            return;
        }
        _mesh.SetActive(true);

        EnableArrow();

        if (!_active || _highestPlayer == null)
        {
            return;
        }

        // Reached the end point, move pack
        if (_index >= _points.Count - 1)
        {
            StartCoroutine(RepositionDelay());
            return;
        }

        // Reached top and waiting to respawn
        if (_waitingToRespawn)
        {
            return;
        }

        // Don't move the platform
        if(Vector3.Distance(_highestPlayer.transform.position, _points[0].transform.position) < 10f && !_skyscraper.ReachedTopSkyscraper)
        {
            _nextPath.y = transform.position.y;
            Debug.Log("[Moving] Not moving: " + _nextPath.y);
        }
        // Moving to the top of the skyscraper
        else if (_highestPlayer.transform.position.y > _points[1].position.y || _skyscraper.ReachedTopSkyscraper)
        {
            _nextPath.y = _points[1].position.y;
            Debug.Log("[Moving] Moving to top: " + _nextPath.y);
        }
        // Move to the highest player
        else
        {
            _nextPath.y = _highestPlayer.transform.position.y;
            Debug.Log("[Moving] Moving to highest player: " + _manager.FindHighestPlayer().name + ", " + _nextPath.y);
        }

        transform.position = Vector3.MoveTowards(transform.position, _nextPath, _speed * Time.deltaTime);

        // Reached next point
        if (Vector3.Distance(transform.position, _nextPath) < 0.1f)
        {
            _index++;
        }
    }

    private bool _previousReachedSkyscraper = false;
    private void EnableArrow()
    {
        if (_skyscraper.ReachedTopSkyscraper && !_previousReachedSkyscraper)
        {
            _previousReachedSkyscraper = true;
            _arrow.transform.DOScale(_scaleUpSize, _scaleTime).SetEase(_ease);
        }
    }

    private void ResetPath()
    {
        _active = false;
        _index = 0;
        Vector3 cur = _points[_index].position;
        transform.position = cur;
    }

    private bool _waitingToRespawn = false;
    private IEnumerator RepositionDelay()
    {
        _waitingToRespawn = true;
        yield return new WaitForSeconds(5f);
        ResetPath();
        _waitingToRespawn = false;
    }
}
