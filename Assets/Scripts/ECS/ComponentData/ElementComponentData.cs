
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ElementComponentData : IComponentData
{
    public float3 BulletDirection;
    public float3 BulletRotation;
    public float BulletSpeed;
}
