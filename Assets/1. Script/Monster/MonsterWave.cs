using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct MonsterGroup
{
    public string groupName;
    public GameObject monsterPrefab;
    public int count;
}
[System.Serializable]
public struct MonsterWave
{
    public string waveName;
    public List<MonsterGroup> monsterGroups; 
    public float spawnInterval;
    public float nextWaveDelay;
}
