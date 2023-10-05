using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ZombieInitializeSystem : ISystem
    {
        private EntityQuery _needInitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _needInitQuery = SystemAPI.QueryBuilder()
                .WithAll<ZombieNeedInitTag>()
                .WithAll<CharacterGameObjectPrefab>()
                .Build();
            state.RequireForUpdate(_needInitQuery);
            state.RequireForUpdate<RandomSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (StaticGameObjectReference.ZombiePrefabs == null || StaticGameObjectReference.ZombiePrefabs.Count == 0) return;
            var random = SystemAPI.GetSingleton<RandomSingleton>().Random;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (needInit, characterPrefab) in SystemAPI.Query<EnabledRefRW<ZombieNeedInitTag>, CharacterGameObjectPrefab>())
            {
                characterPrefab.Value = StaticGameObjectReference.ZombiePrefabs[random.NextInt(0, StaticGameObjectReference.ZombiePrefabs.Count)];
                needInit.ValueRW = false;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}