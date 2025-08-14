using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

    #region - VARIABLES -
    [Header("Main")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Animator _anim;
    [SerializeField] private List<SkinnedMeshRenderer> _meshToHide;
    private CharacterController _controller;
    private PlayerParticles _particles;
    private LevelManager _manager;

    [Header("Name")]
    [SerializeField] private TextMeshPro _playerNameTxt;
    [SyncVar] private string _playerName;

    [Header("Audio")]
    private PlayerAudio _audio;
    public PlayerAudio Audio => _audio;

    [Header("Allow Movement")]
    private bool _canMove = false;
    public bool CanMove => _canMove;
    public void SetMovement(bool value) { _canMove = value; }

    [Header("UI")]
    [SerializeField] private PlayerUI _uiPrefab;
    private PlayerUI _ui;
    public PlayerUI UI => _ui;

    [Header("Camera")]
    [SerializeField] private Camera _camPrefab;
    [SerializeField] private Transform _camPosition;
    private Camera _cam;

    [Header("Movement")]
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private bool _running = false;
    private float _verticalRot;
    private float _horizontalRot;
    private Quaternion _facingDirection;

    [Header("Jumping")]
    [SerializeField] private bool _jumping = false;
    [SerializeField] private float _currentJumpHeight = 0f;

    [Header("Impact")]
    [SerializeField] private bool _impact = false;
    [SerializeField] private float _currentImpactHeight = 0f;
    [SerializeField] private float _flipDuration = 1f;
    [SerializeField] private float _flipTimer = 0f;
    private int _rotationDegree = 0;

    [Header("Head Bopping")]
    private float _bopTimer;
    private Vector3 _originalCamLocalPos;

    [Header("Koyote Time")]
    [SerializeField] private float _currentKoyoteTime = 0f;

    [Header("Knockback")]
    [Tooltip("Has player started knockback")] private bool _startedKnockback;
    [Tooltip("If true, finish knockback over X amount of seconds")] private bool _stopKnockbackOvertime = false;
    [Tooltip("Direction player will move when knockbacked")] private Vector3 _knockbackVelocity;
    [Tooltip("How long user has been knockback for")] private float _knockbackTime = 0f;
    private bool _knockbackOnImpact = false;
    private Vector3 _previousVelocity = Vector3.zero;

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

    [Header("Game Timer")]
    [SerializeField, SyncVar] private float _time;
    public Action<float> OnTimeChanged;
    #endregion

    #region - INIT -
    private void Start()
    {
        LocalPlayerInit();

        // Set the local player name for all clients
        _playerNameTxt.text = _playerName;
        gameObject.name = _playerName;

        GameManager.Instance?.AddPlayers(this);
        SpawnLocalPlayerUI();
    }

    private void LocalPlayerInit()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        _playerName = SteamManager.Initialized && !string.IsNullOrEmpty(SteamFriends.GetPersonaName())
            ? SteamFriends.GetPersonaName()
            : "Player: " + UnityEngine.Random.Range(10, 1000);
        _playerNameTxt.gameObject.SetActive(false);

        GameManager.Instance.LocalPlayer = this;

        _canMove = true;

        _controller = GetComponent<CharacterController>();

        _audio = GetComponent<PlayerAudio>();
        _particles = GetComponent<PlayerParticles>();

        _cam = Instantiate(_camPrefab, _camPosition);
        _cam.transform.localPosition = Vector3.zero;
        _originalCamLocalPos = _cam.transform.localPosition;

        _ui = Instantiate(_uiPrefab);
        _ui.InitUI(this);
        _manager = FindObjectOfType<LevelManager>();
        _manager?.SetTracker(_ui?.Tracker);

        _stamina.SetStamina(_stamina.MaxStamina);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide the head client side only so it doesn't clip with camera
        foreach (var mesh in _meshToHide)
        {
            mesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    /// <summary>
    /// Finds all the players so we can spawn their head
    /// </summary>
    private void SpawnLocalPlayerUI()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Spawn in all existing player heads
        foreach (var player in GameManager.Instance.Players)
        {
            _manager.SpawnPlayerHead(player);
        }
        
        // Add callback so we can add player heads for users who join later
        GameManager.Instance.OnAddPlayer += _manager.SpawnPlayerHead;
    }
    #endregion

    #region - UPDATE FUNC -
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.I))
        {
            SkipToSkyscraper(_manager.MovingPlatformOne);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            SkipToSkyscraper(_manager.MovingPlatformTwo);
        }
