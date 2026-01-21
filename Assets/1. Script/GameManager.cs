using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 게임 매니저입니다.
/// 게임의 전체적인 내용 및 네트워크를 관리합니다
/// </summary>
public class GameManager : MonoBehaviour
{
    static public GameManager _instance;

    [Header("플레이어 스탯")]
    public float _currentHp;
    [SerializeField] private float _maxHp = 100;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _attack = 5;
    [SerializeField] private float _attackSpeed = 4;
    [SerializeField] private int _gold = 99;

    public float Hp { get { return _currentHp; } set { _currentHp = value; } }
    public float MaxHp => _maxHp;
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float Attack => _attack;
    public float AttackSpeed => _attackSpeed;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private bool _isEndBattle = false;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _currentHp = _maxHp;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SavePlayerStats(float hp, int gold)
    {
        _currentHp = hp;
        _gold = gold;
    }
    public void EndBattle(bool playerWon)
    {
        if (_isEndBattle)
        {
            return;
        }
        _isEndBattle = true;
        StartCoroutine(EndCoroutine(playerWon));
    }

    public IEnumerator EndCoroutine(bool playerWon)
    {
        if (playerWon)
        {
            yield return new WaitForSeconds(1.5f);
            SpawnPortal();
        }
        else
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("GameOverScene");
        }
    }
    private void SpawnPortal()
    {

    }
    public void UpgradeStats(string statType, float amount)
    {
        switch (statType)
        {
            case "MaxHp":
                _maxHp += amount;
                _currentHp += amount; //최대 체력 증가시 현재 체력도 같이 증가
                break;
            case "MoveSpeed":
                _moveSpeed += amount;
                break;
            case "Attack":
                _attack += amount;
                break;
            case "AttackSpeed":
                _attackSpeed += amount;
                break;
            default:
                Debug.LogWarning("Unknown stat type: " + statType);
                break;
        }
    }
}
