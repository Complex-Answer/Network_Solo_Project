using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatCard : MonoBehaviour
{
    public TextMeshProUGUI _titleText;       // 프리팹 내의 제목 텍스트
    public TextMeshProUGUI _descriptionText; // 프리팹 내의 설명 텍스트
    public Image _icon;                 // 프리팹 내의 아이콘 이미지
    public GameObject _focusEffect;

    private StatsSO _currentData; // 현재 이 카드가 들고 있는 정보
    public void Setup(StatsSO data)
    {
        if (data == null) return;

        _currentData = data; // 전달받은 SO 저장

        _titleText.text = data.StatName;
        _descriptionText.text = data.Description;

        if (data.Icon != null)
        {
            _icon.sprite = data.Icon;
        }
    }

    public void OnCardClick()
    {
        if (_currentData == null) return;

        if(_focusEffect != null) _focusEffect.SetActive(true);

        Debug.Log($"{_currentData.StatName} 카드를 선택했습니다!");

        string typeName = _currentData.Type.ToString();

        GameManager._instance.UpgradeStats(typeName, _currentData.Value);

        Invoke(nameof(ClosePanel), 0.5f);
    }
    private void ClosePanel()
    {
        SetStat parentPanel = GetComponentInParent<SetStat>();
        if (parentPanel != null) parentPanel.gameObject.SetActive(false);
    }
}
