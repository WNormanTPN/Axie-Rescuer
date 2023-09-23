using Unity.Burst;
using Unity.Entities;

namespace AxieRescuer
{
    public partial struct UpdateRandomizerSystem : ISystem, ISystemStartStop
    {
        private Entity _randomEntity;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomSingleton>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            _randomEntity = SystemAPI.GetSingletonEntity<RandomSingleton>();
            var randomState = (uint)System.DateTime.Now.Ticks;
            var random = new Unity.Mathematics.Random(randomState);
            state.EntityManager.SetComponentData(_randomEntity, new RandomSingleton
            {
                Random = random,
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            var randomState = (uint)System.DateTime.Now.Ticks;
            var random = new Unity.Mathematics.Random(randomState);
            state.EntityManager.SetComponentData(_randomEntity, new RandomSingleton
            {
                Random = random,
            });
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}