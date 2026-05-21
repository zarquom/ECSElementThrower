using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _enemyVisualization;
    [SerializeField] private EnemyType _enemyType;
    public GameObject EnemyVisualization => _enemyVisualization;
    public EnemyType EnemyType => _enemyType;
}
public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponentObject(entity, new EnemyVisualizationComponentData
        {
            EnemyVisualization = authoring.EnemyVisualization
        });
        AddComponent(entity, new EnemyComponentData
        {
            Type = authoring.EnemyType
        });
    }
}