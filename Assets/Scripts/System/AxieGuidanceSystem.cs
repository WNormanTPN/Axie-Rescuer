using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct AxieGuidanceSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FollowingAxie>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            if (!state.EntityManager.HasComponent<FollowingAxie>(player)) return;
            if (!state.EntityManager.IsComponentEnabled<FollowingAxie>(player)) return;
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var followingAxie = SystemAPI.GetComponent<FollowingAxie>(player);
            var axieTransform = SystemAPI.GetComponent<LocalTransform>(followingAxie.Entity);
            var agentBody = SystemAPI.GetComponentRW<AgentBody>(followingAxie.Entity);
            if (math.distance(playerTransform.Position, axieTransform.Position) > 10)
            {
                agentBody.ValueRW.SetDestination(playerTransform.Position);
            }
        }
    }
}