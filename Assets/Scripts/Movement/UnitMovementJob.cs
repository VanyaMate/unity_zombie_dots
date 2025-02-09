using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Movement
{
    [BurstCompile]
    public partial struct UnitMovementJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(
            ref LocalTransform localTransform,
            in UnitMovementComponent movementComponent,
            ref PhysicsVelocity physicsVelocity
        )
        {
            float3 moveDirection = movementComponent.targetPosition - localTransform.Position;
            float reachTargetDistance = 2f;

            if (math.lengthsq(moveDirection) < reachTargetDistance)
            {
                physicsVelocity.Linear = float3.zero;
                physicsVelocity.Angular = float3.zero;
                return;
            }

            float3 normalizedMoveDirection = math.normalize(moveDirection);

            localTransform.Rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(normalizedMoveDirection, math.up()),
                this.deltaTime * movementComponent.rotationSpeed
            );

            physicsVelocity.Linear = normalizedMoveDirection * movementComponent.moveSpeed;
            physicsVelocity.Angular = float3.zero;
        }
    }
}