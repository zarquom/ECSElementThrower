using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct JumpingEnemyAISystem : ISystem
{
    private EntityQuery _enemyEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _enemyEntityQuery = SystemAPI.QueryBuilder().
            WithAll<EnemyManagedComponentData>().
            WithAll<JumpingEnemyComponentData>().
            WithAll<PhysicsCollider>().
            WithAll<PhysicsMass>().
            WithAll<PhysicsVelocity>().
            WithAll<LocalTransform>().Build();
        state.RequireForUpdate(_enemyEntityQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = _enemyEntityQuery.ToEntityArray(Allocator.Temp);
        var ecb = GetEntityCommandBuffer(ref state);
        foreach (Entity entity in enemies)
        {
            var enemyManagedComponentData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(entity);
            var enemyJumpingComponentData = state.EntityManager.GetComponentData<JumpingEnemyComponentData>(entity);
            var localTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);
            var physicsMass = state.EntityManager.GetComponentData<PhysicsMass>(entity);
            var physicsVelocity = state.EntityManager.GetComponentData<PhysicsVelocity>(entity);

            if (math.lengthsq(physicsVelocity.Linear) > 0.01f)
            {
                SyncManagedPosition(enemyManagedComponentData, localTransform);
                continue;
            }
            if (!IsGrounded(ref state, enemyJumpingComponentData, entity, localTransform))
            {
                continue;
            }
            if (enemyJumpingComponentData.State == JumpingEnemyState.Jump)
            {
                SetIdle(ref state, enemyManagedComponentData, enemyJumpingComponentData, entity, ecb);
                continue;
            }
            Idle(ref state, enemyManagedComponentData, enemyJumpingComponentData, entity, ref physicsVelocity, physicsMass, ecb);
        }
    }
    private void SyncManagedPosition(EnemyManagedComponentData enemyManagedComponentData, LocalTransform localTransform)
    {
        enemyManagedComponentData.Transform.position = localTransform.Position;
    }
    private unsafe bool IsGrounded(ref SystemState state, JumpingEnemyComponentData jumpingMovingComponentData, Entity entity, LocalTransform localTransform)
    {
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        float3 position = localTransform.Position;

        var physicsCollider = state.EntityManager.GetComponentData<PhysicsCollider>(entity);
        var boxCollider = (BoxCollider*)physicsCollider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;
        float3 halfExtents = boxGeometry.Size / 2f;

        var raycastInput = new RaycastInput
        {
            Start = position + new float3(0, 0, 0),
            End = position + new float3(0, -halfExtents.y * 1.5f, 0),
            Filter = jumpingMovingComponentData.GroundCollisionFilter
        };
        return collisionWorld.CastRay(raycastInput);
    }

    private void Jump(EnemyManagedComponentData enemyManagedComponentData, JumpingEnemyComponentData jumpingMovingComponentData, Entity entity, ref PhysicsVelocity physicsVelocity, in PhysicsMass physicsMass, EntityCommandBuffer ecb)
    {
        enemyManagedComponentData.Animator.SetTrigger("Jump");
        jumpingMovingComponentData.State = JumpingEnemyState.Jump;
        ecb.SetComponent(entity, jumpingMovingComponentData);
        physicsVelocity.ApplyLinearImpulse(physicsMass, new float3(0, jumpingMovingComponentData.JumpForce, 0));
        ecb.SetComponent(entity, physicsVelocity);
    }
    private void Idle(ref SystemState state,EnemyManagedComponentData enemyManagedComponentData, JumpingEnemyComponentData jumpingMovingComponentData, Entity entity, ref PhysicsVelocity physicsVelocity, in PhysicsMass physicsMass, EntityCommandBuffer ecb)
    {
        double timeToEndIdleState = jumpingMovingComponentData.IdleFinishTime - SystemAPI.Time.ElapsedTime;

        if (timeToEndIdleState <= 0)
        {
            Jump(enemyManagedComponentData, jumpingMovingComponentData, entity, ref physicsVelocity, physicsMass, ecb);
        }
    }
    private void SetIdle(ref SystemState state, EnemyManagedComponentData enemyManagedComponentData, JumpingEnemyComponentData jumpingMovingComponentData, Entity entity, EntityCommandBuffer ecb)
    {
        jumpingMovingComponentData.State = JumpingEnemyState.Idle;
        jumpingMovingComponentData.IdleFinishTime = SystemAPI.Time.ElapsedTime + jumpingMovingComponentData.IdleTime;
        ecb.SetComponent(entity, jumpingMovingComponentData);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}
