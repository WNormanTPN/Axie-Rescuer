using Unity.Burst;
using Unity.Entities;

namespace AxieRescuer
{
    public partial struct AxieGuidanceSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FollowingAxie>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }
    }
}