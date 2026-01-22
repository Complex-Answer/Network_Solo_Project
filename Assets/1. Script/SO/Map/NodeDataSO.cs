using UnityEngine;
public enum NodeType
{
    Combat,
    Rest,
    Event,
    Shop,
    Box,
    Boss
}
[CreateAssetMenu(fileName = "NodeDataSO", menuName = "Scriptable Objects/NodeDataSOd")]
public class NodeDataSO : ScriptableObject
{
    public string _nodeName; // 노드 이름
    public Sprite _nodeSprite; //노드 이미지
    public NodeType _nodeType; //노드 타입
    public string _sceneName; //노드와 연결된 씬 이름
    public Color _iconColor = Color.white; //노드 아이콘 색상
    public float _weight = 10f; //노드 가중치
    public int _maxCount = 1;//노드 최대 생성 개수

    
}
