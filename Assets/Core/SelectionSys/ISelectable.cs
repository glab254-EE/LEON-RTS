using UnityEngine;

public interface ISelectable
{
    public bool IsSelected { get; }
}
public interface IWithWorldPosition
{
    public Vector3 WorldPosition { get; }
}
