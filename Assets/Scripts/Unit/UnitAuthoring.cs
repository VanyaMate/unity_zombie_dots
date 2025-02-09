using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class UnitAuthoring : MonoBehaviour
    {
        public Faction faction;

        private class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit { faction = authoring.faction });
            }
        }
    }

    public struct Unit : IComponentData
    {
        public Faction faction;
    }
}