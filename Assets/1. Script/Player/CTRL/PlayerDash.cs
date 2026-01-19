using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    InputAction _input;
    PlayerManager _player;

    [SerializeField] private float _dashForce = 50; //대쉬 힘
    [SerializeField] private float _dashTime = 0.1f; //코루틴에서 사용할 대쉬 지속 시간

    float _dashDelay = 0.5f;
    bool _isDash = false;
    bool _canDash = false;

    WaitForSeconds DashDelay;
    WaitForSeconds DashTime;

    public bool IsDash => _isDash;


    private void Awake()
    {
        _player = GetComponent<PlayerManager>();

        _input = InputSystem.actions["Dash"];
    }

    void Start()
    {
        

        DashDelay = new WaitForSeconds(_dashDelay);
        DashTime = new WaitForSeconds(_dashTime);

        _input.performed += OnDash;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.phase);
        if (_canDash)
        {
            return;
        }
        //if (_player.Stamina <= 0)
        //{
        //    return;
        //}
        if (ctx.performed && ctx.ReadValue<float>() > 0.1f)
        {
            if (_player.PlayerMove.Move != Vector3.zero)
            {
                StartCoroutine(Dash());
            }
        }
    }
    IEnumerator Dash()
    {
        _isDash = true;
        _canDash = true;

        //_player.UseStamina();
        _player.Rb.linearVelocity = new Vector3(_player.PlayerMove.Move.x * _dashForce, 0, _player.PlayerMove.Move.y * _dashForce);
        yield return DashTime;
        _player.Rb.linearVelocity = Vector3.zero;

        _isDash = false;
        yield return DashDelay;
        _canDash = false;
        //else
        //{
        //    _isDash = false;
        //}
    }
}
