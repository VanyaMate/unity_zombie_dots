using Assets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Unit
{
    public partial struct FindTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach (
                (
                    RefRO<LocalTransform> localTransform,
                    RefRW<FindTarget> findTarget,
                    RefRW<Target> target
                ) in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<FindTarget>,
                    RefRW<Target>
                >()
            )
            {
                findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.timer > 0f)
                {
                    continue;
                }

                findTarget.ValueRW.timer = findTarget.ValueRW.timerMax;


                distanceHits.Clear();

                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                    BelongsTo = ~0u,
                    GroupIndex = 0
                };

                if (
                    collisionWorld.OverlapSphere(
                        localTransform.ValueRO.Position,
                        findTarget.ValueRO.range,
                        ref distanceHits,
                        collisionFilter
                    )
                )
                {
                    foreach (DistanceHit distanceHit in distanceHits)
                    {
                        if (SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                        {
                            Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                            if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                            {
                                target.ValueRW.targetEntity = distanceHit.Entity;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}