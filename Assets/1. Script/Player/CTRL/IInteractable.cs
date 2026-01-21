using UnityEngine;

public interface IInteractable
{
    void OnInteract(PlayerManager player); //상호작용에 실행되는 함수
    string GetInteractText(); // 상호작용 시 텍스트 띄우는 용도
}
