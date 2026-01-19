using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerManager _player;
    InputAction _input;

    bool _timer = false;
    int _lastIndex = -1; //밑에서 저장할 인덱스 저장용
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
        if (!ctx.performed || _timer)
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
        int currentIndex = Random.Range(0, 2);

        if(currentIndex == _lastIndex)
        {
            currentIndex = (currentIndex == 0) ? 1 : 0; //현재 인덱스가 0이면 1로 변경
        }
        _lastIndex = currentIndex; //마지막 인덱스를 저장

        string attackName = (currentIndex == 0) ? "Attack01" : "Attack02"; //이제 어택01이랑 어택02를 골라줌
        _player.Animator.CrossFade(attackName, 0.1f, 1);
     
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
