using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    private MyPlayerInput _inputActions;
    static public MapManager _instance;
    private Dictionary<NodeData, NodeEvent> _nodeEvents = new();
    private List<NodeData> _nextNodes = new();
    private NodeEvent _lastVisitedNode;
    [SerializeField] private GameObject _mapUI;

    public int CurrentRow { get; set; } = -1; //-1은 시작 하지않은 위치
    public int CurrentCol { get; set; } = -1;
    private void Awake()
    {
        _inputActions =  new MyPlayerInput();
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
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
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }
    private void ToggleMap(InputAction.CallbackContext ctx)
    {
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
        foreach (var nodes in _nodeEvents.Values) nodes.SelectableNode(false);
    }
    public List<NodeData> GetNextNode()
    {
        return _nextNodes;
    }
    public void RefreshMapUI()
    {
        if (_nodeEvents == null || _nodeEvents.Count == 0)
        {
            return;
        }

        foreach (var ev in _nodeEvents.Values)
        {
            ev.SelectableNode(false);
        }

        if (CurrentRow == -1)
        {
            int count = 0;
            foreach (var item in _nodeEvents)
            {
                if (item.Key.Row == 0) // NodeData의 Row가 0인 것들
                {
                    item.Value.SelectableNode(true);
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
                    ev.SelectableNode(true);
                }
            }
        }
    }
    public void ExecuteEvent(NodeDataSO data, NodeEvent currentNode)
    {
        if(_lastVisitedNode != null)
        {
            _lastVisitedNode.SetVisitedNode();
        }

        _lastVisitedNode = currentNode;
        _lastVisitedNode.SetCurrentNode();
        switch (data._nodeType)
        {
            case NodeType.Combat: StartBattle(data._sceneName); break;
            case NodeType.Shop: OpenShop(); break;
            case NodeType.Box: OpenBox(); break;
            case NodeType.Boss: StartBattle(data._sceneName); break;
        }
        _mapUI.SetActive(false);
        SceneManager.LoadScene(data._sceneName);
    }
    private void StartBattle(string sceneName) => SceneManager.LoadScene(sceneName);
    private void OpenShop() => SceneManager.LoadScene("ShopScene");
    private void OpenBox() => SceneManager.LoadScene("BoxScene");

}
