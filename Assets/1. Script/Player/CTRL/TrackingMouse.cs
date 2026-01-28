using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackingMouse : MonoBehaviourPun
{
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        if (photonView != null && !photonView.IsMine)
        {
            return;
        }

        RotationToMouse();
    }

    private void RotationToMouse()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 dir = hitPoint - transform.position;
            dir.y = 0; // 캐릭터가 위아래로 꺾이지 않게 고정

            if (dir.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
