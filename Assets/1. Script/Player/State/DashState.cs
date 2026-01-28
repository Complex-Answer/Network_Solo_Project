using UnityEngine;

public class DashState : IState
{
    PlayerManager _player;
    public DashState(PlayerManager player)
    {
        _player = player;
    }
    public void Enter()
    {
        _player.Animator.SetTrigger("OnDash");

        _player.Animator.SetFloat("DirX", _player.PlayerMove.Move.x, 0.1f, Time.deltaTime);
        _player.Animator.SetFloat("DirY", _player.PlayerMove.Move.y, 0.1f, Time.deltaTime);
    }

    public void Exit()
    {
    }

    public void Update()
    {
        if (!_player.PlayerDash.IsDash)
        {
            _player.MoveState(new MoveState(_player));
        }
    }
}
