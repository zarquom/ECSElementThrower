using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct BulletAspect : IAspect
{
    private readonly RefRW<BulletComponentData> _bulletComponentData;
    public float3 BulletDirection => _bulletComponentData.ValueRO.BulletDirection;
    public float3 BulletRotation => _bulletComponentData.ValueRO.BulletRotation;
    public float BulletSpeed => _bulletComponentData.ValueRO.BulletSpeed;
}
