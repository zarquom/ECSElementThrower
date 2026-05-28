using Unity.Entities;
using UnityEditor.Build.Pipeline;
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
        AddComponent(entity, new PlayerMovementComponentData
        {
            Direction = 0f,
            LastDirection = 1f
        });
        AddComponentObject(entity, new PlayerVisualizationComponentData
        {
            PlayerVisualization = authoring.PlayerVisualization
        });
    }
}
