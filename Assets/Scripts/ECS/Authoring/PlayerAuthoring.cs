using Unity.Entities;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    public float Speed => _speed;
    public float JumpForce => _jumpForce;
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerComponentData
        {
            Speed = authoring.Speed,
            JumpForce = authoring.JumpForce
        });
        AddComponent(entity, new PlayerMovementComponentData
        {
            Direction = 0f
        });
    }
}
