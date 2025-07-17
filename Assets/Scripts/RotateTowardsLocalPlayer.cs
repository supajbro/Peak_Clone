using UnityEngine;

public class RotateTowardsLocalPlayer : MonoBehaviour
{
    private GameManager _manager;
    private Player _player;
    [SerializeField] private float rotationSpeed = 360f;

    private void Awake()
    {
        _manager = GameManager.Instance;
    }

    private void Update()
    {
        if(_player == null)
        {
            _player = _manager?.LocalPlayer;
            return;
        }

        Vector3 directionToPlayer = _player.transform.position - transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }
}
