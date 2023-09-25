using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;

namespace AxieRescuer
{
    public partial struct ApplyDamageSystem : ISystem
    {
        private EntityQuery _damageReceiverQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _damageReceiverQuery = SystemAPI.QueryBuilder()
                .WithAll<Health>()
                .WithAll<DamageReceived>()
                .Build();
            state.RequireForUpdate(_damageReceiverQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            new ApplyDamageJob
            {
                ECB = ecb.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency).Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct ApplyDamageJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute
        (
            ref Health health,
            ref DynamicBuffer<DamageReceived> damageReceiveds,
            in Entity entity,
            [EntityIndexInQuery] int sortKey
        )
        {
            foreach (var damageReceived in damageReceiveds)
            {
                health.Current -= damageReceived.Value;
                if (health.Current <= 0)
                {
                    ECB.SetComponentEnabled<IsDie>(sortKey, entity, true);
                }
            }
            damageReceiveds.Clear();
        }
    }
}