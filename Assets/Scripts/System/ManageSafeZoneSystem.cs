using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct ManageSafeZoneSystem : ISystem
    {
        private float timer;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timer = 0;
            state.RequireForUpdate<SafeZone>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var safezone = SystemAPI.GetSingletonEntity<SafeZone>();
            var safezoneRadius = SystemAPI.GetComponent<SafeZone>(safezone).Radius;
            var safezoneTransform = SystemAPI.GetComponent<LocalTransform>(safezone);
            var dis = math.distance(playerTransform.Position, safezoneTransform.Position) - safezoneRadius;
            if(dis < 0)
            {
                timer += SystemAPI.Time.DeltaTime;
                if (timer > 2)
                {
                    timer = 0;
                    var playerHealth = state.EntityManager.GetComponentData<Health>(player);
                    if (playerHealth.Current < playerHealth.Max)
                    {
                        playerHealth.Current = playerHealth.Max;
                    }
                    if(state.EntityManager.IsComponentEnabled<FollowingAxie>(player))
                    {
                        var followingAxie = SystemAPI.GetComponent<FollowingAxie>(player);
                        SystemAPI.SetComponentEnabled<FollowingAxie>(player, false);
                        state.EntityManager.DestroyEntity(followingAxie.Entity);
                    }
                }
            }
        }
    }
}