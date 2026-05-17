using Data;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CollectibleAuthoring : MonoBehaviour
{
    [SerializeField] private List<CollectibleSO> _collectibleData;

    public List<CollectibleSO> CollectibleData => _collectibleData;
}

public class CollectibleBaker : Baker<CollectibleAuthoring>
{
    public override void Bake(CollectibleAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var collectibleBlobAsset = CreateCollectibleBlobAsset(authoring.CollectibleData);
        AddBlobAsset(ref collectibleBlobAsset, out _);
        AddComponent(entity, new CollectibleComponentData
        {
            CollectiblePoolReference = collectibleBlobAsset
        });
    }

    private BlobAssetReference<CollectiblePool> CreateCollectibleBlobAsset(IReadOnlyList<CollectibleSO> collectibleData)
    {
        using var builder = new BlobBuilder(Allocator.Temp);
        ref var collectiblePool = ref builder.ConstructRoot<CollectiblePool>();
        BlobBuilderArray<CollectibleContainer> arrayBuilder = builder.Allocate(ref collectiblePool.CollectibleData, collectibleData.Count);
        for (int i = 0; i < collectibleData.Count; i++)
        {
            var collectible = collectibleData[i];
            CollectibleType collectibleType = collectible.Type;
            float points = collectible.Points;
            arrayBuilder[i] = new CollectibleContainer{ Points = points, Type = collectibleType };
        }

        return builder.CreateBlobAssetReference<CollectiblePool>(Allocator.Persistent);
    }
}