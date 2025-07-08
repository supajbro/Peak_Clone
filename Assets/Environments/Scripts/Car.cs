using Mirror;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Car : NetworkBehaviour
{
    [SerializeField] private List<Transform> _points;

    [Header("Speed")]
    [SerializeField] private float _currentSpeed = -1f;
    [SerializeField] private float _normalSpeed = 10f;
    [SerializeField] private float _turningSpeed = 2f;

    [Header("Raycats")]
    [SerializeField] private Transform _leftPosition;
    [SerializeField] private Transform _middlePosition;
    [SerializeField] private Transform _rightPosition;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Knockback")]
    [SerializeField] private float _force = 50f;
    [SerializeField] private float _knockbackDur = 1f;
    [SerializeField] private float _upwardForce = 1f;

    private int _index = -1;
    private bool _active = false;

    [SyncVar] private Vector3 _syncedPosition;
    [SyncVar] private Quaternion _syncedRotation;

    private void Start()
    {
        ResetPath();
    }

    private void Update()
    {
        if (isServer)
        {
            CarUpdate();
            _syncedPosition = transform.position;
            _syncedRotation = transform.rotation;
        }
        else
        {
            // Smoothly interpolate toward the synced values
            transform.position = Vector3.Lerp(transform.position, _syncedPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _syncedRotation, Time.deltaTime * 10f);
        }
        HitPlayer();
    }

    private Quaternion _previousRot;
    private void CarUpdate()
    {
        if (!_active)
        {
            // return;
        }

        if (_index >= _points.Count - 1)
        {
            ResetPath();
            return;
        }

        Vector3 next = _points[_index + 1].position;
        next.y = transform.position.y;

        // Rotate
        Vector3 direction = (next - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // smooth rotation
        }

        // Setup speed
        _currentSpeed = _normalSpeed;
        if(transform.rotation != _previousRot)
        {
            _currentSpeed = _turningSpeed;
        }

        transform.position = Vector3.MoveTowards(transform.position, next, _currentSpeed * Time.deltaTime);

        _previousRot = transform.rotation;

        // Reached next point
        if (Vector3.Distance(transform.position, next) < 0.1f)
        {
            _index++;
        }
    }

    private void HitPlayer()
    {
        const float dist = 5f;
        const float radius = 1f; // adjust for how wide you want the hit to be

        Vector3 origin = _middlePosition.position; // or just transform.position if appropriate
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * dist, Color.red);

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, dist, _playerLayer))
        {
            Player player = hit.collider.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("[Knockback] Hit Player: " + player.name);
                Knockback(player);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_middlePosition.position, 1f);
    }

    private void Knockback(Player player)
    {
        Vector3 dir = transform.forward;
        player.Knockback(dir, _force, _knockbackDur, _upwardForce);
    }

    private void ResetPath()
    {
        _index = 0;

        Vector3 cur = _points[_index].position;
        cur.y = transform.position.y;

        transform.position = cur;
    }

}
