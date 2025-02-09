using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Interact
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    public partial struct SelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                if (selected.ValueRO.onSelected)
                {
                    RefRW<LocalTransform> localTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    localTransform.ValueRW.Scale = selected.ValueRO.showScale;
                }
                else if (selected.ValueRO.onDeselected)
                {
                    RefRW<LocalTransform> localTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    localTransform.ValueRW.Scale = 0f;
                }
            }
        }
    }
}