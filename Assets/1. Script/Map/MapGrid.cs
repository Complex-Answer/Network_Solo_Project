using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;
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



    [SerializeField] GameObject _nodePerfab;
    public RectTransform _rectTransform;
    [SerializeField] MapDataSO _mapData;

    [SerializeField] int _row = 15;
    [SerializeField] int _col = 20;
    [SerializeField] float errorValue = 0.5f; //맵 오차 값

    private List<NodeData>[] _nodeConnection;
    //노드별 생성 개수 체크용
    private Dictionary<NodeDataSO, int> _nodeCount = new();
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
        CreatPath();
        //거기에 노드 이미지를 그리고
        DrawNode();
        //선을 이어주면 완성!
        _drawLine.DrawLine(_rectTransform,_nodeConnection);

    }
    //경로를 생성하는 메서드
    private void CreatPath()
    {
        //최상단 노드(보스)
        NodeData boss = new() { Row = _row - 1, Col = _col / 2 };
        _nodeConnection[_row - 1].Add(boss);

        //보스부터 차례대로 밑으로 값을 내릴거임
        int bossBranch = Random.Range(2, 4);

        //얘는 보스에서 내려가는 노드
        for (int i = 0; i < bossBranch; i++)
        {
            int childCol = Mathf.Clamp(boss.Col + (i - 1) * 5 + Random.Range(-1, 2), 0, _col - 1);
            NodeData child = GetOrCreatNode(_row - 2, childCol);
            boss.DownStairs.Add(child);
        }

        //보스 바로 밑 노드를 제외한 노드
        for (int r = _row - 2; r > 0; r--)
        {
            foreach (var parents in _nodeConnection[r])
            {
                float random = Random.value;
                int branch = random > 0.65 ? 2 : 1;
                for (int i = 0; i < branch; i++)
                {
                    int move = (i == 0) ? Random.Range(-1, 1) : Random.Range(0, 2);
                    int childCol = Mathf.Clamp(parents.Col + move, 0, _col - 1);

                    NodeData child = GetOrCreatNode(r - 1, childCol);
                    if (!parents.DownStairs.Contains(child))
                    {
                        parents.DownStairs.Add(child);
                    }
                }
            }
        }
    }
    //만약 노드가 존재하면 노드를 그대로 잇고 아니면 새로운 노드를 생성
    private NodeData GetOrCreatNode(int row, int col)
    {
        NodeData exexisting = _nodeConnection[row].Find(n => n.Col == col);
        if (exexisting != null)
        {
            return exexisting;
        }
        NodeData newNode = new() { Row = row, Col = col };
        _nodeConnection[row].Add(newNode);
        return newNode;

    }
    //경로를 기반으로 노드를 배치
    private void DrawNode()
    {
        float cellWidth = _rectTransform.rect.width / _col;
        float cellHeight = _rectTransform.rect.height / _row;
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
                Image nodeImage = node.GetComponent<Image>();

                nodeImage.sprite = RandomNode(r);

                node.name = $"Node_{r}_{nodeData.Col}";
            }
        }
    }
    private Sprite RandomNode(int row)
    {
        if (row == _row - 1)
        {
            return _mapData._bossNode._nodeSprite;
        }
        if (row == 0)
        {
            return _mapData._startMobNode._nodeSprite;
        }
        if (row == _row / 2)
        {
            return _mapData._boxNode._nodeSprite;
        }
        else
        {
            List<NodeDataSO> availableNodes = _mapData._nodeType.FindAll(
                n => n._maxCount == -1 || !_nodeCount.ContainsKey(n) || _nodeCount[n] < n._maxCount);

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
                    return n._nodeSprite;

                }
            }
            return _mapData._startMobNode._nodeSprite;
        }
    }
}
