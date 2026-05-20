using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class JumpingEnemyAuthoring : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    public float JumpForce => _jumpForce;

}
public class JumpingEnemyBaker : Baker<JumpingEnemyAuthoring>
{
    public override void Bake(JumpingEnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new JumpingEnemyComponentData
        {
            JumpForce = authoring.JumpForce
        });
    }
}