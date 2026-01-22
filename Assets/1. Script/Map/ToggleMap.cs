using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleMap : MonoBehaviour
{
    private MyPlayerInput _inputActions;
    [SerializeField] GameObject _mapUI;
    [SerializeField] private MapGrid _mapGrid;

    private void Awake()
    {
        _inputActions = new MyPlayerInput();
    }
    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Map.Map.performed += Toggle;
    }
    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Map.Map.performed -= Toggle;
    }
    private void Toggle(InputAction.CallbackContext ctx)
    {
        if (_mapUI == null) return;

        bool isActive = !_mapUI.activeSelf;
        _mapUI.SetActive(isActive);
        if (isActive)
        {
            _mapGrid.RefreshMapUI();
        }
    }
}
