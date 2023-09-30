using AxieRescuer;
using ProjectDawn.Navigation;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Transforms;
using UnityEngine;

public partial struct FindTargetSystem : ISystem
{
    public EntityQuery query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = SystemAPI.QueryBuilder().
            WithAll<ZombieTag>().
            WithAll<LocalTransform>().
            WithAll<AgentBody>().
            WithAll<FindTargetComponents>().
            Build();
        state.RequireForUpdate(query);
        state.RequireForUpdate<PlayerTag>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        new FindTargetJob
        {
            PlayerPosition = SystemAPI.GetComponent<LocalTransform>(player)
        }.ScheduleParallel(query);
    }
}
[BurstCompile]
public partial struct FindTargetJob : IJobEntity
{
    public LocalTransform PlayerPosition;
    [BurstCompile]
    private void Execute(
        ref LocalTransform transform,
        ref AgentBody body,
        ref FindTargetComponents find
        )
    {
        if (math.distancesq(transform.Position,PlayerPosition.Position) < 300)
        {
            find.onRange = true;
        }
        if (find.onRange)
        {
            body.SetDestination(PlayerPosition.Position);
        }
    }

}