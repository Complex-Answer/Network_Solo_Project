using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO", menuName = "Scriptable Objects/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    public string _enemyName;
    public int _maxHp;
    public int _attackDamage;
    public int _moveSpeed;
    public GameObject _enemyModelPrefab; 
    public float _attackRange;
}