#endif

        if(!_canMove)
        {
            return;
        }

        StateUpdate();
        RotateUpdate();
        MovementUpdate();
        PushPlayerUpdate();
        ServerUpdate();
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

        KnockbackUpdate();

        // Don't run regular movement when player has hit the ground hard
        if (_impact)
        {
            return;
        }

        // Rotate player to camera rot
        if (!_startedKnockback)
        {
            _facingDirection = Quaternion.Euler(0, _cam.transform.rotation.eulerAngles.y, 0);
            transform.rotation = _facingDirection;
        }

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

        // Is player falling
        if (!IsGrounded() && !_jumping)
        {
            SetState(IPlayerState.PlayerState.Falling);
            _currentKoyoteTime += Time.deltaTime;
        }
        else if (_jumping)
        {
            _currentKoyoteTime = _stats.MaxKoyoteTime;
        }
        else if(IsGrounded())
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
        Vector3 move = (_facingDirection * movement) * _currentSpeed * Time.deltaTime;
        move.y = _currentJumpHeight * Time.deltaTime;
        _controller?.Move(move);

        // Check if player has started running
        if (movement.magnitude != 0)
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

        // Check if player has jumped
        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || _currentKoyoteTime < _stats.MaxKoyoteTime) /*&& !_jumping*/)
        {
            SetState(IPlayerState.PlayerState.Jumping);
            _audio.JumpAudio();
        }
        // Check if player is idle or moving
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
    /// Code for the host to update
    /// </summary>
    private void ServerUpdate()
    {
        if (!isServer)
        {
            return;
        }

        _time += Time.deltaTime;
        OnTimeChanged.Invoke(_time);
    }
    #endregion

    #region - CAMERA UPDATE -
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

        if (_startedKnockback)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        const float LookAngle = 85f;

        // Update vertical and horizontal rotation
        _verticalRot -= mouseY * _stats.Sensitivity;
        _verticalRot = Mathf.Clamp(_verticalRot, -LookAngle, LookAngle);
        _horizontalRot += mouseX * _stats.Sensitivity;

        // Rotate camera to mouse position
        float smoothedVerticalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.x, _verticalRot, _stats.SmoothFactor);
        float smoothedHorizontalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.y, _horizontalRot, _stats.SmoothFactor);
        _cam.transform.rotation = Quaternion.Euler(smoothedVerticalRot, smoothedHorizontalRot, 0f);

        HeadBoppingUpdate();
    }

    private bool _playingLandingBop = false;
    private float _landingBopTimer = 0f;
    private float _landingBopDuration = 0.5f;
    private void HeadBoppingUpdate()
    {
        // Head bopping effect
        bool isMoving = (_currentState == IPlayerState.PlayerState.Walking || _currentState == IPlayerState.PlayerState.Running) && !_playingLandingBop;
        if (isMoving)
        {
            float s = (_currentState == IPlayerState.PlayerState.Walking) ? _stats.BopWalkSpeed : _stats.BopRunSpeed;
            _bopTimer += Time.deltaTime * s;
            float bopAmount = Mathf.Sin(_bopTimer) * _stats.BopHeight;
            Vector3 bopPosition = _originalCamLocalPos + new Vector3(0f, bopAmount, 0f);
            _cam.transform.localPosition = bopPosition;
        }
        else
        {
            // Reset position when not moving
            _bopTimer = 0f;
            _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, _originalCamLocalPos, Time.deltaTime * 5f);
        }

        if (_playingLandingBop)
        {
            _landingBopTimer += Time.deltaTime;

            float t = _landingBopTimer / _landingBopDuration;
            t = Mathf.Clamp01(t);

            // Ease-out curve (fast at first, then slows down)
            float downwardOffset = Mathf.Lerp(-_stats.BopHeight * 5f, 0f, t);

            // Apply downward offset to camera position
            Vector3 bopPosition = _originalCamLocalPos + new Vector3(0f, downwardOffset, 0f);
            _cam.transform.localPosition = bopPosition;

            if (_landingBopTimer >= _landingBopDuration)
            {
                _playingLandingBop = false;
            }
        }
        else if(!isMoving)
        {
            // Reset camera position when not bopping
            _bopTimer = 0f;
            _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, _originalCamLocalPos, Time.deltaTime * 5f);
        }
    }
    #endregion

    #region - STATE UPDATE -
    public void IdleUpdate()
    {
        if(_currentState != _previousState)
        {
            _previousState = _currentState;
        }

        _anim.SetFloat("moveSpeed", 0f);
        _anim.SetBool("isClimbing", false);
        _anim.SetBool("isFalling", false);
        _stamina.ReplenishStamina();

        _currentSpeed = (_currentSpeed > 0f) ? _currentSpeed - Time.deltaTime * _stats.SpeedScaler : 0f;
        _currentJumpHeight = 0f;

        if (!IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void WalkingUpdate()
    {
        if (_currentState != _previousState)
        {
            _previousState = _currentState;
        }

        _anim.SetFloat("moveSpeed", .5f);
        _anim.SetBool("isClimbing", false);
        _stamina.ReplenishStamina();

        _currentSpeed = (_currentSpeed > _stats.MinSpeed) ? _currentSpeed - Time.deltaTime * _stats.SpeedScaler : _stats.MinSpeed;

        if (!IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Falling);
        }

        _audio.FootstepAudio(_audio.FootstepWalkingDelay);
    }

    public void RunningUpdate()
    {
        if (_currentState != _previousState)
        {
            _previousState = _currentState;
        }

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

        _audio.FootstepAudio(_audio.FootstepRunningDelay);
    }

    public void JumpingUpdate()
    {
        if (_currentState != _previousState)
        {
            _previousState = _currentState;
        }

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
        if (_currentState != _previousState)
        {
            _previousState = _currentState;
        }

        _anim.SetBool("isFalling", true);
        _currentJumpHeight -= Time.deltaTime * _stats.JumpHeightScaler;
        _audio.PlayFallingAudio();

        if (IsGrounded())
        {
            SetState(IPlayerState.PlayerState.Idle /*_currentJumpHeight < _stats.FallPower ? IPlayerState.PlayerState.BigImpact : IPlayerState.PlayerState.Idle*/);
            _playingLandingBop = true;
            _landingBopTimer = 0f;
            _audio.StopFallingAudio();
            _audio.PlayLandingAudio();
            //_currentJumpHeight = 0f;
        }
    }

    #region - CLIMBING -
    public void ClimbingUpdate()
    {
        _anim.SetBool("isFalling", false);
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

        if (ClimbDirection(out Vector3 climbDir, out Vector3 wallNormal) && !_isPullingPlayer)
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

        _audio.ClimbingAudio(_audio.ClimbingDelay);
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
        // Stop knockback if climbing
        if (_startedKnockback && _knockbackTime >= _stats.PreventKnockbackTimer)
        {
            _startedKnockback = false;
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
        if (_currentState != _previousState)
        {
            _previousState = _currentState;
        }

        // Init of state
        if (!_impact)
        {
            _flipTimer = 0f;
            _rotationDegree = (UnityEngine.Random.value) < 0.5f ? 360 : -360;
            _audio.BigImpactAudio();
        }

        _anim.SetBool("isFalling", true);

        _currentJumpHeight = 0f;
        _impact = true;

        _currentImpactHeight = Mathf.Max(_stats.MinImpactHeight, _currentImpactHeight);
        _currentImpactHeight = (_currentImpactHeight < _stats.MaxImpactHeight) ? _currentImpactHeight + Time.deltaTime * _stats.ImpactScaler : _stats.MaxImpactHeight;

        // Knockback movement if player was knocked back
        Vector3 knockbackVel = (_knockbackOnImpact) ? _previousVelocity : Vector3.zero;
        const float KnockbackScaler = 40f;

        // Move player upwards
        Vector3 verticalMovement = new Vector3(0f, _currentImpactHeight, 0f);
        Vector3 move = (verticalMovement + (knockbackVel / KnockbackScaler)) * _currentSpeed * Time.deltaTime;
        move.y = _currentImpactHeight * Time.deltaTime;
        _controller?.Move(move);

        _flipTimer += Time.deltaTime;

        // Rotate 360 degrees
        float degreesPerSecond = _rotationDegree / _flipDuration;
        float deltaRotation = degreesPerSecond * Time.deltaTime;
        Debug.Log("Rotate: " + _rotationDegree + ", persecond: " + degreesPerSecond + ", Delta: " + deltaRotation);
        transform.Rotate(Vector3.right * deltaRotation, Space.Self);

        if (_currentImpactHeight >= _stats.MaxImpactHeight && _flipTimer >= _flipDuration)
        {
            _currentImpactHeight = 0f;
            _impact = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }
    #endregion

    #region - COLLISION -
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
    #endregion

    #region - GRAB PLAYER -
    private bool _isPullingPlayer = false;
    private bool _grabbedPlayer = false;
    public bool GrabbedPlayed => _grabbedPlayer;
    public void PushPlayerUpdate()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(1) && _stamina.CurrentStamina > 0f)
        {
            _anim.SetTrigger("interact");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            const float RayDistance = 10f; // How far you want to check for hits

            Debug.DrawRay(ray.origin, ray.direction * RayDistance, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, RayDistance, _stats.PlayerLayer))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Player") && hitObject != gameObject)
                {
                    Debug.Log("[Client] Requesting push on: " + hitObject.name);
                    NetworkIdentity hitIdentity = hitObject.GetComponent<NetworkIdentity>();
                    if (hitIdentity != null)
                    {
                        _grabbedPlayer = true;
                        _stamina.DrainStamina(25f);
                        CmdGrab(hitIdentity);
                    }
                }
            }
        }
        else if(_grabbedPlayer)
        {
            _grabbedPlayer = false;
        }
    }

    [Command]
    private void CmdGrab(NetworkIdentity hitPlayer)
    {
        var player = hitPlayer.GetComponent<Player>();
        RpcPlayPunchAnimation();

        if (player != null)
        {
            Debug.Log("[Client] Pulling player toward grabber");

            // Pull direction: from the hit player *toward* this player
            Vector3 direction = (transform.position - hitPlayer.transform.position).normalized;

            // Send to the target client to apply pull
            player.RpcGrab(direction, _stats.GrabForce, _stats.GrabDuration, _stats.GrabUpwardForce); // force, duration, upward
        }
    }

    [TargetRpc]
    public void RpcGrab(Vector3 direction, float force, float duration, float upwardForce)
    {
        _anim.SetTrigger("interact");
        StartCoroutine(GrabCoroutine(direction, force, duration, upwardForce));
    }

    /// <summary>
    /// Play animation of punch on all clients
    /// </summary>
    [ClientRpc]
    private void RpcPlayPunchAnimation()
    {
        _anim.SetTrigger("interact");
    }

    private IEnumerator GrabCoroutine(Vector3 direction, float force, float duration, float upward)
    {
        _isPullingPlayer = true;
        float elapsed = 0f;
        CharacterController controller = GetComponent<CharacterController>();

        Vector3 move = direction * force + Vector3.up * upward;

        while (elapsed < duration)
        {
            controller.Move(move * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isPullingPlayer = false;
    }
    #endregion

    #region - KNOCKBACK -
    public void StartKnockback(Vector3 direction, float force, float duration, float upwardForce = 0f, bool stopKnockback = false)
    {
        if (_startedKnockback)
        {
            return;
        }

        // Set init values of the knockback
        _stopKnockbackOvertime = false;
        Vector3 finalDirection = direction.normalized + Vector3.up * upwardForce;
        _rotationDegree = (UnityEngine.Random.value) < 0.5f ? 360 : -360;
        StartCoroutine(KnockbackCoroutine(finalDirection, force, duration, stopKnockback));
    }

    /// <summary>
    /// Start knockback for the client
    /// </summary>
    [TargetRpc]
    public void RpcStartKnockback(Vector3 direction, float force, float duration, float upwardForce)
    {
        StartKnockback(direction, force, duration, upwardForce);
        _anim.SetTrigger("interact");
    }

    /// <summary>
    /// Movement logic of the knockback
    /// </summary>
    private void KnockbackUpdate()
    {
        if (_startedKnockback)
        {
            // Stop the knockback if player has hit the ground
            if (IsGrounded() && _stopKnockbackOvertime)
            {
                _startedKnockback = false;
                _previousVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector3.zero;

                if (_currentState == IPlayerState.PlayerState.BigImpact)
                {
                    _knockbackOnImpact = true;
                }
            }

            _controller?.Move(_knockbackVelocity * Time.deltaTime);

            float degreesPerSecond = _rotationDegree / _flipDuration;
            float deltaRotation = degreesPerSecond * Time.deltaTime;
            transform.Rotate(Vector3.right * deltaRotation, Space.Self);

            _knockbackTime += Time.deltaTime;
        }
        else
        {
            _knockbackTime = 0f;
        }
    }

    /// <summary>
    /// Stops the knockback over time
    /// </summary>
    private IEnumerator KnockbackCoroutine(Vector3 direction, float force, float duration, bool stopKnockback = false)
    {
        _startedKnockback = true;
        float timer = 0f;
        _knockbackVelocity = direction.normalized * force;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _stopKnockbackOvertime = true;

        if (stopKnockback)
        {
            _knockbackVelocity = Vector3.zero;
            _startedKnockback = false;
        }
    }
    #endregion

    #region - BOUNCE PLAYER -
    public void BouncePlayer()
    {
        _anim.SetBool("isJumping", true);

        _jumping = true;
        _currentJumpHeight = Mathf.Max(_stats.MinJumpHeight, _currentJumpHeight);
        _currentJumpHeight = (_currentJumpHeight < _stats.MaxBounceHeight) ? _currentJumpHeight + Time.deltaTime * _stats.BounceHeightScaler : _stats.MaxBounceHeight;

        _audio.PlayBouncingAudio();
        _particles.PlayGroundHitText();

        if (_currentJumpHeight >= _stats.MaxBounceHeight)
        {
            _jumping = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }
    #endregion

    #region - DEBUG -
    public void SkipToSkyscraper(GameObject skyscraper)
    {
        _controller.enabled = false;
        transform.position = skyscraper.transform.position;
        _controller.enabled = true;
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

    public void DrainStamina(float value)
    {
        _currentStamina -= value;
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
