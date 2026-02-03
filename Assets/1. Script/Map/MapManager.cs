using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Photon.Pun;

/// <summary>
/// 맵 관리 매니저입니다
/// 맵의 노드들을 관리하고, 플레이어의 이동과 이벤트 실행을 담당합니다.
/// </summary>
public class MapManager : MonoBehaviourPunCallbacks
{
    #region 필드

    static public MapManager _instance;
    private List<Vector2> _visitedNodes = new();

    private int _currentRow = -1; //-1은 시작 하지않은 위치
    private int _currentCol = -1;
    private bool _isLoding = false;
    public int CurrentRow { get { return _currentRow; } set { _currentRow = value; } }
    public int CurrentCol { get { return _currentCol; } set { _currentCol = value; } }
    public bool CanMove { get; set; } = true; //전투인지 확인하는 불 값 프로퍼티
    public List<Vector2> VisitedNodes => _visitedNodes;
    public List<NodeData>[] SavedMapData { get; set; }
    #endregion
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    //노드를 선택하는 메서드
    public void ExecuteEvent(NodeDataSO data, NodeEvent currentNode)
    {
        if (_isLoding) return;
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_ExecuteEventRequest), RpcTarget.All, currentNode.MapNode.Row, currentNode.MapNode.Col, data._sceneName);
    }
    [PunRPC]
    public void RPC_ExecuteEventRequest(int row, int col, string sceneName)
    {

        _currentRow = row;
        _currentCol = col;

        Debug.Log($"[RPC] 방장이 이동 명령 수신! 목적지: {sceneName}");

        Vector2 nodePos = new Vector2(row, col);
        if (!_visitedNodes.Contains(nodePos))
        {
            _visitedNodes.Add(nodePos);
        }

        Debug.Log($"PhotonNetwork.LoadLevel({sceneName}) 실행!");


        if (!string.IsNullOrEmpty(sceneName))
        {
            _isLoding = true;
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"모두를 데리고 {sceneName} 씬으로 이동합니다.");
                PhotonNetwork.LoadLevel(sceneName);
            }
            if (BattleManager._instance != null)
            {
                BattleManager._instance.WinBattle();
            }
        }
        else
        {
            Debug.LogError("전달받은 씬 이름이 비어있습니다!");
        }
    }
}
