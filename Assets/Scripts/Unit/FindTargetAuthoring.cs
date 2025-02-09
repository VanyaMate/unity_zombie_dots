using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public float range;
        public Faction targetFaction;
        public float timerMax;

        private class FindTargetBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget
                {
                    range = authoring.range,
                    targetFaction = authoring.targetFaction,
                    timerMax = authoring.timerMax,
                    timer = 0,
                });
            }
        }
    }

    public struct FindTarget : IComponentData
    {
        public float range;
        public Faction targetFaction;
        public float timer;
        public float timerMax;
    }
}