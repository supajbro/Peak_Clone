using Mirror;
using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Player : NetworkBehaviour, IPlayerState
{
    #region - STATES -
    [Header("States")]
    [SerializeField] private IPlayerState.PlayerState _currentState = IPlayerState.PlayerState.Idle;
    [SerializeField] private IPlayerState.PlayerState _previousState;
    public IPlayerState.PlayerState CurrentState
    {
        get => _currentState;
        set
        {
            _previousState = _currentState;
            _currentState = value;
        }
    }

    public IPlayerState.PlayerState PreviousState
    {
        get => _previousState;
        set => _previousState = value;
    }

    public Action<IPlayerState.PlayerState> OnPlayerStateChanged;
    public void SetState(IPlayerState.PlayerState state)
    {
        if(state == _currentState)
        {
            return;
        }

        _previousState = _currentState;
        _currentState = state;
        OnPlayerStateChanged?.Invoke(_currentState);
    }
    #endregion

    [Header("Main")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerUI _ui;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _playerHead;
    [SerializeField] private GameObject _playerEyes;
    [SerializeField] private GameObject _playerBody;
    private CharacterController _controller;

    [Header("Camera")]
    [SerializeField] private Camera _camPrefab;
    [SerializeField] private Transform _camPosition;
    private Camera _cam;

    [Header("Movement")]
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private bool _running = false;
    private float _verticalRot;
    private float _horizontalRot;

    [Header("Jumping")]
    [SerializeField] private bool _jumping = false;
    [SerializeField] private float _currentJumpHeight = 0f;

    [Header("Impact")]
    [SerializeField] private bool _impact = false;
    [SerializeField] private float _currentImpactHeight = 0f;
    [SerializeField] private float _flipDuration = 1f;
    [SerializeField] private float _flipTimer = 0f;
    int _rotationDegree = 0;

    [Header("Koyote Time")]
    [SerializeField] private float _currentKoyoteTime = 0f;

    [Header("Stamina")]
    [SerializeField] private Stamina _stamina;
    public Stamina MyStamina => _stamina;

    [Header("Misc")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    [Header("Ground Check Rays")]
    [SerializeField] private Transform _middle;
    [SerializeField] private Transform _middleBack;
    [SerializeField] private Transform _middleFront;
    [SerializeField] private Transform _middleRight;
    [SerializeField] private Transform _middleLeft;
    [SerializeField] private Transform _backRight;
    [SerializeField] private Transform _backLeft;
    [SerializeField] private Transform _frontRight;
    [SerializeField] private Transform _frontLeft;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        gameObject.name = "Player: " + UnityEngine.Random.Range(10, 1000).ToString();

        _controller = GetComponent<CharacterController>();

        _cam = Instantiate(_camPrefab, _camPosition);
        _cam.transform.localPosition = Vector3.zero;

        var ui = Instantiate(_ui);
        ui.InitUI(this);

        _stamina.SetStamina(_stamina.MaxStamina);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide the head client side only so it doesn't clip with camera
        _playerHead.SetActive(false);
        _playerEyes.SetActive(false);
        _playerBody.SetActive(false);
    }

    private void Update()
    {
        StateUpdate();
        RotateUpdate();
        MovementUpdate();
        PushPlayer();
    }

    /// <summary>
    /// Update functon for the states
    /// </summary>
    public void StateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        switch (_currentState)
        {
            case IPlayerState.PlayerState.Idle:
                IdleUpdate();
                break;

            case IPlayerState.PlayerState.Walking:
                WalkingUpdate();
                break;

            case IPlayerState.PlayerState.Running:
                RunningUpdate();
                break;

            case IPlayerState.PlayerState.Jumping:
                JumpingUpdate();
                break;

            case IPlayerState.PlayerState.Falling:
                FallingUpdate();
                break;

            case IPlayerState.PlayerState.Climbing:
                ClimbingUpdate();
                break;

            case IPlayerState.PlayerState.BigImpact:
                BigImpactUpdate();
                break;
        }
    }

    /// <summary>
    /// Determines what state the player should be in based on their movement
    /// </summary>
    private void MovementUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        ApplyKnockback();

        // Don't run regular movement when player has hit the ground hard
        if (_impact)
        {
            return;
        }

        // Rotate player to camera rot
        Quaternion facingDirection = Quaternion.Euler(0, _cam.transform.rotation.eulerAngles.y, 0);
        transform.rotation = facingDirection;

        if (CanClimb() && Input.GetMouseButton(0) && _stamina.CurrentStamina > _stamina.MinStamina)
        {
            SetState(IPlayerState.PlayerState.Climbing);
        }
        else if (Input.GetMouseButtonUp(0) && _currentState == IPlayerState.PlayerState.Climbing)
        {
            SetState(IPlayerState.PlayerState.Falling);
        }
        else if (CanClimb() && _stamina.CurrentStamina <= _stamina.MinStamina)
        {
            SetState(IPlayerState.PlayerState.Falling);
        }

        // Don't run the rest of this block of code if climbing
        if (_currentState == IPlayerState.PlayerState.Climbing)
        {
            return;
        }

        if (!IsGrounded() && !_jumping)
        {
            SetState(IPlayerState.PlayerState.Falling);
            _currentKoyoteTime += Time.deltaTime;
        }
        else
        {
            _currentKoyoteTime = 0f;
        }

        // Movement data based on input
        Vector3 movement = new Vector3(h * .25f, _currentJumpHeight, v * .25f);

        // If movement starts in air, make them move
        if ((_currentState == IPlayerState.PlayerState.Jumping || _currentState == IPlayerState.PlayerState.Falling)
            && movement.magnitude != 0f)
        {
            _currentSpeed = (_currentSpeed < _stats.MaxSpeed) ? _currentSpeed + Time.deltaTime * _stats.SpeedScaler : _stats.MaxSpeed;
        }

        // Add upwards movement when on a moving platform
        if (_currentPlatform != null)
        {
            Vector3 platformDelta = _currentPlatform.position - _lastPlatformPosition;
            _controller?.Move(platformDelta);
            _lastPlatformPosition = _currentPlatform.position;
        }

        // Move player
        Vector3 move = (facingDirection * movement) * _currentSpeed * Time.deltaTime;
        move.y = _currentJumpHeight * Time.deltaTime;
        _controller?.Move(move);

        CheckIfRunning(movement);

        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || _currentKoyoteTime < _stats.MaxKoyoteTime))
        {
            SetState(IPlayerState.PlayerState.Jumping);
        }
        else if (IsGrounded() && _currentJumpHeight == 0)
        {
            if (movement.magnitude == 0)
            {
                SetState(IPlayerState.PlayerState.Idle);
                _running = false;
            }
            else
            {
                SetState((_running) ? IPlayerState.PlayerState.Running : IPlayerState.PlayerState.Walking);
            }
        }
    }

    /// <summary>
    /// Rotate the camera via the mouse
    /// </summary>
    private void RotateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (_impact)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update vertical and horizontal rotation
        _verticalRot -= mouseY * _stats.Sensitivity;
        _verticalRot = Mathf.Clamp(_verticalRot, -70f, 70);
        _horizontalRot += mouseX * _stats.Sensitivity;

        // Rotate camera to mouse position
        float smoothedVerticalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.x, _verticalRot, _stats.SmoothFactor);
        float smoothedHorizontalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.y, _horizontalRot, _stats.SmoothFactor);
        _cam.transform.rotation = Quaternion.Euler(smoothedVerticalRot, smoothedHorizontalRot, 0f);
    }

    public void IdleUpdate()
    {
        _anim.SetFloat("moveSpeed", 0f);
        _anim.SetBool("isClimbing", false);
        _stamina.ReplenishStamina();

        _currentSpeed = 0f;
        _currentJumpHeight = 0f;

        if (!IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void WalkingUpdate()
    {
        _anim.SetFloat("moveSpeed", .5f);
        _anim.SetBool("isClimbing", false);
        _stamina.ReplenishStamina();

        _currentSpeed = (_currentSpeed > _stats.MinSpeed) ? _currentSpeed - Time.deltaTime * _stats.SpeedScaler : _stats.MinSpeed;

        if (!IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void RunningUpdate()
    {
        _anim.SetFloat("moveSpeed", 1f);
        _anim.SetBool("isClimbing", false);

        _stamina.DrainStamina();

        _currentSpeed = (_currentSpeed < _stats.MaxSpeed) ? _currentSpeed + Time.deltaTime * _stats.SpeedScaler : _stats.MaxSpeed;

        if (_stamina.CurrentStamina <= _stamina.MinStamina)
        {
            _running = false;
            SetState(IPlayerState.PlayerState.Walking);
        }

        if (!IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void JumpingUpdate()
    {
        _anim.SetBool("isJumping", true);
        _stamina.DrainStamina();

        _jumping = true;
        _currentJumpHeight = Mathf.Max(_stats.MinJumpHeight, _currentJumpHeight);
        _currentJumpHeight = (_currentJumpHeight < _stats.MaxJumpHeight) ? _currentJumpHeight + Time.deltaTime * _stats.JumpHeightScaler : _stats.MaxJumpHeight;

        if(_currentJumpHeight >= _stats.MaxJumpHeight)
        {
            _jumping = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void FallingUpdate()
    {
        _currentJumpHeight -= Time.deltaTime * _stats.JumpHeightScaler;

        if (IsGrounded())
        {
            Debug.Log("Current jump height: " + _currentJumpHeight);
            SetState(_currentJumpHeight < _stats.FallPower ? IPlayerState.PlayerState.BigImpact : IPlayerState.PlayerState.Idle);
            //_currentJumpHeight = 0f;
        }
    }

    #region - CLIMBING -
    public void ClimbingUpdate()
    {
        _anim.SetBool("isClimbing", true);
        _stamina.DrainStamina();

        if (_running)
        {
            _running = false;
        }

        if(_currentJumpHeight != 0f)
        {
            _currentJumpHeight = 0f;
        }

        // Make player jump to position looking at
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }

        if (ClimbDirection(out Vector3 climbDir, out Vector3 wallNormal))
        {
            // Get direction vectors based on wall
            Vector3 climbUp = climbDir;
            Vector3 wallRight = Vector3.Cross(wallNormal, climbUp).normalized;
            Vector3 wallForward = Vector3.Cross(wallRight, wallNormal).normalized;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                move += climbUp;
            if (Input.GetKey(KeyCode.S))
                move -= climbUp;
            if (Input.GetKey(KeyCode.D))
                move += wallRight;
            if (Input.GetKey(KeyCode.A))
                move -= wallRight;

            // Move forward
            if (!_top && _bottom)
            {
                Debug.Log("[Climbing] Going forward");
                move += transform.forward;
                move += climbUp;
            }

            if (move != Vector3.zero)
            {
                Vector3 climbVector = move.normalized * _stats.ClimbSpeed * Time.deltaTime;
                _controller?.Move(climbVector);

                Debug.DrawRay(transform.position, climbVector, Color.cyan);
            }
        }
    }

    const float ClimbCheckDistance = 1f;
    const float ClimbRadius = .5f;
    public bool CanClimb()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        return Physics.SphereCast(origin, ClimbRadius, direction, out RaycastHit hit, ClimbCheckDistance, _wallLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, ClimbRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * 4f, ClimbRadius);
    }

    bool _top = false;
    bool _bottom = false;
    public bool ClimbDirection(out Vector3 climbDirection, out Vector3 wallNormal)
    {
        if (_isKnockback)
        {
            _isKnockback = false;
        }

        climbDirection = Vector3.zero;
        wallNormal = Vector3.zero;

        Vector3 rayOrigin = transform.position + Vector3.up;
        Vector3 bottomRayOrigin = transform.position - Vector3.up;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * ClimbCheckDistance, Color.blue);

        _top = false;
        _bottom = false;

        // Top of player
        //if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, ClimbCheckDistance, _wallLayer))
        if(Physics.SphereCast(rayOrigin, ClimbRadius, rayDirection, out RaycastHit hit, ClimbCheckDistance, _wallLayer))
        {
            wallNormal = hit.normal;

            Vector3 wallRight = Vector3.Cross(wallNormal, Vector3.up).normalized;
            climbDirection = Vector3.Cross(wallRight, wallNormal).normalized;

            Debug.DrawRay(hit.point, wallNormal, Color.red);
            Debug.DrawRay(hit.point, wallRight, Color.yellow);
            Debug.DrawRay(hit.point, climbDirection, Color.green);

            _top = true;
        }

        // Bottom of player - runs if top is false so player can still climb up and get over the hill
        //if (Physics.SphereCast(bottomRayOrigin, ClimbRadius, rayDirection, out RaycastHit hitBottom, ClimbCheckDistance, _wallLayer))
        if (Physics.Raycast(bottomRayOrigin, rayDirection, out RaycastHit hitBottom, ClimbCheckDistance, _wallLayer))
        {
            wallNormal = hitBottom.normal;

            Vector3 wallRight = Vector3.Cross(wallNormal, Vector3.up).normalized;
            climbDirection = Vector3.Cross(wallRight, wallNormal).normalized;

            Debug.DrawRay(hitBottom.point, wallNormal, Color.red);
            Debug.DrawRay(hitBottom.point, wallRight, Color.yellow);
            Debug.DrawRay(hitBottom.point, climbDirection, Color.green);

            _bottom = true;
        }

        if(_top || _bottom)
        {
            return true;
        }

        SetState((_running) ? IPlayerState.PlayerState.Running : IPlayerState.PlayerState.Walking);
        return false;
    }
    #endregion

    public void BigImpactUpdate()
    {
        // Init of state
        if (!_impact)
        {
            _flipTimer = 0f;
            _rotationDegree = (UnityEngine.Random.value) < 0.5f ? 360 : -360;
        }

        _anim.SetBool("isJumping", true);

        _currentJumpHeight = 0f;
        _impact = true;

        _currentImpactHeight = Mathf.Max(_stats.MinImpactHeight, _currentImpactHeight);
        _currentImpactHeight = (_currentImpactHeight < _stats.MaxImpactHeight) ? _currentImpactHeight + Time.deltaTime * _stats.ImpactScaler : _stats.MaxImpactHeight;

        // Move player upwards
        Vector3 movement = new Vector3(0f, _currentImpactHeight, 0f);
        Vector3 move = (movement) * _currentSpeed * Time.deltaTime;
        move.y = _currentImpactHeight * Time.deltaTime;
        _controller?.Move(move);

        _flipTimer += Time.deltaTime;

        // Rotate 360 degrees
        float degreesPerSecond = _rotationDegree / _flipDuration;
        float deltaRotation = degreesPerSecond * Time.deltaTime;
        transform.Rotate(Vector3.right * deltaRotation, Space.Self);

        if (_currentImpactHeight >= _stats.MaxImpactHeight && _flipTimer >= _flipDuration)
        {
            _currentImpactHeight = 0f;
            _impact = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    private Transform _currentPlatform;
    private Vector3 _lastPlatformPosition;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "MovingPlatform")
        {
            _currentPlatform = other.gameObject.transform;
            _lastPlatformPosition = _currentPlatform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            _currentPlatform = null;
        }
    }

    public bool IsGrounded()
    {
        const float GroundCheckDistance = .5f;
        Debug.DrawRay(_middle.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_middleBack.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_middleFront.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_middleLeft.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_middleRight.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_backLeft.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_backRight.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_frontLeft.position, Vector3.down * GroundCheckDistance, Color.red);
        Debug.DrawRay(_frontRight.position, Vector3.down * GroundCheckDistance, Color.red);

        if (Physics.Raycast(_middle.position, Vector3.down, out RaycastHit middle, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_middleBack.position, Vector3.down, out RaycastHit middleBack, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_middleFront.position, Vector3.down, out RaycastHit middleFront, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_middleLeft.position, Vector3.down, out RaycastHit middleLeft, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_middleRight.position, Vector3.down, out RaycastHit middleRight, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_backLeft.position, Vector3.down, out RaycastHit backLeft, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_backRight.position, Vector3.down, out RaycastHit backRight, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_frontLeft.position, Vector3.down, out RaycastHit frontLeft, GroundCheckDistance, _groundLayer))
        {
            return true;
        }

        if (Physics.Raycast(_frontRight.position, Vector3.down, out RaycastHit frontRight, GroundCheckDistance, _groundLayer))
        {
            return true;
        }
        return false;
    }

    private void CheckIfRunning(Vector3 dir)
    {
        if (dir.magnitude != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && _stamina.CurrentStamina > _stamina.MinStamina)
            {
                _running = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _running = false;
            }
        }
    }

    public void BouncePlayer()
    {
        _anim.SetBool("isJumping", true);

        _jumping = true;
        _currentJumpHeight = Mathf.Max(_stats.MinJumpHeight, _currentJumpHeight);
        _currentJumpHeight = (_currentJumpHeight < _stats.MaxBounceHeight) ? _currentJumpHeight + Time.deltaTime * _stats.BounceHeightScaler : _stats.MaxBounceHeight;

        if (_currentJumpHeight >= _stats.MaxBounceHeight)
        {
            _jumping = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    private float _rayDistance = 5f; // How far you want to check for hits
    public void PushPlayer()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(1))
        {
            _anim.SetTrigger("interact");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _stats.PlayerLayer))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Player") && hitObject != gameObject)
                {
                    Debug.Log("[Client] Requesting push on: " + hitObject.name);
                    NetworkIdentity hitIdentity = hitObject.GetComponent<NetworkIdentity>();
                    if (hitIdentity != null)
                    {
                        CmdPunch(hitIdentity);
                    }
                }
            }
        }
    }

    [Command]
    private void CmdPunch(NetworkIdentity hitPlayer)
    {
        var knockback = hitPlayer.GetComponent<Player>();
        RpcPlayPunch();
        if (knockback != null)
        {
            Debug.Log("[Client] Pushing");
            Vector3 direction = (hitPlayer.transform.position - transform.position).normalized;
            knockback.RpcApplyKnockback(direction, 10f, 0.25f, 1f); // direction, force, duration, upward
            //knockback.SetState(IPlayerState.PlayerState.Jumping);
        }
    }

    [TargetRpc]
    public void RpcApplyKnockback(Vector3 direction, float force, float duration, float upwardForce)
    {
        Knockback(direction, force, duration, upwardForce);
        _anim.SetTrigger("interact");
    }

    [ClientRpc]
    private void RpcPlayPunch()
    {
        _anim.SetTrigger("interact");
    }

    #region - KNOCKBACK -
    private void ApplyKnockback()
    {
        if (_isKnockback)
        {
            if (IsGrounded() && _stopKnockback)
            {
                _isKnockback = false;
                _knockbackVelocity = Vector3.zero;
            }
            Debug.Log("[Knockback] Started knockback: " + gameObject.name);
            _controller.Move(_knockbackVelocity * Time.deltaTime);
        }
    }

    private bool _isKnockback;
    private bool _stopKnockback = false;
    private Vector3 _knockbackVelocity;
    public void Knockback(Vector3 direction, float force, float duration, float upwardForce = 0f, bool stopKnockback = false)
    {
        if (_isKnockback)
        {
            return;
        }
        _stopKnockback = false;
        Vector3 finalDirection = direction.normalized + Vector3.up * upwardForce;
        StartCoroutine(DoKnockback(finalDirection, force, duration, stopKnockback));
    }

    private IEnumerator DoKnockback(Vector3 direction, float force, float duration, bool stopKnockback = false)
    {
        _isKnockback = true;
        float timer = 0f;
        _knockbackVelocity = direction.normalized * force;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _stopKnockback = true;

        if (stopKnockback)
        {
            _knockbackVelocity = Vector3.zero;
            _isKnockback = false;
        }
    }
    #endregion
}

