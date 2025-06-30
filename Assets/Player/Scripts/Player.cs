using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : NetworkBehaviour, IPlayerState
{
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
    public void SetState(IPlayerState.PlayerState state)
    {
        if(state == _currentState)
        {
            return;
        }

        _previousState = _currentState;
        _currentState = state;
    }

    private CharacterController _controller;

    [Header("Camera")]
    [SerializeField] private Camera _camPrefab;
    [SerializeField] private Transform _camPosition;
    private Camera _cam;

    [Header("Movement")]
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _speedScaler = 10f;
    [SerializeField] private bool _running = false;

    [Header("Jumping")]
    [SerializeField] private bool _jumping = false;
    [SerializeField] private float _currentJumpHeight = 0f;
    [SerializeField] private float _minJumpHeight = 5f;
    [SerializeField] private float _maxJumpHeight = 10f;
    [SerializeField] private float _jumpHeightScaler = 10f;

    [Header("Climbing")]
    [SerializeField] private float _climbSpeed = 3f;

    [Header("Rotation")]
    [SerializeField] private float _sensitivity = 10f;
    [SerializeField, Range(0f, 1f)] private float _smoothFactor = 0.5f;
    private float _verticalRot;
    private float _horizontalRot;

    [Header("Misc")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _controller = GetComponent<CharacterController>();

            _cam = Instantiate(_camPrefab, _camPosition);
            _cam.transform.localPosition = Vector3.zero;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        StateUpdate();
        RotateUpdate();
        MovementUpdate();
    }

    public void StateUpdate()
    {
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
        }
    }

    private void MovementUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (CanClimb() && Input.GetMouseButton(0))
        {
            SetState(IPlayerState.PlayerState.Climbing);
        }
        else if (Input.GetMouseButtonUp(0) && _currentState == IPlayerState.PlayerState.Climbing)
        {
            SetState(IPlayerState.PlayerState.Falling);
        }

        if (_currentState != IPlayerState.PlayerState.Climbing)
        {
            if (!IsGrounded() && !_jumping)
            {
                SetState(IPlayerState.PlayerState.Falling);
            }

            // Rotate player to camera rot
            Quaternion facingDirection = Quaternion.Euler(0, _cam.transform.rotation.eulerAngles.y, 0);
            transform.rotation = facingDirection;

            // Move player
            Vector3 playerMovement = new Vector3(h * .25f, _currentJumpHeight, v * .25f);
            Vector3 move = (facingDirection * playerMovement) * _currentSpeed * Time.deltaTime;
            move.y = _currentJumpHeight * Time.deltaTime;
            _controller?.Move(move);

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                SetState(IPlayerState.PlayerState.Jumping);
            }
            else if (IsGrounded() && _currentJumpHeight == 0)
            {
                if (playerMovement.magnitude == 0)
                {
                    SetState(IPlayerState.PlayerState.Idle);
                    _running = false;
                }
                else if (_currentState != IPlayerState.PlayerState.Running)
                {
                    SetState((_running) ? IPlayerState.PlayerState.Running : IPlayerState.PlayerState.Walking);
                }
            }
        }
    }

    private void RotateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update vertical and horizontal rotation
        _verticalRot -= mouseY * _sensitivity;
        _verticalRot = Mathf.Clamp(_verticalRot, -70f, 70);
        _horizontalRot += mouseX * _sensitivity;

        // Rotate camera to mouse position
        float smoothedVerticalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.x, _verticalRot, _smoothFactor);
        float smoothedHorizontalRot = Mathf.LerpAngle(_cam.transform.eulerAngles.y, _horizontalRot, _smoothFactor);
        _cam.transform.rotation = Quaternion.Euler(smoothedVerticalRot, smoothedHorizontalRot, 0f);
    }

    public void IdleUpdate()
    {
        _currentSpeed = 0f;
    }

    public void WalkingUpdate()
    {
        _currentSpeed = (_currentSpeed > _minSpeed) ? _currentSpeed - Time.deltaTime * _speedScaler : _minSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _running = true;
            SetState(IPlayerState.PlayerState.Running);
        }
    }

    public void RunningUpdate()
    {
        _currentSpeed = (_currentSpeed < _maxSpeed) ? _currentSpeed + Time.deltaTime * _speedScaler : _maxSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _running = false;
            SetState(IPlayerState.PlayerState.Walking);
        }
    }

    public void JumpingUpdate()
    {
        _jumping = true;
        _currentJumpHeight = Mathf.Max(_minJumpHeight, _currentJumpHeight);
        _currentJumpHeight = (_currentJumpHeight < _maxJumpHeight) ? _currentJumpHeight + Time.deltaTime * _jumpHeightScaler : _maxJumpHeight;

        if(_currentJumpHeight >= _maxJumpHeight)
        {
            _jumping = false;
            SetState(IPlayerState.PlayerState.Falling);
        }
    }

    public void FallingUpdate()
    {
        //_currentJumpHeight = Mathf.Min(-_maxJumpHeight, _currentJumpHeight);
        _currentJumpHeight = (_currentJumpHeight > -_maxJumpHeight) ? _currentJumpHeight - Time.deltaTime * _jumpHeightScaler : -_maxJumpHeight;

        if (IsGrounded())
        {
            _currentJumpHeight = 0f;
            SetState(IPlayerState.PlayerState.Idle);
        }
    }

    public void ClimbingUpdate()
    {
        // Make player jump to position looking at
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }

        if (ClimbDirection(out Vector3 climbDir, out Vector3 wallNormal))
        {
            // Get direction vectors based on wall
            Vector3 climbUp = climbDir;
            Vector3 wallRight = Vector3.Cross(wallNormal, climbUp).normalized;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                move += climbUp;
            if (Input.GetKey(KeyCode.S))
                move -= climbUp;
            if (Input.GetKey(KeyCode.D))
                move += wallRight;
            if (Input.GetKey(KeyCode.A))
                move -= wallRight;

            if (move != Vector3.zero)
            {
                Vector3 climbVector = move.normalized * _climbSpeed * Time.deltaTime;
                _controller.Move(climbVector);

                Debug.DrawRay(transform.position, climbVector, Color.cyan);
            }
        }
    }

    public bool CanClimb()
    {
        const float ClimbCheckDistance = 2f;

        if (Physics.Raycast(transform.position, transform.forward, ClimbCheckDistance, _wallLayer))
        {
            return true;
        }
        return false;
    }

    public bool ClimbDirection(out Vector3 climbDirection, out Vector3 wallNormal)
    {
        const float ClimbCheckDistance = 2f;
        climbDirection = Vector3.zero;
        wallNormal = Vector3.zero;

        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Vector3 bottomRayOrigin = transform.position - Vector3.up * 2f;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * ClimbCheckDistance, Color.blue);

        if (Physics.Raycast(bottomRayOrigin, rayDirection, out RaycastHit hit, ClimbCheckDistance, _wallLayer))
        {
            wallNormal = hit.normal;

            Vector3 wallRight = Vector3.Cross(wallNormal, Vector3.up).normalized;
            climbDirection = Vector3.Cross(wallRight, wallNormal).normalized;

            Debug.DrawRay(hit.point, wallNormal, Color.red);
            Debug.DrawRay(hit.point, wallRight, Color.yellow);
            Debug.DrawRay(hit.point, climbDirection, Color.green);

            return true;
        }

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitBottom, ClimbCheckDistance, _wallLayer))
        {
            wallNormal = hitBottom.normal;

            Vector3 wallRight = Vector3.Cross(wallNormal, Vector3.up).normalized;
            climbDirection = Vector3.Cross(wallRight, wallNormal).normalized;

            Debug.DrawRay(hitBottom.point, wallNormal, Color.red);
            Debug.DrawRay(hitBottom.point, wallRight, Color.yellow);
            Debug.DrawRay(hitBottom.point, climbDirection, Color.green);

            return true;
        }

        SetState((_running) ? IPlayerState.PlayerState.Running : IPlayerState.PlayerState.Walking);
        return false;
    }

    public bool IsGrounded()
    {
        const float GroundCheckDistance = 1.5f;
        Debug.DrawRay(transform.position, Vector3.down * GroundCheckDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, GroundCheckDistance, _groundLayer))
        {
            return true;
        }
        return false;
    }
}
