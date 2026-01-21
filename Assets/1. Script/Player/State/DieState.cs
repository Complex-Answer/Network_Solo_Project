using UnityEngine;

public class DieState : IState
{
    private PlayerManager _player;

    public DieState(PlayerManager player)
    {
        _player = player;
    }
    public void Enter()
    {
        if (_player.Animator != null)
        {
            _player.Animator.SetTrigger("Die");
        }
        if(_player.Rb != null)
        {
            _player.Rb.linearVelocity = Vector3.zero;
        }
        if(GameManager._instance != null)
        {
            GameManager._instance.EndBattle(false);
        }
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}
