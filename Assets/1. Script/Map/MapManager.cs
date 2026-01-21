using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Photon.Pun;

/// <summary>
/// 맵 관리 매니저입니다
/// 맵의 노드들을 관리하고, 플레이어의 이동과 이벤트 실행을 담당합니다.
/// </summary>
public class MapManager : MonoBehaviour
{
    private MyPlayerInput _inputActions;
    static public MapManager _instance;
    private Dictionary<NodeData, NodeEvent> _nodeEvents = new();
    private List<NodeData> _nextNodes = new();
    private NodeEvent _lastVisitedNode;
    [SerializeField] private GameObject _mapUI;

    private List<Vector2> _visitedNodes = new();

    private int _currentRow = -1;
    private int _currentCol = -1;

    public int CurrentRow { get { return _currentRow; } set { _currentRow = value; } } //-1은 시작 하지않은 위치
    public int CurrentCol { get { return _currentCol; } set { _currentCol = value; } }
    public bool CanMove { get; set; } = true; //전투인지 확인하는 불 값 프로퍼티
    private void Awake()
    {
        _inputActions = new MyPlayerInput();
        if (_instance == null) //싱글톤 패턴
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            PhotonNetwork.AutomaticallySyncScene = true;
            if (SceneManager.GetActiveScene().name == "Floor")
            {
                CanMove = true;
                Debug.Log("MapManager: 맵 씬으로 시작됨. CanMove 활성화.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Map.Map.performed += ToggleMap;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Map.Map.performed -= ToggleMap;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void ToggleMap(InputAction.CallbackContext ctx)
    {
        if (_mapUI == null) return;

        bool isActive = !_mapUI.activeSelf;
        _mapUI.SetActive(isActive);
        if (isActive)
        {
            RefreshMapUI();
        }
    }
    public void RegisterNode(NodeData data, NodeEvent nodeEvent)
    {
        _nodeEvents[data] = nodeEvent;
    }
    public void ClearMap() => _nodeEvents.Clear();
    public void SetNextNode(List<NodeData> nextNode)
    {
        _nextNodes = nextNode;
        foreach (var nodes in _nodeEvents.Values)
        {
            nodes.SelectableNode(false);
        }
    }

    public List<NodeData> GetNextNode()
    {
        return _nextNodes;
    }
    public void RefreshMapUI() //얘는 노드 갱신중
    {
        if (_mapUI == null) return;

        if (_nodeEvents == null || _nodeEvents.Count == 0)
        {
            return;
        }

        foreach (var ev in _nodeEvents.Values)
        {
            ev.SelectableNode(false); //모든 노드를 비활성화
        }

        if (CurrentRow == -1)
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
            Debug.Log($"[성공] 0층 노드 {count}개를 활성화했습니다.");
        }
        else if (_nextNodes != null)
        {
            foreach (var data in _nextNodes)
            {
                if (_nodeEvents.TryGetValue(data, out NodeEvent ev))
                {
                    ev.SelectableNode(true); //이제 한칸 씩 전진할 수 있는 노드를 활성화
                }
            }
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Floor")
        {
            if (_mapUI != null)
            {
                CanMove = true;
                Debug.Log("CanMove가 true로 설정되었습니다.");

                if (_mapUI != null)
                {
                    _mapUI.SetActive(true);
                }
                //밑에서 노드 저장해놨으니까 거기서 하나하나 살펴보기
                foreach (var nodeEnter in _nodeEvents)
                {
                    Vector2 nodeCoord = new(nodeEnter.Key.Row, nodeEnter.Key.Col);
                    //내가 있는 곳이 현재 위치라면
                    if (nodeEnter.Key.Row == _currentRow && nodeEnter.Key.Col == _currentCol)
                    {
                        nodeEnter.Value.SetCurrentNode(); //노드 이벤트에서 테두리 키기
                    }
                    //이미 지나온 곳이라면
                    else if (_visitedNodes.Contains(nodeCoord))
                    {
                        nodeEnter.Value.SetVisitedNode(); //노드 이벤트에서 색을 바꿔서 처리
                    }
                    //다른 모든 노드는 프레임 제거. 혹시나 싶어서..
                    else
                    {
                        nodeEnter.Value.HideFrame();
                    }
                }
                RefreshMapUI(); //얘는 맵 갱신시키는거
            }
        }
        else
        {
            CanMove = false;
            RefreshMapUI();
        }
    }
    //노드를 선택하는 메서드
    public void ExecuteEvent(NodeDataSO data, NodeEvent currentNode)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    Debug.LogWarning("방장만 노드 이벤트를 실행할 수 있습니다.");
        //    return;
        //}

        //해당 노드의 좌표만 받아 놓기
        _currentRow = currentNode.MapNode.Row;
        _currentCol = currentNode.MapNode.Col;

        Vector2 currentNodes = new(CurrentRow, CurrentCol);
        if (!_visitedNodes.Contains(currentNodes))
        {
            _visitedNodes.Add(currentNodes);
        }

        if (_mapUI != null)
        {
            _mapUI.SetActive(false);
        }

        //포톤을 사용해서 씬 전환 (방장만 이동가능)
        if (!string.IsNullOrEmpty(data._sceneName))
        {
            Debug.Log($"[맵매니저] 노드 이벤트 실행: {data._nodeName}, 씬 로드: {data._sceneName}");
            PhotonNetwork.LoadLevel(data._sceneName);
        }
        else
        {
            Debug.LogWarning($"[맵매니저] 노드 이벤트 실행: {data._nodeName}, 씬 이름이 비어있습니다.");
        }
    }

}
