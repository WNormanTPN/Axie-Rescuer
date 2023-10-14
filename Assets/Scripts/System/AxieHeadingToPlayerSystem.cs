using System.ComponentModel;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AxieRescuer
{
    public partial struct AxieHeaadingPlayerSystem : ISystem
    {
        public EntityQuery _wildAxieQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _wildAxieQuery = SystemAPI.QueryBuilder()
                .WithAll<WildAxieTag>()
                .WithAll<LocalTransform>()
                .Build();
            state.RequireForUpdate(_wildAxieQuery);
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerPos = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<PlayerTag>()).Position;
            new HeadingToPlayerJob
            {
                PlayerPos = playerPos,
            }.ScheduleParallel(_wildAxieQuery);
        }
    }

    [BurstCompile]
    public partial struct HeadingToPlayerJob : IJobEntity
    {
        public float3 PlayerPos;
        public void Execute(ref LocalTransform transform)
        {
            transform.Rotation = quaternion.LookRotation(PlayerPos - transform.Position, math.up());
        }
    }
}