using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AxieRescuer
{
    public partial struct ZombieAttackSystem : ISystem
    {
        private EntityQuery zombieQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            zombieQuery = SystemAPI.QueryBuilder()
                .WithAll<ZombieTag>()
                .WithAll<ZombieAttackTimer>()
                .WithAll<Damage>()
                .WithAll<LocalTransform>()
                .Build();
            state.RequireForUpdate<DamageReceived>();
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<RandomSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;
            var random = SystemAPI.GetSingleton<RandomSingleton>().Random;
            var deltaTime = SystemAPI.Time.DeltaTime;
            var damageList = new NativeList<int>(zombieQuery.CalculateEntityCountWithoutFiltering(), state.WorldUpdateAllocator);
            var job = new AttackPlayerJob
            {
                PlayerPos = playerPos,
                Random = random,
                DeltaTime = deltaTime,
                DamageList = damageList.AsParallelWriter(),
            };
            job.ScheduleParallelByRef(state.Dependency).Complete();

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var damage in damageList)
            {
                ecb.AppendToBuffer(player, new DamageReceived { Value = damage });
            }
            ecb.Playback(state.EntityManager);
        }
    }
    [BurstCompile]
    public partial struct AttackPlayerJob : IJobEntity
    {
        public float3 PlayerPos;
        public Random Random;
        public float DeltaTime;
        [WriteOnly] public NativeList<int>.ParallelWriter DamageList;

        public void Execute(ref LocalTransform transform, ref ZombieAttackTimer timer, in Damage damage)
        {
            if (math.distance(PlayerPos, transform.Position) < 3)
            {
                timer.Value += DeltaTime;
                if (timer.Value > 2)
                {
                    DamageList.AddNoResize(Random.NextInt((int)damage.MinValue, (int)damage.MaxValue));
                    timer.Value = 0;
                }
            }
            else
            {
                timer.Value = 0;
            }
        }
    }
}