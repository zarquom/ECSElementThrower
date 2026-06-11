using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public struct PlayerComponentData : IComponentData
{
    public float Speed;
    public float JumpForce;
    public bool IsDead;
    public float Health;
    public bool Throwing;
}
public struct CollectibleElement : IBufferElementData
{
    public CollectibleType Type;
    public int Amount;
}