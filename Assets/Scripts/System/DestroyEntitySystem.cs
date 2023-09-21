using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        private EntityQuery _needDestroyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _needDestroyQuery = SystemAPI.QueryBuilder()
                .WithAll<NeedDestroy>()
                .Build();
            state.RequireForUpdate(_needDestroyQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            new DestroyJob
            {
                DeltaTime = deltaTime,
                ECB = ecb.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency).Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct DestroyJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public void Execute
        (
            ref NeedDestroy needDestroy,
            in Entity entity,
            [EntityIndexInQuery] int sortKey
        )
        {
            if (needDestroy.CountdownTime > 0)
            {
                needDestroy.CountdownTime -= DeltaTime;
            }
            else
            {
                ECB.DestroyEntity(sortKey, entity);
            }
        }
    }
}