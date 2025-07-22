using DG.Tweening;
using Mirror;
using System.Collections;
using UnityEngine;

public class PlayerParticles : NetworkBehaviour
{
    private Player _player;

    [Header("Positions")]
    [SerializeField] private Transform _groundHit;

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player == null)
        {
            return;
        }
        PlayGroundHit();
    }

    #region - GROUND HIT -
    public void PlayGroundHit()
    {
        if (_player.CurrentState != _player.PreviousState &&_player.CurrentState == IPlayerState.PlayerState.BigImpact)
        {
            CmdGroundHit();
        }
    }

    [Command]
    private void CmdGroundHit()
    {
        RpcGroundHit();
    }

    [ClientRpc]
    private void RpcGroundHit()
    {
        DOVirtual.DelayedCall(.1f, () =>
        {
            ParticlePooler.Instance.PlayParticle(ParticlePooler.ParticleType.GroundHit, _groundHit.position);
        }).OnComplete( () => 
        {
            DOVirtual.DelayedCall(.1f, () =>
            {
                ParticlePooler.Instance.PlayParticle(ParticlePooler.ParticleType.GroundHitText, _groundHit.position);
            });
        });
    }
    #endregion

    #region - GROUND HIT TEXT -
    public void PlayGroundHitText()
    {
        CmdGroundHitText();
    }

    [Command]
    private void CmdGroundHitText()
    {
        RpcGroundHitText();
    }

    [ClientRpc]
    private void RpcGroundHitText()
    {
        ParticlePooler.Instance.PlayParticle(ParticlePooler.ParticleType.GroundHitText, _groundHit.position);
    }
    #endregion
}
