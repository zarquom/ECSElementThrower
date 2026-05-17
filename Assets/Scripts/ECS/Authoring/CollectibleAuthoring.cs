using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CollectibleAuthoring : MonoBehaviour
{
    [SerializeField] private List<float> _collectibleData;

    public List<float> CollectibleData => _collectibleData;
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

    private BlobAssetReference<CollectiblePool> CreateCollectibleBlobAsset(IReadOnlyList<float> collectibleData)
    {
        using var builder = new BlobBuilder(Allocator.Temp);
        ref var collectiblePool = ref builder.ConstructRoot<CollectiblePool>();
        BlobBuilderArray<CollectibleContainer> arrayBuilder = builder.Allocate(ref collectiblePool.CollectibleData, collectibleData.Count);
        for (int i = 0; i < collectibleData.Count; i++)
        {
            float points = collectibleData[i];
            arrayBuilder[i] = new CollectibleContainer{ Points = points };
        }

        return builder.CreateBlobAssetReference<CollectiblePool>(Allocator.Persistent);
    }
}