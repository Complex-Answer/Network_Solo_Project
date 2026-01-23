using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{
    PlayerManager _player;

    Vector3 _move;

    public Vector3 Move => _move;

    private void Awake()
    {
        _player = GetComponent<PlayerManager>();
    }
    public void SetMoveInput(Vector2 dir)
    {
        _move = new Vector3(dir.x, dir.y).normalized;
    }

    void FixedUpdate()
    {
        if (!_player.PlayerDash.IsDash)
        {
            _player.Rb.linearVelocity = new Vector3(_move.x * _player.MoveSpeed, 0, _move.y * _player.MoveSpeed);
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(_player.Rb.position, out hit, 1.2f, NavMesh.AllAreas))
        {
            float distance = Vector3.Distance(_player.Rb.position, hit.position);

            if (distance > 0.1f)
            {
                _player.Rb.position = hit.position; // 위치를 즉시 안으로 고정

                // 대시 중이라면 속도를 0으로 만들어 뚫고 나가는 힘을 제거
                if (_player.PlayerDash.IsDash)
                {
                    _player.Rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
    
}
