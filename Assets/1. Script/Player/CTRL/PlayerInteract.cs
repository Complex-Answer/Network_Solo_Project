using UnityEngine;
/// <summary>
/// 상호작용과 관련된 기본 클래스
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    PlayerManager _player;
    IInteractable _target;

    [SerializeField] private float _interactRange = 2.0f; // 상호작용 범위
    [SerializeField] private LayerMask _interactLayer; // 상호작용 레이어



    private void Awake()
    {
        _player = GetComponent<PlayerManager>();
    }
    public void DoInteract()
    {
        if(_target != null) //타겟이 null이 아니면
        {
            _target.OnInteract(_player); //상호작용 실행
        }
    }

    public void Interact()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, _interactRange,_interactLayer);
        // 플레이어 주변의 콜라이더 탐색 _interactRange 반경 내에서 _interactLayer 레이어에 속한 콜라이더들만 탐색
        IInteractable closest = null; //가장 가까운 상호작용 대상

        float closestDist = Mathf.Infinity; //가장 가까운 상호작용 대상과의 거리

        foreach (var col in cols)
        {
            if(col.TryGetComponent<IInteractable>(out IInteractable interactable)) // 콜라이더가 IInteractable 인터페이스를 구현하는지 확인
            {
                float dist = Vector3.Distance(transform.position, col.transform.position); //플레이어와 상호작용 대상과의 거리 계산
                
                if(dist < closestDist) //만약 지금까지 찾은 가장 가까운 거리보다 작다면
                {
                    closestDist = dist; //이제 dist가 최소 거리임
                    closest = interactable; //가장 가까운 상호작용 대상 갱신
                }
            }
        }
        //만약 closest가 null이 아니면
        if (_target != closest) //타겟이 변경되었는지 확인
        {
            _target = closest; //타겟 갱신
            //여기에 UI띄우고 어쩌고 저쩌고
            if(_target != null)
            {
                //타겟이 null이 아니면 UI띄우기
            }
            else
            {
                //타겟이 null이면 UI끄기
            }
        }
    }
}
