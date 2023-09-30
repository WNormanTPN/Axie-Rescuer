using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using System;
using AxieRescuer;
using System.Globalization;
using UnityEngine.AI;
using Unity.Jobs;
using Unity.VisualScripting;
using Unity.Logging;
using Unity.Burst;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct SpawnZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnBuffer>();
            state.RequireForUpdate<RandomSingleton>();
        }
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var count = 0;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var spawnerEntity = SystemAPI.GetSingletonEntity<SpawnBuffer>();
            var prefab = SystemAPI.GetSingleton<SpawnZombieComponent>();
            var random = SystemAPI.GetSingleton<RandomSingleton>();
            for (int i = 0; i < prefab.Value; i++)
            {
                
                float3 randomPoint = new float3
                {
                    x = random.Random.NextFloat(prefab.StartMap.x, prefab.EndMap.x),
                    y = playerTransform.Position.y,
                    z = random.Random.NextFloat(prefab.StartMap.y, prefab.EndMap.y)
                };
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 40000f, NavMesh.AllAreas))
                {
                    count++;
                    ecb.AppendToBuffer(spawnerEntity, new SpawnBuffer
                    {
                        Entity = prefab.Entity,
                        Transform = new LocalTransform
                        {
                            Position = hit.position + new Vector3(0,0.3f,0),
                            Rotation = quaternion.identity,
                            Scale = 1f
                        }
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}


