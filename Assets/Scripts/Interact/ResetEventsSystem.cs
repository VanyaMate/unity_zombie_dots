using Unity.Burst;
using Unity.Entities;

namespace Interact
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.onSelected = false;
                selected.ValueRW.onDeselected = false;
            }
        }
    }
}