using UnityEngine;

public class MonsterProxy : MonoBehaviour
{
    private BasicMonster _parents; // 신호를 전달할 부모 스크립트 보관함

    void Awake()
    {
        _parents = GetComponentInParent<BasicMonster>();

        if (_parents == null)
        {
            Debug.LogError($"{gameObject.name}의 부모에서 BasicMonster를 찾을 수 없습니다!");
        }
    }

    public void OnAttackHit()
    {
        if (_parents != null)
        {
            _parents.OnAttackHit();
        }
    }
}
