using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPoolSO", menuName = "Scriptable Objects/MonsterPoolSO")]
public class MonsterPoolSO : ScriptableObject
{
    public List<GameObject> monsterPrefabs;
}
