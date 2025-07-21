using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
    #region - FOOTSTEPS -
    [Header("Footsteps")]
    [SerializeField] private AudioSource _footsteps;
    [SerializeField] private float _footstepWalkingDelay = .5f;
    [SerializeField] private float _footstepRunningDelay = .25f;
    private float _currentFootstepDelay = 0f;

    public float FootstepWalkingDelay => _footstepWalkingDelay;
    public float FootstepRunningDelay => _footstepRunningDelay;
    public void FootstepAudio(float delay)
    {
        _currentFootstepDelay += Time.deltaTime;
        if (_currentFootstepDelay > delay)
        {
            _currentFootstepDelay = 0f;
            CmdPlayFootstepAudio();
        }
    }

    [Command]
    private void CmdPlayFootstepAudio()
    {
        RpcPlayFootstepAudio();
    }

    [ClientRpc]
    private void RpcPlayFootstepAudio()
    {
        _footsteps.Play();
    }
    #endregion

    #region - JUMPING -
    [Header("Jumping")]
    [SerializeField] private List<AudioSource> _jumpSources;

    public void JumpAudio()
    {
        CmdPlayJumpingAudio();
    }

    [Command]
    private void CmdPlayJumpingAudio()
    {
        RpcPlayJumpingAudio();
    }

    [ClientRpc]
    private void RpcPlayJumpingAudio()
    {
        var rand = Random.Range(0, _jumpSources.Count);
        _jumpSources[rand].Stop();
        _jumpSources[rand].Play();
    }
    #endregion

    #region - CLIMBING -
    [Header("Footsteps")]
    [SerializeField] private List<AudioSource> _climbingSources;
    [SerializeField] private float _climbingDelay = .5f;
    private float _currentClimbingDelay = 0f;

    public float ClimbingDelay => _footstepWalkingDelay;
    public void ClimbingAudio(float delay)
    {
        _currentClimbingDelay += Time.deltaTime;
        if (_currentClimbingDelay > delay)
        {
            _currentClimbingDelay = 0f;
            CmdPlayClimbingAudio();
        }
    }

    [Command]
    private void CmdPlayClimbingAudio()
    {
        RpcPlayClimbingAudio();
    }

    [ClientRpc]
    private void RpcPlayClimbingAudio()
    {
        if (_climbingSources.Count == 0) return;

        int rand = Random.Range(0, _climbingSources.Count);
        _climbingSources[rand].Play();
    }
    #endregion
}
