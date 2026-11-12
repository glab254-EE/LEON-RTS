using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitDataSO : ScriptableObject
{
    public float Speed = 1;
    public float StoppingDistance = 0.5f;
}

public struct UnitComponent : ISelectable, IWithWorldPosition
{
    public Transform AttachedTransform;
    public float Speed;
    public float StoppingDistance;
    public float CurrentStoppingDistance;
    public Vector3 OffsetPosition;
    public Vector3 Target;
    public Vector3 WorldPosition { get; set; }
    public bool IsSelected { get; set; }
    public bool IsInitialized { get; set; }
}