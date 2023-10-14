using AxieRescuer;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Search;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct TakeItemSystem : ISystem
    {
        private EntityQuery _query;
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<DroppedItem>()
                .WithAll<LocalTransform>()
                .WithAll<WeaponObjectReference>()
                .WithDisabled<NeedDestroy>()
                .Build();
            state.RequireForUpdate(_query);
        }
        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var job = new TakeWeaponJob
            {
                ECB = ecb,
                PlayerTransform = playerTransform,
                Player = player,
            };
            //foreach (var (droppedItem, localTransform, needDestroy, weaponPrefab, entity) in SystemAPI.Query<EnabledRefRW<DroppedItem>, LocalTransform, EnabledRefRW<NeedDestroy>, WeaponPrefab>().WithEntityAccess())
            //{
            //    Log.Debug("t");
            //    if (math.distance(playerTransform.Position, localTransform.Position) < 10)
            //    {
            //        Log.Debug("vinh");
            //        droppedItem.ValueRW = false;
            //        GameObject.Destroy(weaponPrefab.Value);
            //        needDestroy.ValueRW = true;
            //        ecb.SetComponent(player, new InitialWeapon
            //        {
            //            WeaponEntity = entity,
            //        });
            //        ecb.SetComponentEnabled<InitialWeapon>(player, true);
            //    }
            //}
            job.RunByRef(_query);
            ecb.Playback(state.EntityManager);
        }

    }
    public partial struct TakeWeaponJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public LocalTransform PlayerTransform;
        public Entity Player;
        public void Execute(EnabledRefRW<DroppedItem> droppedItem,
            in LocalTransform localTransform,
            EnabledRefRW<NeedDestroy> needDestroy,
            WeaponObjectReference weaponPrefab,
            in Entity entity)
        {
            if (math.distance(PlayerTransform.Position,localTransform.Position) < 2)
            {

                droppedItem.ValueRW = false;
                GameObject.Destroy(weaponPrefab.Value);
                //needDestroy.ValueRW = true;
                ECB.SetComponent(Player, new InitialWeapon
                {
                    WeaponEntity = entity,
                });
                ECB.SetComponentEnabled<InitialWeapon>(Player, true);
            }
        }
    }
}