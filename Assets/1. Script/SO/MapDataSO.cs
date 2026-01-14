using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Scriptable Objects/MapDataSO")]
public class MapDataSO : ScriptableObject
{
    public List<NodeDataSO> _nodeType;
    public NodeDataSO _bossNode;
    public NodeDataSO _startMobNode;
    public NodeDataSO _MobNode;
    public NodeDataSO _boxNode;
    public NodeDataSO _restNode;
    public NodeDataSO _rndNode;
}
