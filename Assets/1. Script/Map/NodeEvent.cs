using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeEvent : MonoBehaviour, IPointerClickHandler
{
    private NodeDataSO _nodeData;
    private NodeData _mapNode;
    private int _row, _col;
    private bool _isSelectable = false;
    private bool _isVisited = false;

    [SerializeField] private GameObject _frame;
    private Image _nodeImage;

    public NodeData MapNode => _mapNode; //내가 가지고 있는 노드를 뿌리기
    private void Awake()
    {
        _nodeImage = GetComponent<Image>();
    }
    public void Setup(NodeDataSO data, NodeData mapData)
    {
        _nodeData = data;
        _mapNode = mapData;
        _row = mapData.Row;
        _col = mapData.Col;

        if (_nodeImage != null && _nodeData != null)
        {
            _nodeImage.sprite = _nodeData._nodeSprite;
        }
    }

    public void SetCurrentNode() //지금 현재 있는 곳 테두리 치기
    {
        if(_frame != null)
        {
            _frame.SetActive(true);
        }
        if (_nodeImage != null)
        {
            _nodeImage.color = Color.white;
        }
    }
    public void SetVisitedNode() //방문한거 색 바꾸기
    {
        _isVisited = true;
        HideFrame();
        if (_nodeImage != null)
        {
            _nodeImage.color = Color.darkCyan;
        }
    }
    public void HideFrame() => _frame?.SetActive(false);
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭 감지됨");

        if(!_isSelectable)
        {
            Debug.Log("이 노드는 선택할 수 없습니다.");
            return;
        }
        if (!MapManager._instance.CanMove)
        {
            Debug.LogWarning("전투 중에는 이동할 수 없습니다!");
            return;
        }
       
        ////if (!PhotonNetwork.IsMasterClient) return;
        ////MapManager._instance.CurrentRow = _row;
        ////MapManager._instance.CurrentCol = _col;
        //Debug.Log($"노드 클릭: {_nodeData._nodeName} at ({_row}, {_col})");

        //SetCurrentNode();
        //Debug.Log($"{gameObject.name}의 프레임 활성화 상태: {_frame.activeSelf}");

        //GetComponent<Image>().sprite = _nodeData._nodeSprite;
        //if (_nodeData != null)
        //{
        //    MapManager._instance.ExecuteEvent(_nodeData,this);
        //}
        MapManager._instance.photonView.RPC("RPC_ExecuteEventRequest", RpcTarget.MasterClient, _row, _col, _nodeData._sceneName);
    }
    public void SelectableNode(bool select)
    {
        if(_isVisited)
        {
            _frame.SetActive(false);
            return;
        }
        _isSelectable = select;
        if (_nodeImage != null)
        {
            _nodeImage.color = select ? Color.white : new Color(0.2f, 0.2f, 0.2f, 0.8f);
            _nodeImage.raycastTarget = select; // 선택 불가능하면 클릭 방지
        }
    }
}
