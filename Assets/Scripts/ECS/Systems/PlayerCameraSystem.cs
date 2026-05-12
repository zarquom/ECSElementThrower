using Unity.Burst;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

public partial class PlayerCameraSystem : SystemBase
{
    private EntityQuery _playerEntityQuery;
    private CinemachineCamera _playerCamera;

    protected override void OnCreate()
    {
        _playerEntityQuery = SystemAPI.QueryBuilder()
            .WithAll<PlayerManagedComponentData>()
            .Build();
        RequireForUpdate(_playerEntityQuery);
    }

    protected override void OnUpdate()
    {
        if(_playerCamera != null)
        {
            return;
        }
        _playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamera>();
        var playerManagedComponentData = _playerEntityQuery.GetSingleton<PlayerManagedComponentData>();
        _playerCamera.Follow = playerManagedComponentData.TransformData;
    }
}
