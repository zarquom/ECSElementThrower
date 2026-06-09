using Unity.Burst;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class PlayerCameraSystem : SystemBase
{
    private EntityQuery _playerEntityQuery;
    private CinemachineCamera _playerCamera;

    protected override void OnCreate()
    {
        _playerEntityQuery = SystemAPI.QueryBuilder()
            .WithAll<PlayerManagedComponentData>()
            .WithAll<CameraComponentData>()
            .Build();
        RequireForUpdate(_playerEntityQuery);
    }

    protected override void OnUpdate()
    {
        if(_playerCamera == null)
        {
            _playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamera>();
        }
        if (_playerCamera == null)
        {
            return;
        }
        var cameraComponentData = _playerEntityQuery.GetSingleton<CameraComponentData>();
        float deltaTime = SystemAPI.Time.DeltaTime;
        float desiredOrthographicSize = cameraComponentData.IsCenteredOnPlayer ? 20f : 120f;
        if (cameraComponentData.IsCenteredOnPlayer)
        {
            _playerCamera.Lens.OrthographicSize = math.lerp(_playerCamera.Lens.OrthographicSize, desiredOrthographicSize, deltaTime * 5f);
        }
        else
        {
            _playerCamera.Lens.OrthographicSize = math.lerp(_playerCamera.Lens.OrthographicSize, desiredOrthographicSize, deltaTime * 5f);
        }
        if (_playerCamera.Follow != null)
        {
            return;
        }

        var playerManagedComponentData = _playerEntityQuery.GetSingleton<PlayerManagedComponentData>();
        _playerCamera.Follow = playerManagedComponentData.TransformData;
    }
}
