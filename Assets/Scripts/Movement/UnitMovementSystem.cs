using Unity.Burst;
using Unity.Entities;

namespace Movement
{
    public partial struct UnitMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new UnitMovementJob() { deltaTime = SystemAPI.Time.DeltaTime }.ScheduleParallel();
        }
    }
}