using Leopotam.EcsProto;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitAspect : IProtoAspect
{
    public ProtoPool<UnitComponent> Units;
    public InputActionReference SelectButton;
    public InputActionReference MoveButton;
    public float TargetPointOffsetRange;
    public LayerMask GroundMask;
    private ProtoWorld world;
    public void Init(ProtoWorld _world)
    {
        world = _world;
        world.AddAspect(this);

        Units = new();
        world.AddPool(Units);
    }
    public void PostInit()
    {
    }
    public ProtoWorld World() => world;
}
public class UnitMovementSystem : IProtoInitSystem, IProtoRunSystem, IProtoDestroySystem
{
    private UnitAspect aspect;
    private ProtoIt iterator;
    private InputAction moveKeybinding;
    private float maxOffset;
    private Camera camera;
    public void Init(IProtoSystems systems)
    {
        camera = Camera.main;
        var World = systems.World();
        aspect = (UnitAspect)World.Aspect(typeof(UnitAspect));
        iterator = new(new[] { typeof(UnitComponent) });
        iterator.Init(World);
        maxOffset = aspect.TargetPointOffsetRange;
        if (aspect != null && aspect.MoveButton != null)
        {
            moveKeybinding = aspect.MoveButton.action;
            moveKeybinding.performed += OnMoveButtonPress;
            moveKeybinding.Enable();
        }
    }
    public void Destroy()
    {
        if (moveKeybinding != null)
        {
            moveKeybinding.performed -= OnMoveButtonPress;
            moveKeybinding.Disable();
        }
    }
    public void Run()
    {
        foreach (var item in iterator)
        {
            ref UnitComponent unit = ref aspect.Units.Get(item);
            if (!IsUnitInDestination(unit))
            {
                MoveUnitToDestination(ref unit);
                UpdateVisualization(unit);
            }
        }
    }
    private void MoveUnitToDestination(ref UnitComponent unit)
    {
        unit.WorldPosition = Vector3.MoveTowards(unit.WorldPosition, unit.Target, unit.Speed * Time.deltaTime);
    }
    private void UpdateVisualization(UnitComponent unit)
    {
        unit.AttachedTransform.position = unit.WorldPosition + unit.OffsetPosition;
    }
    private void OnMoveButtonPress(InputAction.CallbackContext _)
    {
        Ray targetRay = camera.ScreenPointToRay(Mouse.current.position.value);
        if (Physics.Raycast(targetRay, out RaycastHit hitInfo, 1000, aspect.GroundMask.value))
        {
            foreach (var item in iterator)
            {
                ref UnitComponent unit = ref aspect.Units.Get(item);
                if (unit.IsSelected)
                {
                    Vector3 targetPos = hitInfo.point;
                    targetPos += new Vector3(Random.Range(-maxOffset, maxOffset), 0, Random.Range(-maxOffset, maxOffset));
                    unit.Target = targetPos;
                    unit.CurrentStoppingDistance = Random.Range(0, unit.StoppingDistance);
                }
            }
        }
    }
    private bool IsUnitInDestination(UnitComponent component)
    {
        if (component.Target == null)
        {
            return true;
        }
        if (Vector3.Distance(component.WorldPosition, component.Target) <= component.CurrentStoppingDistance)
        {
            return true;
        }
        return false;
    }
}