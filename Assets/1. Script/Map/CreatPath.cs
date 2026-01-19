using System.Collections.Generic;
using UnityEngine;

public class CreatPath : MonoBehaviour
{
    //경로를 생성하는 메서드
    public void PathCreat(List<NodeData>[] _nodeConnection, int _row, int _col)
    {
        //최상단 노드(보스)
        NodeData boss = new() { Row = _row - 1, Col = _col / 2 };
        _nodeConnection[_row - 1].Add(boss);

        //보스부터 차례대로 밑으로 값을 내릴거임
        int bossBranch = Random.Range(2, 4);

        //얘는 보스에서 내려가는 노드
        for (int i = 0; i < bossBranch; i++)
        {
            int childCol = Mathf.Clamp(boss.Col + (i - 1) * 4 + Random.Range(-1, 2), 0, _col - 1);
            NodeData child = GetOrCreatNode(_nodeConnection, _row - 2, childCol);
            if (!child.NextStairs.Contains(boss))
            {
                child.NextStairs.Add(boss);
            }
        }

        //보스 바로 밑 노드를 제외한 노드
        for (int r = _row - 2; r > 0; r--)
        {
            foreach (var parents in _nodeConnection[r])
            {
                float random = Random.value;
                int branch = random > 0.6 ? 2 : 1;
                for (int i = 0; i < branch; i++)
                {
                    int move = (i == 0) ? Random.Range(-1, 1) : Random.Range(0, 2);
                    int childCol = Mathf.Clamp(parents.Col + move, 0, _col - 1);

                    NodeData child = GetOrCreatNode(_nodeConnection, r - 1, childCol);
                    if (!child.NextStairs.Contains(parents))
                    {
                        child.NextStairs.Add(parents);
                    }
                }
            }
        }
    }
    //만약 노드가 존재하면 노드를 그대로 잇고 아니면 새로운 노드를 생성
    private NodeData GetOrCreatNode(List<NodeData>[] _nodeConnection, int row, int col)
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
}
