using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviourPun
{
    PlayerManager _player;

    bool _isAttack = false;
    int _lastIndex = -1; //밑에서 저장할 인덱스 저장용
    float _attSpeed = 0;

    void Awake()
    {
        _player = GetComponent<PlayerManager>();
    }
   
    public void OnAttack()
    {
        if (!_player.photonView.IsMine)
        {
            return;
        }
        if ( _isAttack) //중복 실행 불가
        {
            return;
        }

        int currentIndex = Random.Range(0, 2);

        if (currentIndex == _lastIndex)
        {
            currentIndex = (currentIndex == 0) ? 1 : 0; //현재 인덱스가 0이면 1로 변경
        }
        _lastIndex = currentIndex; //마지막 인덱스를 저장

        StartCoroutine(AttackSpeed()); //공격속도 딜레이
        _player.photonView.RPC("RPC_Attack", RpcTarget.All, currentIndex); //위에서 랜덤으로 지정된 인덱스들을 RPC로 쏴주기
    }

    [PunRPC]
    private void RPC_Attack(int index)
    {
        _isAttack = true;

        string attackName = (index == 0) ? "Attack01" : "Attack02"; //이제 어택01이랑 어택02를 골라줌
        _player.Animator.CrossFade(attackName, 0.1f, 1);

        //_isAttacking = true; //공격 트리거용 변수
    }
    IEnumerator AttackSpeed() //공격 속도 딜레이 코루틴
    {
        _attSpeed = 1f / Mathf.Max(0.1f, _player.AttackSpeed); //최대 공격속도 지정해주기

        yield return new WaitForSeconds(_attSpeed);
        _isAttack = false;
    }
}
