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

namespace AxieRescuer
{
    public partial struct SpawnZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnEnemyTag>();
            state.RequireForUpdate<SpawnBuffer>();
        }
        public void OnUpdate(ref SystemState state)
        {
            var count = 0;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnerEntity = SystemAPI.GetSingletonEntity<SpawnBuffer>();
            var prefab = SystemAPI.GetSingleton<SpawnZombieComponent>();
            for (int i = 0; i < 30; i++)
            {
                float3 randomPoint = float3.zero ;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    count++;
                    ecb.AppendToBuffer(spawnerEntity, new SpawnBuffer
                    {
                        Entity = prefab.Entity,
                        Transform = new LocalTransform
                        {
                            Position = hit.position,
                            Rotation = quaternion.identity,
                            Scale = 1f
                        }
                    });
                }
            }
            ecb.Playback(state.EntityManager);
            if (count >= 30)
            {
                state.Enabled = false;
            }
        }
    }
}

