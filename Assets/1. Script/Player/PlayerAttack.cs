using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerManager _player;
    InputAction _input;

    bool _timer = false;
    float _attSpeed = 0;
    float _delayTime = 0.3f;
    void Awake()
    {
        _player = GetComponent<PlayerManager>();
        _input = InputSystem.actions["Attack"];
    }
    private void Start()
    {
        _attSpeed = _player.AttackSpeed/1.1f;
        _input.performed += OnAttack;
        _input.canceled += ctx => //입력 뗄 때
        {
            //_isAttacking = false;
            //isUse = false;
        };

    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}
        if (!_timer)
        {
            Debug.Log(_timer);
        }

        if (_timer)
        {
            return;
        }
        _timer = true;
        //_isAttacking = true; //공격 트리거용 변수
        Debug.Log("공격");
        StartCoroutine(AttackSpeed()); //공격속도 딜레이
    }
    IEnumerator AttackSpeed() //공격 속도 딜레이 코루틴
    {
        yield return new WaitForSeconds(_attSpeed);
        _timer = false;
    }

    private void OnDestroy()
    {
        _input.performed -= OnAttack;
    }
}
