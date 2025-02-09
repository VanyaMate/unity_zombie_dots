using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        public float timerMax;

        private class ZombieSpawnerBaker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ZombieSpawner
                {
                    timerMax = authoring.timerMax,
                    timer = 0
                });
            }
        }
    }

    public struct ZombieSpawner : IComponentData
    {
        public float timer;
        public float timerMax;
    }
}