using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.VisualScripting;

namespace AxieRescuer
{
    public partial struct SpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnBuffer>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawnerBuffer = SystemAPI.GetSingletonBuffer<SpawnBuffer>();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var spawnBufferArray = spawnerBuffer.ToNativeArray(state.WorldUpdateAllocator);

            new SpawnJob
            {
                SpawnBuffers = spawnBufferArray,
                ECB = ecb.AsParallelWriter(),
            }.ScheduleParallel(spawnBufferArray.Length, 1, state.Dependency).Complete();
            spawnerBuffer.Clear();
            ecb.Playback(state.EntityManager);
        }

    }
    [BurstCompile]
    public partial struct SpawnJob : IJobFor
    {
        public NativeArray<SpawnBuffer> SpawnBuffers;
        public EntityCommandBuffer.ParallelWriter ECB;
        public void Execute([EntityIndexInQuery]int index)
        {
            var entity = ECB.Instantiate(index,SpawnBuffers[index].Entity);
            ECB.SetComponent(index,entity, SpawnBuffers[index].Transform);
        }
    }
}

