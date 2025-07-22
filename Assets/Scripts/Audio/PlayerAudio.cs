using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

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

    #region - LANDING -
    [Header("Footsteps")]
    [SerializeField] private AudioSource _landingSource;
    public void PlayLandingAudio()
    {
        CmdPlayLandingAudio();
    }

    [Command]
    private void CmdPlayLandingAudio()
    {
        RpcPlayLandingAudio();
    }

    [ClientRpc]
    private void RpcPlayLandingAudio()
    {
        _landingSource.Play();
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

    #region - CAR SMASH -
    [Header("Jumping")]
    [SerializeField] private AudioSource _carSmash;

    public void CarSmashAudio()
    {
        CmdCarSmashAudio();
    }

    [Command]
    private void CmdCarSmashAudio()
    {
        RpcCarSmashAudio();
    }

    [ClientRpc]
    private void RpcCarSmashAudio()
    {
        _carSmash.Stop();
        _carSmash.Play();
    }
    #endregion

    #region - BIG IMPACT -
    [Header("Jumping")]
    [SerializeField] private AudioSource _bigImpactSource;

    public void BigImpactAudio()
    {
        CmdBigImpactAudio();
    }

    [Command]
    private void CmdBigImpactAudio()
    {
        RpcBigImpactAudio();
    }

    [ClientRpc]
    private void RpcBigImpactAudio()
    {
        _bigImpactSource.Stop();
        _bigImpactSource.Play();
    }
    #endregion

    #region - FALLING -
    [Header("Falling")]
    [SerializeField] private AudioSource _fallingSource;

    public void PlayFallingAudio()
    {
        if (!_fallingSource.isPlaying)
        {
            _fallingSource.Play();
        }
        _fallingSource.volume += Time.deltaTime * .5f;
    }

    public void StopFallingAudio()
    {
        if (_fallingSource.isPlaying)
        {
            StartCoroutine(MuteFallingAudio());
        }
    }

    private IEnumerator MuteFallingAudio()
    {
        float startVolume = _fallingSource.volume;

        while (_fallingSource.volume > 0f)
        {
            _fallingSource.volume -= startVolume * Time.deltaTime / 0.5f;
            yield return null;
        }

        _fallingSource.volume = 0f;
        _fallingSource.Stop();
    }
    #endregion

    #region - BOUNCING -
    [Header("Jumping")]
    [SerializeField] private List<AudioSource> _bouncingSources;

    public void PlayBouncingAudio()
    {
        CmdPlayBouncingAudio();
    }

    [Command]
    private void CmdPlayBouncingAudio()
    {
        RpcPlayBouncingAudio();
    }

    [ClientRpc]
    private void RpcPlayBouncingAudio()
    {
        var rand = Random.Range(0, _bouncingSources.Count);
        _bouncingSources[rand].Stop();
        _bouncingSources[rand].Play();
    }
    #endregion
}
