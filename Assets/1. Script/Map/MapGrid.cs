using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NodeData
{
    int row, col;
    Vector2 position;
    List<NodeData> _next = new();

    public int Row
    {
        get { return row; }
        set { row = value; }
    }
    public int Col
    {
        get { return col; }
        set { col = value; }
    }
    public Vector2 Position { get; set; }
    public List<NodeData> NextStairs => _next;
    public NodeDataSO NodeType { get; set; }
    public bool HasPosition => Position != Vector2.zero;
}
public class MapGrid : MonoBehaviourPun
{
    MyPlayerInput _inputActions;

    [SerializeField] private LineDraw _drawLine;
    [SerializeField] private CreatPath _creatPath;
    [SerializeField] GameObject _nodePerfab;
    [SerializeField] MapDataSO _mapData;

    public RectTransform _rectTransform;

    [SerializeField] int _row = 15;
    [SerializeField] int _col = 20;
    [SerializeField] float errorValue = 0.3f; //맵 오차 값
    int _mapSeed;

    private MapManager _manager;

    private List<NodeData>[] _nodeConnection;
    private Dictionary<NodeData, NodeEvent> _nodeEvents = new();
    //노드별 생성 개수 체크용
    private Dictionary<NodeDataSO, int> _nodeCount = new();
    //이전 노드 개수 저장용(노드 연속 생성 방지)
    private Dictionary<NodeDataSO, int> _lastNodeCount = new();
    private void Awake()
    {
        _inputActions = new MyPlayerInput();
    }
    private void Start()
    {
        _manager = MapManager._instance;
        if (_manager == null)
        {
            Debug.LogError("맵 매니저를 찾을 수 없습니다");
            return;
        }

        if (_manager.SavedMapData != null)
        {
            _nodeConnection = _manager.SavedMapData;
            DrawNode();
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_RequestMapState", RpcTarget.MasterClient);
            }
            else
            {
                // 방장은 이미 자기 데이터를 알고 있으니 바로 그립니다.
                UpdateMapVisuals();
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int newSeed = Random.Range(0, 1000000);
                photonView.RPC("RPC_SyncSeed", RpcTarget.All, newSeed);
            }
        }
    }
    public void CreateMapWithSeed(int seed)
    {
        if (_nodeConnection != null) return;

        _mapSeed = seed;
        Random.InitState(seed);

        GridNode();

        if (_nodeConnection != null && _drawLine != null)
        {
            _drawLine.DrawLine(_rectTransform, _nodeConnection);
            RestoreMapVisuals();
            RefreshMapUI();
        }

        _manager.SavedMapData = _nodeConnection;
    }
    [PunRPC]
    public void RPC_RequestMapState()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 방장의 매니저에 들어있는 '진짜' 위치 정보를 모두에게 쏴줍니다.
            photonView.RPC("RPC_ForceUpdateMapState", RpcTarget.All, _manager.CurrentRow, _manager.CurrentCol);
        }
    }

    [PunRPC]
    public void RPC_ForceUpdateMapState(int row, int col)
    {
        _manager.CurrentRow = row;
        _manager.CurrentCol = col;

        // 데이터가 갱신됐으니 이제 비주얼을 다시 그립니다.
        UpdateMapVisuals();
    }
    [PunRPC]
    public void RPC_SyncSeed(int seed)
    {
        if (_nodeConnection != null) return; // 이미 생성됐다면 무시

        Debug.Log($"[MapGrid] 직접 수신된 시드: {seed}");

        _mapSeed = seed;

        foreach (Transform child in _rectTransform) { Destroy(child.gameObject); }
        Random.InitState(seed);

        GridNode();


        _manager.SavedMapData = _nodeConnection;
        UpdateMapVisuals();
    }

    private void UpdateMapVisuals()
    {
        if (_nodeConnection != null && _drawLine != null)
        {
            _drawLine.DrawLine(_rectTransform, _nodeConnection);
            RestoreMapVisuals();
            RefreshMapUI();
        }
    }

    private void GridNode()
    {
        //맵 만들 때 기존과 겹치치 않게 싹 지워버리기
        foreach (Transform child in _rectTransform)
        {
            Destroy(child.gameObject);
        }

        _nodeConnection = new List<NodeData>[_row];
        for (int i = 0; i < _row; i++) _nodeConnection[i] = new List<NodeData>();
        //경로를 먼저 만들고
        _creatPath.PathCreat(_nodeConnection, _row, _col);
        //거기에 노드 이미지를 그리고
        DrawNode();
    }
    //경로를 기반으로 노드를 배치
    private void DrawNode()
    {
        float cellWidth = _rectTransform.rect.width / _col;
        float cellHeight = _rectTransform.rect.height / _row;

        //맵 새로 만들 때 초기화
        _nodeCount.Clear();
        _lastNodeCount.Clear();
        _nodeEvents.Clear();
        for (int r = 0; r < _row; r++)
        {

            foreach (var nodeData in _nodeConnection[r])
            {
                if (!nodeData.HasPosition)
                {
                    nodeData.Row = r;
                    //노드들을 격자 형태로 배치
                    float xPos = nodeData.Col * cellWidth + (cellWidth / 2);
                    float yPos = r * cellHeight + (cellHeight / 2);

                    float xOffset = Random.Range(-cellWidth * errorValue, cellWidth * errorValue);
                    float yOffset = Random.Range(-cellHeight * errorValue * 0.1f, cellHeight * errorValue * 0.1f);

                    //UI는 0,0이 중심이라 0.5 정도를 뺴줘야 자연스럽게 보임
                    Vector2 pos = new((xPos - _rectTransform.rect.width / 2), yPos - _rectTransform.rect.height / 2);
                    //적당히 오차를 내서 바둑판처럼 나오지 않게 하기
                    pos += new Vector2(xOffset, yOffset);
                    //나중에 선 추가 할 때 참고할 노드주소
                    nodeData.Position = pos;
                }

                //이제 노드를 생성 시키고 렉트트랜스폼을 가져와서 위치를 지정
                GameObject node = Instantiate(_nodePerfab, _rectTransform);
                node.GetComponent<RectTransform>().anchoredPosition = nodeData.Position;

                NodeEvent _mapNode = node.GetComponent<NodeEvent>();

                if(nodeData.NodeType == null)
                {
                    nodeData.NodeType = RandomNode(r);
                }
                NodeDataSO selectedNode = nodeData.NodeType;
                if (_mapNode == null)
                {
                    Debug.LogError($"[오류] {node.name} 프리팹에 'NodeEvent' 스크립트가 없습니다!");
                    continue;
                }
                if (_mapNode != null)
                {
                    _mapNode.Setup(selectedNode, nodeData);
                    _nodeEvents[nodeData] = _mapNode;
                }
                Debug.Log($"[성공] {node.name} 등록 시도");

                Image nodeImage = node.GetComponent<Image>();
                if (nodeImage != null && selectedNode != null)
                {
                    nodeImage.sprite = selectedNode._nodeSprite;
                }

                node.name = $"Node_{r}_{nodeData.Col}";
            }
        }
    }
    public void RefreshMapUI() //얘는 노드 갱신용
    {
        if (_manager == null) return;

        if (_nodeEvents == null || _nodeEvents.Count == 0)
        {
            return;
        }

        foreach (var ev in _nodeEvents.Values)
        {
            ev.SelectableNode(false); //모든 노드를 비활성화
        }

        if (_manager.CurrentRow == -1)
        {
            int count = 0;
            foreach (var ev in _nodeEvents)
            {
                if (ev.Key.Row == 0) // NodeData의 Row가 0인 것들
                {
                    ev.Value.SelectableNode(true); //선택 가능한 노드 활성화
                    count++;
                }
            }
            Debug.Log($"0층 노드 {count}개를 활성화");
        }
        else
        {
            NodeData currentNode = null;
            foreach (var data in _nodeEvents.Keys)
            {
                if (data.Row == _manager.CurrentRow && data.Col == _manager.CurrentCol)
                {
                    currentNode = data;
                    break;
                }
            }
            if (currentNode != null)
            {
                foreach (var nextData in currentNode.NextStairs)
                {
                    // 여기서 중요: 객체 참조가 다를 수 있으므로 좌표로 다시 찾아서 활성화
                    foreach (var ev in _nodeEvents)
                    {
                        if (ev.Key.Row == nextData.Row && ev.Key.Col == nextData.Col)
                        {
                            ev.Value.SelectableNode(true);
                        }
                    }
                }
            }
        }
    }
    //private void RestoreMapVisuals()
    //{
    //    foreach (var nodeEnter in _nodeEvents)
    //    {
    //        Vector2 nodeCoord = new(nodeEnter.Key.Row, nodeEnter.Key.Col);
    //        //현재 서 있는 곳 테두리 켜기
    //        if (nodeEnter.Key.Row == _manager.CurrentRow && nodeEnter.Key.Col == _manager.CurrentCol)
    //        {
    //            nodeEnter.Value.SetCurrentNode();
    //        }
    //        //이미 방문한 곳 색칠하기
    //        else if (_manager.VisitedNodes.Contains(nodeCoord))
    //        {
    //            nodeEnter.Value.SetVisitedNode();
    //        }
    //        //어짜피 테두리는 없지만 혹시나 싶으니까
    //        else
    //        {
    //            nodeEnter.Value.HideFrame();
    //        }
    //    }
    //}
    private void RestoreMapVisuals()
    {
        foreach (var nodeEnter in _nodeEvents)
        {
            int r = nodeEnter.Key.Row;
            int c = nodeEnter.Key.Col;
            NodeEvent ui = nodeEnter.Value;

            // 1. 현재 위치 체크 (발바닥)
            if (r == _manager.CurrentRow && c == _manager.CurrentCol)
            {
                Debug.Log($"[MapGrid] 현재 위치 표시 중: {r}, {c}");
                ui.SetCurrentNode();
            }
            // 2. 방문 기록 체크 (리스트를 뒤져서 좌표가 같은게 있는지 확인)
            else if (IsVisited(r, c))
            {
                ui.SetVisitedNode();
            }
            else
            {
                ui.HideFrame();
            }
        }
    }

    // Vector2.Contains 대신 직접 비교하는 헬퍼 함수
    private bool IsVisited(int r, int c)
    {
        foreach (Vector2 v in _manager.VisitedNodes)
        {
            // 정수값이 일치하는지 확인 (오차 방지)
            if (Mathf.Approximately(v.x, r) && Mathf.Approximately(v.y, c))
                return true;
        }
        return false;
    }
    private NodeDataSO RandomNode(int row)
    {
        //마지막은 무조건 보스
        if (row == _row - 1)
        {
            return _mapData._bossNode;
        }
        //첫 시작은 무조건 몹으로 시작함
        if (row == 0)
        {
            return _mapData._startMobNode;
        }
        if (row == _row / 2)
        {
            return _mapData._boxNode;
        }
        else
        {
            //가중치 계산
            List<NodeDataSO> availableNodes = _mapData._nodeType.FindAll(n =>
            {

                bool maxCount = n._maxCount == -1 || !_nodeCount.ContainsKey(n) || _nodeCount[n] < n._maxCount;

                bool lastNodeCheck = (n._nodeName == "MobNodeSO") || !_lastNodeCount.ContainsKey(n) || (row - _lastNodeCount[n] > 1);

                return maxCount && lastNodeCheck;
            });

            if (availableNodes.Count == 0)
            {
                return _mapData._startMobNode;
            }

            float totalWeight = 0;
            foreach (var n in availableNodes)
            {
                totalWeight += n._weight;
            }

            float randomValue = Random.Range(0, totalWeight);
            float curreuntValue = 0;

            foreach (var n in availableNodes)
            {
                curreuntValue += n._weight;
                if (randomValue < curreuntValue)
                {
                    if (_nodeCount.ContainsKey(n))
                    {
                        _nodeCount[n]++;
                    }
                    else
                    {
                        _nodeCount[n] = 1;
                    }
                    _lastNodeCount[n] = row;
                    return n;
                }
            }
            return _mapData._startMobNode;
        }
    }
   
    
}
