using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour
{
    [SerializeField] private List<Transform> _points;

    [Header("Speed")]
    [SerializeField] private float _speed = 10f;

    private int _index = -1;
    private bool _active = false;

    [SyncVar] private Vector3 _syncedPosition;

    private void Start()
    {
        ResetPath();
    }

    private void OnTriggerEnter(Collider collision)
    {
        //if (collision.gameObject.tag == "Player")
        {
            _active = true;
        }
    }

    private void Update()
    {
        if (isServer)
        {
            Move();
            _syncedPosition = transform.position;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _syncedPosition, Time.deltaTime * 10f);
        }
    }

    private void Move()
    {
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

        Vector3 next = _points[_index + 1].position;
        transform.position = Vector3.MoveTowards(transform.position, next, _speed * Time.deltaTime);

        // Reached next point
        if (Vector3.Distance(transform.position, next) < 0.1f)
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
