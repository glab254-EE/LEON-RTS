using Leopotam.EcsProto;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference SelectButton;
    [SerializeField]
    private InputActionReference MoveButton;
    [SerializeField]
    private LayerMask GroundMask;
    [SerializeField]
    private float TargetOffset;
    private ProtoWorld world;
    private ProtoSystems systems;
    private UnitAspect aspect;
    void Start()
    {
        aspect = new()
        {
            SelectButton = this.SelectButton,
            MoveButton = this.MoveButton, 
            GroundMask = this.GroundMask,
            TargetPointOffsetRange = this.TargetOffset
        };

        world = new(aspect);

        var potentialExsistingUnits = FindObjectsByType<UnitDataHolder>();

        if (potentialExsistingUnits != null && potentialExsistingUnits.Length > 0)
        {
            foreach (var unit in potentialExsistingUnits)
            {
                if (unit.Data == null) continue;
                ref UnitComponent newEntity = ref aspect.Units.NewEntity(out _);
                newEntity.StoppingDistance = unit.Data.StoppingDistance;
                newEntity.Speed = unit.Data.Speed;
                newEntity.AttachedTransform = unit.transform;
                newEntity.OffsetPosition = new(0, unit.transform.position.y,0);
            }
        }

        systems = new(world);

        systems.AddSystem(new SelectionSystem());
        systems.AddSystem(new UnitMovementSystem());

        systems.Init();
    }
    void Update()
    {
        systems.Run();
    }
    void OnDestroy()
    {
        systems?.Destroy();
        systems = null;
        world?.Destroy();
        world = null;
    }
}