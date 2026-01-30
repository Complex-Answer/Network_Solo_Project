using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterGroupSO", menuName = "Scriptable Objects/MonsterGroupSO")]
public class MonsterGroupSO : ScriptableObject
{
    public List<MonsterPoolSO> _monsterPool;
}
