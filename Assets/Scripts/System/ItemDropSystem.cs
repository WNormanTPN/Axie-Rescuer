using AxieRescuer;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;
namespace AxieRescuer
{
    public partial struct ItemDropSystem : ISystem
    {
        public EntityQuery Query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomSingleton>();
            state.RequireForUpdate<WeaponsBuffer>();
            Query = SystemAPI.QueryBuilder().
                WithAll<IsDie>().
                WithAll<LocalTransform>().
                WithAll<DropRate>().
                Build();
            state.RequireForUpdate(Query);
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var resourceEntity = SystemAPI.GetSingletonEntity<WeaponsBuffer>();
            var randomSingleton = SystemAPI.GetSingleton<RandomSingleton>();
            var weaponBuffer = SystemAPI.GetBuffer<WeaponsBuffer>(resourceEntity);
            NativeList<Entity> weaponList = new NativeList<Entity>(weaponBuffer.Length, state.WorldUpdateAllocator);
            foreach (var weapon in weaponBuffer)
            {
                weaponList.Add(weapon.Value);
            }
            new DropItemJob
            {
                ECB = ecb,
                RandomSingleton = randomSingleton,
                WeaponList = weaponList,
            }.Schedule(Query,state.Dependency).Complete();
            ecb.Playback(state.EntityManager);
        }

    }
    [BurstCompile]
    public partial struct DropItemJob : IJobEntity
    {
        [ReadOnly] public NativeList<Entity> WeaponList;
        public RandomSingleton RandomSingleton;
        public EntityCommandBuffer ECB;
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
            weaponEntity = ECB.Instantiate(weaponEntity);
            ECB.SetComponent(weaponEntity, transform);
            ECB.SetComponentEnabled<WeaponNeedInitTag>(weaponEntity, true);
            ECB.SetComponentEnabled<DroppedItem>( weaponEntity, true);
        }
    }
}