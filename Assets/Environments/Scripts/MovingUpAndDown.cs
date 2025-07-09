using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUpAndDown : NetworkBehaviour
{
    [SyncVar] private Vector3 _syncedPosition;
    [SerializeField] private EnableNextSkyscraper _skyscraper;
    [SerializeField] private MeshRenderer _mesh;

    [SerializeField, Tooltip("Height of movement")] private float _amplitude = 1f;
    [SerializeField, Tooltip("Speed")] private float _frequency = 1f;
    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
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
        if (!_skyscraper.IsNextActive)
        {
            _mesh.enabled = false;
            return;
        }
        _mesh.enabled = true;

        float newY = Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.position = _startPos + new Vector3(0f, newY, 0f);
    }
}
