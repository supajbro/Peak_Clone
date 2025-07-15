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

    [Header("Checks")]
    [SerializeField] private bool _active = false;
    [SerializeField] private bool _activeOnStart = false;
    [SerializeField] private EnableNextSkyscraper _skyscraper;

    private int _index = -1;
    private Vector3 _nextPath = Vector3.zero;

    // DEPRECATED
    [SyncVar] private Vector3 _syncedPosition;

    private void Start()
    {
        ResetPath();
        _manager = FindObjectOfType<LevelManager>();
        _nextPath = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        _active = true;
    }

    private void Update()
    {
        //Debug.Log("Highest Player: " + _manager?.FindHighestPlayer()?.name);
        PositionUpdate();

        //if (isServer)
        //{
        //    PositionUpdate();
        //    _syncedPosition = transform.position;
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(transform.position, _syncedPosition, Time.deltaTime * 10f);
        //}
    }

    /// <summary>
    /// Moves the platform based on the height of the highest player
    /// </summary>
    private void PositionUpdate()
    {
        if (!_skyscraper.IsNextActive && !_activeOnStart)
        {
            _mesh.SetActive(false);
            return;
        }
        _mesh.SetActive(true);

        if (!_active)
        {
            return;
        }

        if (_index >= _points.Count - 1)
        {
            StartCoroutine(RepositionDelay());
            return;
        }

        if (_waiting)
        {
            return;
        }

        //Vector3 next = _points[_index + 1].position;

        if (_manager.FindHighestPlayer() == GameManager.Instance.LocalPlayer)
        {
            _nextPath.y = transform.position.y;
            Debug.Log("[Moving] Not moving: " + _nextPath.y);
        }
        else if (_manager.FindHighestPlayer().transform.position.y > _points[1].position.y)
        {
            _nextPath.y = _points[1].position.y;
            Debug.Log("[Moving] Moving to top: " + _nextPath.y);
        }
        else
        {
            _nextPath.y = _manager.FindHighestPlayer().transform.position.y;
            Debug.Log("[Moving] Moving to highest player: " + _manager.FindHighestPlayer().name + ", " + _nextPath.y);
        }

        transform.position = Vector3.MoveTowards(transform.position, _nextPath, _speed * Time.deltaTime);

        // Reached next point
        if (Vector3.Distance(transform.position, _nextPath) < 0.1f)
        {
            _index++;
        }
    }

    private void ResetPath()
    {
        _active = false;
        _index = 0;
        Vector3 cur = _points[_index].position;
        transform.position = cur;
    }

    private bool _waiting = false;
    private IEnumerator RepositionDelay()
    {
        _waiting = true;
        yield return new WaitForSeconds(5f);
        ResetPath();
        _waiting = false;
    }
}
