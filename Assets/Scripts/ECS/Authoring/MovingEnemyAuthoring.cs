using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovingEnemyAuthoring : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    public float MovementSpeed => _movementSpeed;

}
public class MovingEnemyBaker : Baker<MovingEnemyAuthoring>
{
    public override void Bake(MovingEnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new MovingEnemyComponentData
        {
            MovementSpeed = authoring.MovementSpeed
        });
    }
}