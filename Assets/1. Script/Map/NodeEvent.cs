using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeEvent : MonoBehaviour, IPointerClickHandler
{
    private NodeDataSO _nodeData;
    private int _row, _col;
    private bool _isSelectable = false;

    public void Setup(NodeDataSO data, int row, int col)
    {
        _nodeData = data;
        _row = row;
        _col = col;

        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"노드 클릭: {_nodeData._nodeName} at ({_row}, {_col})");

        MapManager._instance.CurrentRow = _row;
        MapManager._instance.CurrentCol = _col;

        GetComponent<Image>().sprite = _nodeData._nodeSprite;
        if (_nodeData != null)
        {
            MapManager._instance.ExecuteEvent(_nodeData);
        }
    }
    public void SelectableNode(bool select)
    {
        _isSelectable = select;

        Image img = GetComponent<Image>();
        img.color = select ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.8f);
        img.raycastTarget = select;
    }
}
