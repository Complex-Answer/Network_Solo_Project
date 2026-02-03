using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

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

        _player.IsDead = true;

        if (_player.photonView.IsMine)
        {
            Debug.Log($"[GameManager] 1번 플레이어 생존 상태 변경: False");
            _player.ReportDeath();
        }

        _player.gameObject.tag = "Untagged";

        if (_player.Rb != null)
        {
            _player.Rb.linearVelocity = Vector3.zero;
        }
        var input = _player.GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
            Debug.Log("플레이어 입력이 차단되었습니다.");
        }

        if (BattleManager._instance != null)
        {
            BattleManager._instance.RemovePlayer(_player);
        }

        _player.DestroySelfDelayed(2.0f);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}
