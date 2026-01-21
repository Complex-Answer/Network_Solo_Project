using UnityEngine;

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
        if (_player.PlayerDash.IsDash)
        {
            return;
        }
        _player.Rb.linearVelocity = new Vector3(_move.x * _player.MoveSpeed, 0,_move.y * _player.MoveSpeed);
    }
    
}
