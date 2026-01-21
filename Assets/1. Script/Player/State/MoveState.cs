using UnityEngine;

public class MoveState : IState
{
    PlayerManager _player;
    public MoveState(PlayerManager player)
    {
        _player = player;
    }


    public void Enter()
    {
        _player.Animator.CrossFade("DashTree", 0.2f);
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        _player.Animator.SetFloat("DirX", _player.PlayerMove.Move.x, 0.1f, Time.deltaTime);
        _player.Animator.SetFloat("DirY", _player.PlayerMove.Move.y, 0.1f,Time.deltaTime);

        if (_player.PlayerDash.IsDash)
        {
            _player.MoveState(new DashState(_player));
        }
    }
}
