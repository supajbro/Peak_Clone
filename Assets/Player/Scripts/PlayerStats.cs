using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _speedScaler = 10f;

    [Header("Jumping")]
    [SerializeField] private float _minJumpHeight = 5f;
    [SerializeField] private float _maxJumpHeight = 10f;
    [SerializeField] private float _jumpHeightScaler = 10f;

    [Header("Bounce")]
    [SerializeField] private float _bounceHeightScaler = 100f;
    [SerializeField] private float _maxBounceHeight = 100f;

    [Header("Climbing")]
    [SerializeField] private float _climbSpeed = 3f;

    [Header("Rotation")]
    [SerializeField] private float _sensitivity = 10f;
    [SerializeField, Range(0f, 1f)] private float _smoothFactor = 0.5f;

    [Header("Koyote Time")]
    [SerializeField, Tooltip("How long player can be off the ground and still jump")] private float _maxKoyoteTime = 0.5f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _playerLayer;

    // Movement
    public float MinSpeed => _minSpeed;
    public float MaxSpeed => _maxSpeed;
    public float SpeedScaler => _speedScaler;

    // Jumping
    public float MinJumpHeight => _minJumpHeight;
    public float MaxJumpHeight => _maxJumpHeight;
    public float JumpHeightScaler => _jumpHeightScaler;

    // Bounce
    public float BounceHeightScaler => _bounceHeightScaler;
    public float MaxBounceHeight => _maxBounceHeight;

    // Climbing
    public float ClimbSpeed => _climbSpeed;

    // Rotation
    public float Sensitivity => _sensitivity;
    public float SmoothFactor => _smoothFactor;

    // Koyote Time
    public float MaxKoyoteTime => _maxKoyoteTime;

    // Layer Masks
    public LayerMask GroundLayer => _groundLayer;
    public LayerMask WallLayer => _wallLayer;
    public LayerMask PlayerLayer => _playerLayer;

}
