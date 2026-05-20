using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MovingEnemyAISystem : ISystem
{
    private EntityQuery _enemyEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _enemyEntityQuery = SystemAPI.QueryBuilder().WithAll<EnemyManagedComponentData>().WithAll<MovingEnemyComponentData>().WithAll<LocalTransform>().WithAll<PhysicsCollider>().Build();
        state.RequireForUpdate(_enemyEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = _enemyEntityQuery.ToEntityArray(Allocator.Temp);
        var ecb = GetEntityCommandBuffer(ref state);
        foreach (Entity entity in enemies)
        {
            var enemyManagedComponentData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(entity);
            var enemyMovingComponentData = state.EntityManager.GetComponentData<MovingEnemyComponentData>(entity);
            var localTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);

            if(enemyMovingComponentData.State == MovingEnemyState.Move)
            {
                Move(ref state, enemyManagedComponentData, enemyMovingComponentData, entity, localTransform, ecb);

                if(IsGrounded(ref state, enemyMovingComponentData, entity, localTransform))
                    continue;

                SetIdle(ref state, enemyManagedComponentData, enemyMovingComponentData, entity, ecb);
                continue;
            }

            Idle(ref state, enemyManagedComponentData, enemyMovingComponentData, entity, localTransform, ecb);
        }
    }

    private unsafe bool IsGrounded(ref SystemState state, MovingEnemyComponentData enemyMovingComponentData, Entity entity, LocalTransform localTransform)
    {
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        float3 position = localTransform.Position;

        var physicsCollider = state.EntityManager.GetComponentData<PhysicsCollider>(entity);
        var boxCollider = (BoxCollider*)physicsCollider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;
        float3 halfExtents = boxGeometry.Size / 2f;
        float xShift = halfExtents.x * math.sign(enemyMovingComponentData.MovementSpeed);

        var raycastInput = new RaycastInput
        {
            Start = position + new float3(xShift, 0, 0),
            End = position + new float3(xShift, -halfExtents.y * 2, 0),
            Filter = enemyMovingComponentData.GroundCollisionFilter
        };
        return collisionWorld.CastRay(raycastInput);
    }

    private void Move(ref SystemState state, EnemyManagedComponentData enemyManagedComponentData, MovingEnemyComponentData enemyMovingComponentData, Entity entity, LocalTransform localTransform, EntityCommandBuffer ecb)
    {
        float3 position = localTransform.Position + enemyMovingComponentData.MovementSpeed * SystemAPI.Time.DeltaTime * new float3(1, 0, 0);
        ecb.SetComponent(entity, localTransform.WithPosition(position));
        enemyManagedComponentData.Transform.position = position;
    }
    private void SetMove(ref SystemState state, EnemyManagedComponentData enemyManagedComponentData, MovingEnemyComponentData enemyMovingComponentData, Entity entity, LocalTransform localTransform, EntityCommandBuffer ecb)
    {
        enemyManagedComponentData.Animator.SetBool("Move", true);
        enemyMovingComponentData.State = MovingEnemyState.Move;
        enemyMovingComponentData.MovementSpeed *= -1;

        ecb.SetComponent(entity, enemyMovingComponentData);
        localTransform.Rotation = quaternion.RotateY(180);
        enemyManagedComponentData.Transform.Rotate(0, 180, 0);
        ecb.SetComponent(entity, new LocalTransform { Position = localTransform.Position, Rotation = localTransform.Rotation, Scale = localTransform.Scale });
    }
    private void Idle(ref SystemState state, EnemyManagedComponentData enemyManagedComponentData, MovingEnemyComponentData enemyMovingComponentData, Entity entity, LocalTransform localTransform, EntityCommandBuffer ecb)
    {
        double timeToEndIdleState = enemyMovingComponentData.IdleFinishTime - SystemAPI.Time.ElapsedTime;

        if (timeToEndIdleState <= 0)
        {
            SetMove(ref state, enemyManagedComponentData, enemyMovingComponentData, entity, localTransform, ecb);
        }
    }
    private void SetIdle(ref SystemState state, EnemyManagedComponentData enemyManagedComponentData, MovingEnemyComponentData enemyMovingComponentData, Entity entity, EntityCommandBuffer ecb)
    {
        enemyManagedComponentData.Animator.SetBool("Move", false);
        enemyMovingComponentData.State = MovingEnemyState.Idle;
        enemyMovingComponentData.IdleFinishTime = SystemAPI.Time.ElapsedTime + enemyMovingComponentData.IdleTime;
        ecb.SetComponent(entity, enemyMovingComponentData);
    }

    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