[System.Serializable]
public class Stamina : IStamina
{
    [SerializeField] private float _currentStamina;
    public float CurrentStamina
    {
        get => _currentStamina;
        set
        {
            _currentStamina = value;
        }
    }

    [SerializeField] private float _maxStamina;
    public float MaxStamina
    {
        get => _maxStamina;
        set
        {
            _maxStamina = value;
        }
    }

    [SerializeField] private float _drainScaler;
    public float DrainScaler
    {
        get => _drainScaler;
        set
        {
            _drainScaler = value;
        }
    }

    [SerializeField] private float _replenishScaler;
    public float ReplenishScaler
    {
        get => _replenishScaler;
        set
        {
            _replenishScaler = value;
        }
    }

    [SerializeField] private float _minStamina = 10f;
    public float MinStamina => _minStamina;

    public Action<float> OnStaminaChanged { get; set; }

    public void DrainStamina()
    {
        _currentStamina = Mathf.Max(_minStamina, _currentStamina - Time.deltaTime * _drainScaler);
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    public void ReplenishStamina()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        _currentStamina = Mathf.Min(_maxStamina, _currentStamina + Time.deltaTime * _replenishScaler);
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    public void SetStamina(float value)
    {
        _currentStamina = value;
        OnStaminaChanged?.Invoke(_currentStamina);
    }
}
