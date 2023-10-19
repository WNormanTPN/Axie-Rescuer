using AxieRescuer;
using System.Numerics;
using Unity.Burst;
using Unity.Entities;
using Unity.Logging;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public partial struct RotateWeaponSystem : ISystem
{
    private EntityQuery _query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder()
            .WithAll<DroppedItem>()
            .WithAll<WeaponObjectReference>()
            .Build();
    }

    public void OnUpdate(ref SystemState state)
    {
        var job = new RotateWeaponJob
        {

        };
        job.RunByRef(_query);
    }
}

public partial struct RotateWeaponJob : IJobEntity
{
    public void Execute(WeaponObjectReference objectReference, EnabledRefRO<DroppedItem> droppedItem)
    {
        if (droppedItem.ValueRO == false) return;
        UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 1f, 0);
        objectReference.Value.transform.Rotate(rotation);
    }
}