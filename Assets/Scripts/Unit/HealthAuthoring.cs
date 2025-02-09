using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class HealthAuthoring : MonoBehaviour
    {
        public int healthAmount;

        private class HealthBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health
                {
                    healthAmount = authoring.healthAmount
                });
            }
        }
    }

    public struct Health : IComponentData
    {
        public int healthAmount;
    }
}