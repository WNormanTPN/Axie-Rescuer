using AxieRescuer;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct ItemDropSystem : ISystem
{
    public EntityQuery Query;
    public void OnCreate(ref SystemState state)
    {
        Query = SystemAPI.QueryBuilder().
            WithAll<IsDie>().
            WithAll<LocalTransform>().
            WithAll<DropRate>().
            Build();
    }
    public void OnUpdate(ref SystemState state) 
    {
        
    }
    
}

public partial struct DropItemJob : IJobEntity
{
    public NativeList<Entity> WeaponList;
    public Random
    public void Execute
    (
        in LocalTransform transform,
        in DropRate rate
    )
    {

    }
}