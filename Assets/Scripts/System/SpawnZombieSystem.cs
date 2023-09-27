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
            for (int i = 0; i < 30; i++)
            {
                
                float3 randomPoint = new float3
                {
                    x = random.Random.NextFloat(playerTransform.Position.x -20f, playerTransform.Position.x + 20f),
                    y = playerTransform.Position.y,
                    z = random.Random.NextFloat(playerTransform.Position.z - 20f, playerTransform.Position.z + 20f)
                };
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
                {
                    Debug.Log("1");
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
            //if (count >= 30)
            //{
            //    state.Enabled = false;
            //}
        }
        public float3 NextInsideSphere(Unity.Mathematics.Random rand)
        {
            var phi = rand.NextFloat(2 * math.PI);
            var theta = math.acos(rand.NextFloat(-1f, 1f));
            var r = math.pow(rand.NextFloat(), 1f / 3f);
            var x = math.sin(theta) * math.cos(phi);
            var y = math.sin(theta) * math.sin(phi);
            var z = math.cos(theta);
            return r * new float3(x, y, z);
        }
        public static float3 NextOnSphereSurfase(Unity.Mathematics.Random rand)
        {
            var phi = rand.NextFloat(2 * math.PI);
            var theta = math.acos(rand.NextFloat(-1f, 1f));
            var x = math.sin(theta) * math.cos(phi);
            var y = math.sin(theta) * math.sin(phi);
            var z = math.cos(theta);
            return new float3(x, y, z);
        }
    }
}


