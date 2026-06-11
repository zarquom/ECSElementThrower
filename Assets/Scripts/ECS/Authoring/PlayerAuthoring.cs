using System.Collections.Generic;
using Unity.Entities;
using UnityEditor.Build.Pipeline;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _playerVisualization;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _health;

    public float Speed => _speed;
    public float JumpForce => _jumpForce;
    public float Health => _health;
    public GameObject PlayerVisualization => _playerVisualization;
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerComponentData
        {
            Speed = authoring.Speed,
            JumpForce = authoring.JumpForce,
            IsDead = false,
            Health = authoring.Health,
            Throwing = false
        });
        var buffer = AddBuffer<CollectibleElement>(entity);
        buffer.Add(new CollectibleElement { Type = CollectibleType.Fire, Amount = 0 });
        buffer.Add(new CollectibleElement { Type = CollectibleType.Water, Amount = 0 });
        buffer.Add(new CollectibleElement { Type = CollectibleType.Earth, Amount = 0 });
        buffer.Add(new CollectibleElement { Type = CollectibleType.Wind, Amount = 0 });
        AddComponent(entity, new PlayerMovementComponentData
        {
            Direction = 0f,
            LastDirection = 1f
        });
        AddComponentObject(entity, new PlayerVisualizationComponentData
        {
            PlayerVisualization = authoring.PlayerVisualization
        });
        AddComponent(entity, new CameraComponentData
        {
            IsCenteredOnPlayer = true
        });
    }
}
