using UnityEngine;
using UnityEngine.InputSystem;

public class TrackingMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        RotationToMouse();
    }

    private void RotationToMouse()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;

            Vector3 dir = (targetPosition - transform.position).normalized;

            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation  = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
                 Debug.DrawRay(transform.position, dir * 5f, Color.red);
            }
        }
    }
}
