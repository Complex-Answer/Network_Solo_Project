using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 게임 매니저입니다.
/// 게임의 전체적인 내용 및 네트워크를 관리합니다
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    static public GameManager _instance;

    private Dictionary<int, bool> _alivePlayers = new ();

    [Header("플레이어 스탯")]
    public float _currentHp;
    [SerializeField] private float _maxHp = 100;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _attack = 5;
    [SerializeField] private float _attackSpeed = 4;

    private float _baseMaxHp;
    private float _baseMoveSpeed;
    private float _baseAttack;
    private float _baseAttackSpeed;

    [Header("캐릭터 외형 및 투사체 설정")]
    public List<PlayerSO> characterSkins = new();
    public float Hp { get { return _currentHp; } set { _currentHp = value; } }
    public float MaxHp => _maxHp;
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float Attack => _attack;
    public float AttackSpeed => _attackSpeed;


    private bool _isEndBattle = false;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _baseMaxHp = _maxHp;
            _baseMoveSpeed = _moveSpeed;
            _baseAttack = _attack;
            _baseAttackSpeed = _attackSpeed;
            _currentHp = _maxHp;
            if (PhotonNetwork.InRoom) InitSurvivalList();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SavePlayerStats(float hp)
    {
        _currentHp = hp;
    }
    public void InitSurvivalList()
    {
        if (_alivePlayers.Count > 0) return;
        _alivePlayers.Clear();
        foreach (var p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            _alivePlayers.Add(p.ActorNumber, true);
        }
    }

    // 외부(PlayerManager)에서 호출하여 장부를 갱신할 함수
    public void UpdateSurvivalStatus(int actorNr, bool isAlive)
    {
        if (_alivePlayers.ContainsKey(actorNr))
        {
            _alivePlayers[actorNr] = isAlive;
            Debug.Log($"[GameManager] {actorNr}번 플레이어 생존 상태 변경: {isAlive}");
        }
    }
    public bool IsAlive(int actorNr)
    {
        if (_alivePlayers.TryGetValue(actorNr, out bool alive)) return alive;
        return true;
    }

    [PunRPC]
    public void RPC_EndBattleAll(bool playerWon)
    {
        EndBattle(playerWon);
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
        }
        else
        {
            yield return new WaitForSeconds(2f);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOver"); // 씬 이름 오타 주의!
            }
        }
    }
    public void UpgradeStats(string statType, float amount)
    {
        switch (statType)
        {
            case "MaxHp":
                _maxHp += amount;
                _currentHp = Mathf.Min(_currentHp + amount, _maxHp); //최대 체력 증가시 현재 체력도 같이 증가
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

    public void BackToLobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ResetGameStates();

            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }

    public void ResetGameStates()
    {
        _isEndBattle = false;
        _alivePlayers.Clear();

        _maxHp = _baseMaxHp;
        _currentHp = _baseMaxHp;
        _moveSpeed = _baseMoveSpeed;
        _attack = _baseAttack;
        _attackSpeed = _baseAttackSpeed;
    }
}
