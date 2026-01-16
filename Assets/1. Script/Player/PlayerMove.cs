using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    PlayerManager _player;
    InputAction _input;


    Vector3 _dir;
    Vector3 _move;

    public Vector3 Move => _move;

    private void Awake()
    {
        _player = GetComponent<PlayerManager>();
        _input = InputSystem.actions["Move"];
    }
    void Start()
    {
        _input.performed += OnMove;
        _input.canceled += OnStopped;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _dir = ctx.ReadValue<Vector2>();
        _move = new Vector3(_dir.x,_dir.y).normalized;
    }
    private void OnStopped(InputAction.CallbackContext ctx)
    {
        _move = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (_player.PlayerDash.IsDash)
        {
            return;
        }
        _player.Rb.linearVelocity = new Vector3(_move.x * _player.MoveSpeed, 0,_move.y * _player.MoveSpeed);
    }
    private void OnDestroy()
    {
        _input.actionMap["Move"].performed -= OnMove;
        _input.actionMap["Move"].canceled -= OnStopped;
    }
}
