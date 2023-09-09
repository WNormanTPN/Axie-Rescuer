using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using static Unity.Entities.SystemAPI;

namespace ProjectDawn.Navigation.Sample.Zerg
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AgentSystemGroup))]
    [UpdateAfter(typeof(AgentTransformSystemGroup))]
    public partial struct AgentSmartStopSystem : ISystem
    {
        ComponentLookup<AgentSmartStop> m_SmartStopLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_SmartStopLookup = state.GetComponentLookup<AgentSmartStop>(isReadOnly: true);
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spatial = GetSingleton<AgentSpatialPartitioningSystem.Singleton>();

            m_SmartStopLookup.Update(ref state);

            new AgentSmartStopJob
            {
                Spatial = spatial,
                SmartStopLookup = m_SmartStopLookup,
            }.Schedule();
        }

        [BurstCompile]
        partial struct AgentSmartStopJob : IJobEntity
        {
            [ReadOnly]
            public AgentSpatialPartitioningSystem.Singleton Spatial;
            [ReadOnly]
            public ComponentLookup<AgentSmartStop> SmartStopLookup;

            public void Execute(Entity entity, ref AgentBody body, in AgentSmartStop smartStop, in LocalTransform transform)
            {
                if (body.IsStopped)
                    return;

                // This is just a high performance foreach for nearby agents
                // It is basically as: foreach (var nearbyAgent in GetNearbyAgents()) Spatial.Execute(...)
                // For each nearby agent check if they reached destination
                var action = new FindTargetAction
                {
                    SmartStopLookup = SmartStopLookup,
                    Entity = entity,
                    Body = body,
                    SmartStop = smartStop,
                    Transform = transform,
                };
                Spatial.QuerySphere(transform.Position, smartStop.HiveMindStop.Radius, ref action);

                // If any nearby agent reached destination, this agent should stop too
                if (!action.Stop)
                    return;

                body.Stop();
            }

            [BurstCompile]
            struct FindTargetAction : ISpatialQueryEntity
            {
                [ReadOnly]
                public ComponentLookup<AgentSmartStop> SmartStopLookup;

                public Entity Entity;
                public AgentBody Body;
                public AgentSmartStop SmartStop;
                public LocalTransform Transform;

                // Output if this agent should stop
                public bool Stop;

                public void Execute(Entity entity, AgentBody body, AgentShape shape, LocalTransform transform)
                {
                    // Exclude itself
                    if (Entity == entity)
                        return;

                    // Exclude still moving ones
                    if (!body.IsStopped)
                        return;

                    // Check if they collide
                    float distance = math.distance(Transform.Position, transform.Position);
                    if (SmartStop.HiveMindStop.Radius < distance)
                        return;

                    // Check if neaby one has smart stop
                    if (!SmartStopLookup.TryGetComponent(entity, out AgentSmartStop brain) || !brain.HiveMindStop.Enabled)
                        return;

                    // Check if they have similar destinations within the radius
                    float distance2 = math.distance(Body.Destination, body.Destination);
                    if (SmartStop.HiveMindStop.Radius < distance2)
                        return;

                    Stop = true;
                }
            }
        }
    }
}
