using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPun
{
    [SerializeField] private Image _hpSlider;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _goldText;

    private PlayerManager _myPlayer;
    private void Start()
    {
        StartCoroutine(ConnectPlayer());
    }
    IEnumerator ConnectPlayer()
    {
        while (PlayerManager._instance == null)
        {
            yield return null;
        }

        _myPlayer = PlayerManager._instance;

        _myPlayer.OnHpChanged -= UpdateHPUI;
        //_myPlayer.OnGoldChanged -= UpdateGoldUI;

        _myPlayer.OnHpChanged += UpdateHPUI;
        //_myPlayer.OnGoldChanged += UpdateGoldUI;

        Debug.Log("UI 매니저: 플레이어 연결 완료 및 UI 초기화");
        UpdateHPUI(_myPlayer.Hp, _myPlayer.MaxHp);
        //UpdateGoldUI(_myPlayer.Gold);
    }
    private void UpdateHPUI(float current, float max)
    {
        if (_hpText != null) _hpText.text = $"{current} / {max}";
        if (_hpSlider != null) _hpSlider.fillAmount = current / max;
    }

    //public void UpdateGoldUI(int gold)
    //{
    //    if (_goldText != null) _goldText.text =
    //
    //    .ToString("N0");
    //}

    private void OnDestroy()
    {
        if (_myPlayer != null)
        {
            _myPlayer.OnHpChanged -= UpdateHPUI;
            //_myPlayer.OnGoldChanged -= UpdateGoldUI;
        }
    }
}
