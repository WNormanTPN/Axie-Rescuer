using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]

    public partial struct ZombieDestroySystem : ISystem
    {
        public EntityQuery ZombieDieQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            ZombieDieQuery = SystemAPI.QueryBuilder()
                .WithAll<IsDie>()
                .WithAll<ZombieTag>()
                .WithDisabled<NeedDestroy>()
                .Build();
            state.RequireForUpdate(ZombieDieQuery);
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            new ZombieDestroyJob
            {
                ECB = ecb.AsParallelWriter(),
            }.ScheduleParallel(ZombieDieQuery, state.Dependency).Complete();
            ecb.Playback(state.EntityManager);
        } 
    }
    public partial struct ZombieDestroyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public void Execute(
            [EntityIndexInQuery] int index,
            in Entity entity)
        {
            ECB.SetComponentEnabled<NeedDestroy>(index,entity,true);
        }
    }
}