using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeEvent : MonoBehaviour, IPointerClickHandler
{
    private NodeDataSO _nodeData;
    private NodeData _mapNode;
    private int _row, _col;
    private bool _isSelectable = false;

    [SerializeField] private GameObject _frame;
    private Image _nodeImage;

    private void Awake()
    {
        _nodeImage = GetComponent<Image>();
    }

    public void SetCurrentNode()
    {
        if(_frame != null)
        {
            _frame.SetActive(true);
        }
    }
    public void SetVisitedNode()
    {
        if(_frame!= null)
        {
            _frame.SetActive(false);
        }
        if(_nodeImage != null)
        {
            _nodeImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
    public void Setup(NodeDataSO data, NodeData mapData)
    {
        _nodeData = data;
        _mapNode = mapData;
        _row = mapData.Row;
        _col = mapData.Col;
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        if(!_isSelectable)
        {
            Debug.Log("이 노드는 선택할 수 없습니다.");
            return;
        }
        MapManager._instance.CurrentRow = _row;
        MapManager._instance.CurrentCol = _col;


        Debug.Log($"노드 클릭: {_nodeData._nodeName} at ({_row}, {_col})");

        SetCurrentNode();
        Debug.Log($"{gameObject.name}의 프레임 활성화 상태: {_frame.activeSelf}");

        if (_mapNode != null && _mapNode.NextStairs != null)
        {
            // 매니저에 이 리스트를 보관하는 함수가 필요함
            MapManager._instance.SetNextNode(_mapNode.NextStairs);
        }

        GetComponent<Image>().sprite = _nodeData._nodeSprite;
        if (_nodeData != null)
        {
            MapManager._instance.ExecuteEvent(_nodeData,this);
        }
    }
    public void SelectableNode(bool select)
    {
        _isSelectable = select;
        _nodeImage.color = select ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.8f);
        _nodeImage.raycastTarget = select;
    }
}
