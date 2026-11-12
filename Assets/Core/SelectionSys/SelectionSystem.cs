using Leopotam.EcsProto;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class SelectionSystem : IProtoInitSystem, IProtoDestroySystem
{
    private UnitAspect aspect;
    private ProtoIt iterator;
    private Vector2 startPosition;
    private InputAction keybinding;
    private Camera camera;
    public void Destroy()
    {
        if (keybinding != null)
        {
            keybinding.performed -= OnSelectInput;
            keybinding.canceled -= OnSelectInput;
            keybinding.Disable();
        }
    }
    public void Init(IProtoSystems systems)
    {
        var World = systems.World();
        aspect = (UnitAspect)World.Aspect(typeof(UnitAspect));
        iterator = new(new[] { typeof(UnitComponent) });
        iterator.Init(World);
        camera = Camera.main;
        if (aspect != null && aspect.SelectButton != null)
        {
            keybinding = aspect.SelectButton.action;
            keybinding.performed += OnSelectInput;
            keybinding.canceled += OnSelectInput;
            keybinding.Enable();
        }
    }
    void OnSelectInput(InputAction.CallbackContext callback)
    {
        var mousePos = Mouse.current.position.value;
        if (callback.performed)
        {
            startPosition = mousePos;
        }
        else
        {
            if (Vector2.Distance(startPosition,mousePos) >= 0.5f)
            {
                float width = Mathf.Abs(mousePos.x- startPosition.x);
                float height = Mathf.Abs(mousePos.y - startPosition.y);
                Vector2 centerPosition = (startPosition + mousePos) / 2;
                SelectUnits(new(centerPosition, new Vector3(width, height, width)));
            }
            else
            {
                SelectUnits(null);
            }
        }
    }
    private void SelectUnits(Bounds? bounds)
    {
        foreach(var item in iterator)
        {
            ref UnitComponent unit = ref aspect.Units.Get(item);
            unit.IsSelected = false;
            if (bounds != null && IsUnitWithinCamera(unit, bounds.Value))
            {
                unit.IsSelected = true;
                if (unit.WorldPosition == default)
                {
                    unit.WorldPosition = unit.AttachedTransform.position - unit.OffsetPosition;
                }
            }
        }
    }
    private bool IsUnitWithinCamera(UnitComponent unit, Bounds bounds)
    {
        Vector3 unitPosInCamera = camera.WorldToScreenPoint(unit.WorldPosition);
        return unitPosInCamera.x >= bounds.min.x
            && unitPosInCamera.x <= bounds.max.x
            && unitPosInCamera.y >= bounds.min.y
            && unitPosInCamera.y <= bounds.max.y;
    }
}
