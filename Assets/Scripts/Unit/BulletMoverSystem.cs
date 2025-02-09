using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Unit
{
    public partial struct BulletMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (
                (
                    RefRW<LocalTransform> localTransform,
                    RefRO<Bullet> bullet,
                    RefRO<Target> target,
                    Entity entity
                ) in SystemAPI
                    .Query<
                        RefRW<LocalTransform>,
                        RefRO<Bullet>,
                        RefRO<Target>>()
                    .WithEntityAccess()
            )
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                    continue;
                }

                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
                float3 hitTarget = targetLocalTransform.TransformPoint(shootVictim.hitLocalPosition);
                float3 moveDirection = hitTarget - localTransform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);


                float distanceToMove = bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;
                float distanceToTarget =
                    math.distancesq(localTransform.ValueRO.Position, hitTarget);

                float destroyDistance = 0f;
                if (distanceToTarget - distanceToMove < destroyDistance)
                {
                    RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    health.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                    entityCommandBuffer.DestroyEntity(entity);
                }
                else
                {
                    localTransform.ValueRW.Position += moveDirection * distanceToMove;
                }
            }
        }
    }
}