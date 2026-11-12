using UnityEngine;
using UnityEngine.InputSystem;

public class UISelectionVisualizer : MonoBehaviour
{
    [SerializeField]
    private RectTransform SelectionBoxTransform;
    [SerializeField]
    private InputActionReference keybind;
    [SerializeField]
    private float DragTreshold = 0.5f;
    private bool IsHeldDown = false;
    private bool IsDragging = false;
    private Vector2 StartPosition;
    void Start()
    {
        keybind.action.performed += OnInputPress;
        keybind.action.canceled += OnInputPress;
    }
    void OnDestroy()
    {
        keybind.action.performed -= OnInputPress;
        keybind.action.canceled -= OnInputPress;
    }
    void Update()
    {
        var currentMousePos = Mouse.current.position.value;
        if (IsHeldDown && Vector2.Distance(StartPosition,currentMousePos) >= DragTreshold && !IsDragging)
        {
            IsDragging = true;
        }
        if (IsDragging && IsHeldDown)
        {
            if (!SelectionBoxTransform.gameObject.activeInHierarchy)
            {
                SelectionBoxTransform.gameObject.SetActive(true);
            }
            Vector2 boxCenter = (StartPosition + currentMousePos) / 2;
            SelectionBoxTransform.position = boxCenter;
            SelectionBoxTransform.sizeDelta = new(Mathf.Abs(StartPosition.x - currentMousePos.x), Mathf.Abs(StartPosition.y - currentMousePos.y));
        }
        else
        {
            if (SelectionBoxTransform.gameObject.activeInHierarchy)
            {
                SelectionBoxTransform.gameObject.SetActive(false);
            }
        }
    }
    void OnInputPress(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            StartPosition = Mouse.current.position.value;
        }
        IsDragging = false;
        IsHeldDown = callback.performed;
    }
}
