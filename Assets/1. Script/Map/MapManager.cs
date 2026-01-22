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
    #region 필드
    
    static public MapManager _instance;
    private List<Vector2> _visitedNodes = new();

    private int _currentRow = -1; //-1은 시작 하지않은 위치
    private int _currentCol = -1;

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
            DontDestroyOnLoad(gameObject); // 이 줄이 있어야 데이터가 유지됩니다
        }
        else { Destroy(gameObject); }
    }
   
    
    //그냥 맵 껐다 켜주는 거
   
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
        //포톤을 사용해서 씬 전환 (방장만 이동가능)
        if (!string.IsNullOrEmpty(data._sceneName))
        {
            Debug.Log($"[맵매니저] 노드 이벤트 실행: {data._nodeName}, 씬 로드: {data._sceneName}");
            SceneManager.LoadScene(data._sceneName);
            //PhotonNetwork.LoadLevel(data._sceneName);
        }
        else
        {
            Debug.LogWarning($"[맵매니저] 노드 이벤트 실행: {data._nodeName}, 씬 이름이 비어있습니다.");
        }
    }

}
