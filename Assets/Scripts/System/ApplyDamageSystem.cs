using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial struct ApplyDamageSystem : ISystem
    {
        private EntityQuery _damageReceiverQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _damageReceiverQuery = SystemAPI.QueryBuilder()
                .WithAll<Health>()
                .WithAll<DamageReceived>()
                .WithAll<LocalTransform>()
                .WithNone<PlayerTag>()
                .Build();
            state.RequireForUpdate(_damageReceiverQuery);
            state.RequireForUpdate<FollowOffset>();
            state.RequireForUpdate<PlayerTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var posAliveList = new NativeList<float3>(_damageReceiverQuery.CalculateEntityCountWithoutFiltering(), state.WorldUpdateAllocator);
            var healthAliveList = new NativeList<Health>(_damageReceiverQuery.CalculateEntityCountWithoutFiltering(), state.WorldUpdateAllocator);
            var angleOffset = SystemAPI.GetSingleton<FollowOffset>().Rotation.y;

            new ApplyDamageJob
            {
                ECB = ecb.AsParallelWriter(),
                PosAliveList = posAliveList.AsParallelWriter(),
                HealthAliveList = healthAliveList.AsParallelWriter(),
            }.ScheduleParallel(_damageReceiverQuery, state.Dependency).Complete();

            for(int i = 0; i < posAliveList.Length; i++)
            {
                if (posAliveList[i].Equals(float3.zero)) return;
                var healthBar = GameObject.Instantiate(StaticGameObjectReference.HealthBar);
                healthBar.transform.position = posAliveList[i] + math.up() * 3;
                healthBar.transform.eulerAngles += new Vector3(0, angleOffset, 0);
                var health = healthBar.GetComponentInChildren<Slider>();
                health.value = healthAliveList[i].Current / healthAliveList[i].Max;
                GameObject.Destroy(healthBar, 0.2f);
            }

            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerHealth = SystemAPI.GetComponentRW<Health>(player);
            var damageRecaivedBuffer = SystemAPI.GetBuffer<DamageReceived>(player);
            foreach (var damage in damageRecaivedBuffer.ToNativeArray(state.WorldUpdateAllocator))
            {
                playerHealth.ValueRW.Current -= damage.Value;
            }
            damageRecaivedBuffer.Clear();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct ApplyDamageJob : IJobEntity
    {
        public NativeList<float3>.ParallelWriter PosAliveList;
        public NativeList<Health>.ParallelWriter HealthAliveList;
        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute
        (
            ref Health health,
            ref DynamicBuffer<DamageReceived> damageReceiveds,
            in LocalTransform transform,
            in Entity entity,
            [EntityIndexInQuery] int sortKey
        )
        {
            if(damageReceiveds.Length == 0) return;
            foreach (var damageReceived in damageReceiveds)
            {
                health.Current -= damageReceived.Value;
            }
            if (health.Current <= 0)
            {
                ECB.SetComponentEnabled<IsDie>(sortKey, entity, true);
            }
            else
            {
                PosAliveList.AddNoResize(transform.Position);
                HealthAliveList.AddNoResize(health);
            }
            damageReceiveds.Clear();
        }
    }
}