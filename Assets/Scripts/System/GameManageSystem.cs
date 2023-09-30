using Unity.Burst;
using Unity.Entities;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct GameManageSystem : ISystem, ISystemStartStop
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            state.EntityManager.SetComponentEnabled<InitialWeapon>(playerEntity, true);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}