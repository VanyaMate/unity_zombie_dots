using Unity.Entities;
using UnityEngine;

namespace Movement
{
    public class UnitMovementAuthoring : MonoBehaviour
    {
        public float moveSpeed = 0;
        public float rotationSpeed = 0;

        public class Baker : Baker<UnitMovementAuthoring>
        {
            public override void Bake(UnitMovementAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMovementComponent()
                {
                    moveSpeed = authoring.moveSpeed,
                    rotationSpeed = authoring.rotationSpeed,
                    targetPosition = Vector3.zero,
                });
            }
        }
    }
}