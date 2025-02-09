using Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Unit
{
    public partial struct ShootAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (
                (
                    RefRW<LocalTransform> localTransform,
                    RefRW<ShootAttack> shootAttack,
                    RefRO<Target> target,
                    RefRW<UnitMovementComponent> movementComponent
                )
                in SystemAPI
                    .Query<
                        RefRW<LocalTransform>,
                        RefRW<ShootAttack>,
                        RefRO<Target>,
                        RefRW<UnitMovementComponent>
                    >()
            )
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    continue;
                }

                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) >
                    shootAttack.ValueRO.attackDistance)
                {
                    movementComponent.ValueRW.targetPosition = targetLocalTransform.Position;
                    continue;
                }
                else
                {
                    movementComponent.ValueRW.targetPosition = localTransform.ValueRO.Position;
                }

                float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(aimDirection, math.up()),
                    SystemAPI.Time.DeltaTime * movementComponent.ValueRO.rotationSpeed);

                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.timer > 0f)
                {
                    continue;
                }

                shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

                Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
                float3 bulletSpawnWorldPosition =
                    localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

                RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
            }
        }
    }
}