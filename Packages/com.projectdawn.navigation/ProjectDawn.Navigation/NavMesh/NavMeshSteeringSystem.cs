using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using UnityEngine.Experimental.AI;
using Unity.Collections.LowLevel.Unsafe;
using static Unity.Entities.SystemAPI;

namespace ProjectDawn.Navigation
{
    /// <summary>
    /// System that steers agents within the NavMesh path.
    /// </summary>
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AgentSystemGroup))]
    [UpdateAfter(typeof(AgentSteeringSystemGroup))]
    [UpdateBefore(typeof(AgentForceSystemGroup))]
    public partial struct NavMeshSteeringSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var navmesh = GetSingleton<NavMeshQuerySystem.Singleton>();
            new NavMeshSteeringJob
            {
                NavMesh = navmesh,
            }.ScheduleParallel();
            navmesh.World.AddDependency(state.Dependency);
        }

        [BurstCompile]
        unsafe partial struct NavMeshSteeringJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            [ReadOnly]
            public NavMeshQuerySystem.Singleton NavMesh;

            [NativeDisableContainerSafetyRestriction]
            NavMeshFunnel Funnel;

            public void Execute(ref AgentBody body, ref NavMeshPath path, in LocalTransform transform, in DynamicBuffer<NavMeshNode> nodes)
            {
                // Update current location if changed
                if (!NavMesh.IsValid(path.Location.polygon) || math.distancesq(transform.Position, (float3) path.Location.position) > 0.01f)
                {
                    NavMeshLocation location = NavMesh.MapLocation(transform.Position, path.MappingExtent, path.AgentTypeId, path.AreaMask);

                    // Handle case if failde to map location
                    if (location.polygon.IsNull())
                    {
                        UnityEngine.Debug.LogWarning("Failed to map agent position to nav mesh location. This can happen either if nav mesh is not present or property MappingExtent value is too low.");
                        return;
                    }

                    path.Location = location;
                }

                if (body.IsStopped)
                    return;

                // Skip if path is not finished
                if (path.State != NavMeshPathState.Finished && path.State != NavMeshPathState.Failed)
                {
                    // TODO: Check would be best way to handle un finished path
                    // Now it continues to move to destination without accounting navmesh
                    //body.Force = 0;
                    //destination.RemainingDistance = 0;
                    return;
                }

                // Update end location if changed
                if (!NavMesh.IsValid(path.EndLocation.polygon) || math.distancesq(body.Destination, (float3) path.EndLocation.position) > 0.01f)
                {
                    NavMeshLocation location = NavMesh.MapLocation(body.Destination, path.MappingExtent, path.AgentTypeId, path.AreaMask);

                    // Handle case if failde to map location
                    if (location.polygon.IsNull())
                    {
                        UnityEngine.Debug.LogWarning("Failed to map agent destination to nav mesh location. This can happen either if nav mesh is not present or property MappingExtent value is too low.");
                    }

                    // Update destination to avoid mapping location again
                    body.Destination = location.position;

                    // Handle the case if destination is no longer within the path
                    if (path.EndLocation.polygon != location.polygon)
                    {
                        path.State = NavMeshPathState.WaitingNewPath;
                        return;
                    }
                }

                // If path failed simply stop
                if (path.State == NavMeshPathState.Failed)
                {
                    body.Stop();
                    return;
                }

                var polygons = nodes.AsNativeArray().Reinterpret<PolygonId>();

                // With empty polygons we can assume destination reached
                if (polygons.Length == 0)
                {
                    body.Stop();
                    return;
                }

                // Path nodes only contains optimal path using navigation mesh polygons
                // Here we create the corridor from those polygons
                // It basically finds the shortest path using polygons vertices (a.k.a. corners)
                if (NavMesh.TryCreateFunnel(ref Funnel, polygons, transform.Position, body.Destination))
                {
                    var locations = Funnel.AsLocations();
                    if (locations.Length > 1)
                    {
                        body.Force = math.normalizesafe((float3) locations[1].position - transform.Position);
                        body.RemainingDistance = Funnel.IsEndReachable ? Funnel.GetCornersDistance() : float.MaxValue;
                    }
                }
                else
                {
                    // If we can not create corridor from polygons, request new path
                    // Usually it can happen if some nodes are not connected
                    path.State = NavMeshPathState.InValid;
                }
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                // Create tunnel that will allow finding optimal path is navmesh nodes
                // Only 4 positions constructed as, because of local changes there is change path will need change
                Funnel = new NavMeshFunnel(4, Allocator.Temp);
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            {
                Funnel.Dispose();
            }
        }
    }
}
