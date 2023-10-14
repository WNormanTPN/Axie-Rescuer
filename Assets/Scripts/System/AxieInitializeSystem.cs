using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct AxieInitializeSystem : ISystem
    {
        private EntityQuery _axieQuery;
        private EntityQuery _buildingQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _axieQuery = SystemAPI.QueryBuilder()
                .WithAll<AxieNeedInitTag>()
                .WithAll<LocalTransform>()
                .WithAll<CharacterGameObjectPrefab>()
                .Build();
            state.RequireForUpdate(_axieQuery);

            _buildingQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform>()
                .WithAll<BuildingTag>()
                .Build();
            state.RequireForUpdate(_buildingQuery);
            state.RequireForUpdate<RandomSingleton>();
        }

        
        public unsafe void OnUpdate(ref SystemState state)
        {
            var random = SystemAPI.GetSingleton<RandomSingleton>().Random;
            var buildingTransform = _buildingQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
            foreach(var(transform, prefab, needInit) in SystemAPI.Query<RefRW<LocalTransform>, CharacterGameObjectPrefab, EnabledRefRW<AxieNeedInitTag>>())
            {
                needInit.ValueRW = false;
                var buildingIndex = random.NextInt(0, buildingTransform.Length);
                transform.ValueRW.Position = buildingTransform[buildingIndex].Position;
                var axieIndex = random.NextInt(0, StaticGameObjectReference.AxiePrefabs.Count);
                prefab.Value = StaticGameObjectReference.AxiePrefabs[axieIndex];
                CutoutObjectv2.TargetObjects.Add(transform.ValueRO);
            }
        }
    }
}