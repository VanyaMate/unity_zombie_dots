using Unity.Entities;
using Unity.Mathematics;

namespace Movement
{
    public struct UnitMovementComponent : IComponentData
    {
        public float moveSpeed;
        public float rotationSpeed;
        public float3 targetPosition;
    }
}