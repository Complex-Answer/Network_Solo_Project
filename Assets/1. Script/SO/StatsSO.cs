using UnityEngine;
public enum StatType
{
    Attack,
    MoveSpeed,
    AttackSpeed,
    MaxHp,
}

[CreateAssetMenu(fileName = "StatsSO", menuName = "Scriptable Objects/StatsSO")]
public class StatsSO : ScriptableObject
{
    [Header("기본 정보")]
    public string StatName; //이름  
    [TextArea]
    public string Description; //설명
    public Sprite Icon; //아이콘

    [Header("분류 및 수치")]
    public int Grade;   //등급    
    public StatType Type;  
    public float Value; //실제 능력치 값    
}
