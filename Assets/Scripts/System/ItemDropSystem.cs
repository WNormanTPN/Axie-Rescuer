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
        state.RequireForUpdate(Query);
    }
    public void OnUpdate(ref SystemState state) 
    {
        
    }
    
}

public partial struct DropItemJob : IJobEntity
{
    public NativeList<Entity> WeaponList;
    public RandomSingleton RandomSingleton;
    public EntityCommandBuffer.ParallelWriter ECB;
    public void Execute
    (
        in LocalTransform transform,
        in DropRate rate,
        [EntityIndexInQuery] int sortKey
    )
    {
        var percent = RandomSingleton.Random.NextFloat(0, 1);
        if (percent > rate.Value) return;
        var weaponEntity = WeaponList[RandomSingleton.Random.NextInt(0, WeaponList.Length - 1)];
        weaponEntity = ECB.Instantiate(sortKey, weaponEntity);
        ECB.SetComponent(sortKey, weaponEntity, transform);
        ECB.SetComponentEnabled<DroppedItem>(sortKey, weaponEntity, true);
    }
}