using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _enemyVisualization;
    public GameObject EnemyVisualization => _enemyVisualization;

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
    }
}