using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class ZombieAuthoring : MonoBehaviour
    {
        private class ZombieBaker : Baker<ZombieAuthoring>
        {
            public override void Bake(ZombieAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Zombie());
            }
        }
    }

    public struct Zombie : IComponentData
    {
    }
}