using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NodeData
{
    int row, col;
    Vector2 position;
    List<NodeData> downstairs = new();

    public int Row { get; set; }
    public int Col { get; set; }
    public Vector2 Position { get; set; }
    public List<NodeData> DownStairs => downstairs;
}
public class MapGrid : MonoBehaviour
{
    [SerializeField] private LineDraw _drawLine;
    [SerializeField] private CreatPath _creatPath;

    [SerializeField] GameObject _nodePerfab;
    public RectTransform _rectTransform;
    [SerializeField] MapDataSO _mapData;

    [SerializeField] int _row = 15;
    [SerializeField] int _col = 20;
    [SerializeField] float errorValue = 0.5f; //맵 오차 값

    private List<NodeData>[] _nodeConnection;
    //노드별 생성 개수 체크용
    private Dictionary<NodeDataSO, int> _nodeCount = new();
    //이전 노드 개수 저장용(노드 연속 생성 방지)
    private Dictionary<NodeDataSO, int> _lastNodeCount = new();
    private void Start()
    {
        GridNode();
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
        _creatPath.PathCreat(_nodeConnection,_row,_col);
        //거기에 노드 이미지를 그리고
        DrawNode();
        //선을 이어주면 완성!
        _drawLine.DrawLine(_rectTransform,_nodeConnection);

    }
    //경로를 기반으로 노드를 배치
    private void DrawNode()
    {
        float cellWidth = _rectTransform.rect.width / _col;
        float cellHeight = _rectTransform.rect.height / _row;

        //맵 새로 만들 때 초기화
        _nodeCount.Clear();
        _lastNodeCount.Clear();
        for (int r = 0; r < _row; r++)
        {

            foreach (var nodeData in _nodeConnection[r])
            {
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

                //이제 노드를 생성 시키고 렉트트랜스폼을 가져와서 위치를 지정
                GameObject node = Instantiate(_nodePerfab, _rectTransform);
                node.GetComponent<RectTransform>().anchoredPosition = pos;

                NodeDataSO selectedNode = RandomNode(r);
                NodeEvent _mapNode = node.GetComponent<NodeEvent>();
                if(_mapNode != null)
                {
                    _mapNode.Setup(selectedNode,r,nodeData.Col);
                }

                node.GetComponent<Image>().sprite = selectedNode._nodeSprite;

                node.name = $"Node_{r}_{nodeData.Col}";
            }
        }
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

                bool lastNodeCheck = (n._nodeName == "MobNodeSO")||!_lastNodeCount.ContainsKey(n) || (row - _lastNodeCount[n] > 1);

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
