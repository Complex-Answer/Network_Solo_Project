using System.Collections.Generic;
using UnityEngine;

public class SetStat : MonoBehaviour
{
    [Header("등급별 프리팹 (Green, Blue, Purple 순서)")]
    public GameObject[] _gradePrefabs;

    [Header("카드가 생성될 위치")]
    public Transform[] _slots;

    public List<StatsSO> UncommonStats;
    public List<StatsSO> RareStats;
    public List<StatsSO> EpicStats;
    public void Open()
    {
        gameObject.SetActive(true);

        foreach (Transform slot in _slots)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            int gradeIndex = GetRandomGrade();

            GameObject cardObj = Instantiate(_gradePrefabs[gradeIndex], _slots[i]);

            // 데이터 주입
            StatCard cardScript = cardObj.GetComponent<StatCard>();
            cardScript.Setup(GetRandomStat(gradeIndex));
        }
    }
    private int GetRandomGrade()
    {
        float rand = Random.Range(0f, 100f);

        if (rand < 10f) return 2;      
        else if (rand < 40f) return 1; 
        else return 0;                 
    }
    private StatsSO GetRandomStat(int gradeIndex)
    {
        List<StatsSO> targetList;

        switch (gradeIndex)
        {
            case 2: targetList = EpicStats; break;
            case 1: targetList = RareStats; break;
            default: targetList = UncommonStats; break;
        }

        if (targetList == null || targetList.Count == 0) return null;
        return targetList[Random.Range(0, targetList.Count)];
    }
}
