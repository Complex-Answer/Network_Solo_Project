using UnityEngine;

[CreateAssetMenu(fileName = "NodeDataSO", menuName = "Scriptable Objects/MapDataSo")]
public class NodeDataSO : ScriptableObject
{
    public string _nodeName; // 노드 이름
    public Sprite _nodeSprite; //노드 이미지
    public Color _iconColor = Color.white; //노드 아이콘 색상
    public float _weight = 10f; //노드 가중치
    public int _maxCount = 1;//노드 최대 생성 개수
}
