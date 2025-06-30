public interface IPlayerState
{
    [System.Serializable]
    public enum PlayerState
    {
        Idle = 0,
        Walking,
        Running,
        Jumping,
        Falling,
        Climbing,
        ENDOFSTATES
    }

    public PlayerState CurrentState { get; set; }
    public PlayerState PreviousState { get; set; }
    public void SetState(PlayerState state);

    public void StateUpdate();
    public void IdleUpdate();
    public void WalkingUpdate();
    public void RunningUpdate();
    public void JumpingUpdate();
    public void FallingUpdate();
    public void ClimbingUpdate();
}
