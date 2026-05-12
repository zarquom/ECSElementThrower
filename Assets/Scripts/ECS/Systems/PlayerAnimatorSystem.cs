using System.Globalization;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct PlayerAnimatorSystem :ISystem
{
    private EntityQuery _playerEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _playerEntityQuery = SystemAPI.QueryBuilder()
        .WithAll<LocalTransform>()
        .WithAll<PlayerMovementComponentData>()
        .WithAll<PlayerAnimationComponentData>()
        .Build();
        state.RequireForUpdate(_playerEntityQuery);
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerLocalTransform = _playerEntityQuery.GetSingleton<LocalTransform>();
        var playerMovementData = _playerEntityQuery.GetSingleton<PlayerMovementComponentData>();
        var playerAnimationData = _playerEntityQuery.GetSingleton<PlayerAnimationComponentData>();

        var transform = playerAnimationData.AnimatorData.transform;
        transform.position = playerLocalTransform.Position;
        transform.rotation = playerLocalTransform.Rotation;
        float scale = playerLocalTransform.Scale;
        transform.localScale = new UnityEngine.Vector3 (scale, scale, scale);

        if (playerMovementData.IsGrounded && playerMovementData.IsJump)
        {
            playerAnimationData.AnimatorData.SetTrigger("Jump");
        }
        playerAnimationData.AnimatorData.SetBool("Run", math.abs(playerMovementData.Direction) > 0);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
