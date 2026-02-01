using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviourPun
{
    PlayerManager _player;
    [SerializeField] Transform _firePoint;
    public GameObject _ballPrefab;
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
        if (_isAttack) //중복 실행 불가
        {
            return;
        }

        _isAttack = true;

        int currentIndex = Random.Range(0, 2);

        if (currentIndex == _lastIndex)
        {
            currentIndex = (currentIndex == 0) ? 1 : 0; //현재 인덱스가 0이면 1로 변경
        }
        _lastIndex = currentIndex; //마지막 인덱스를 저장

        _player.Animator.SetInteger("AttackIndex", currentIndex);
        _player.Animator.SetTrigger("OnAttack");

        StartCoroutine(AttackSpeed()); //공격속도 딜레이
    }
    IEnumerator AttackSpeed() //공격 속도 딜레이 코루틴
    {
        _attSpeed = 1f / Mathf.Max(0.1f, _player.AttackSpeed); //최대 공격속도 지정해주기

        yield return new WaitForSeconds(_attSpeed);
        _isAttack = false;
    }
    public void OnFire()
    {
        // 내꺼일때만 투사체 생성
        if (!photonView.IsMine) return;

        // 생성 위치
        _firePoint.GetPositionAndRotation(out Vector3 spawnPos, out Quaternion spawnRot);

        //투사체 생성
        GameObject magic = PhotonNetwork.Instantiate(_ballPrefab.name, spawnPos, transform.rotation);

        // 3. 데미지 주입
        PlayerProjectile proj = magic.GetComponent<PlayerProjectile>();
        if (proj != null)
        {

        }
    }
}
