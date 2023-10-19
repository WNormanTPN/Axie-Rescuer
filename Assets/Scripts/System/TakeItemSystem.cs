using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct TakeItemSystem : ISystem
    {
        private EntityQuery _query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<DroppedItem>()
                .WithAll<LocalTransform>()
                .WithAll<WeaponObjectReference>()
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
            WeaponObjectReference weaponPrefab,
            in Entity entity)
        {
            if (math.distance(PlayerTransform.Position,localTransform.Position) < 2)
            {

                droppedItem.ValueRW = false;
                GameObject.Destroy(weaponPrefab.Value);
                ECB.SetComponent(Player, new InitialWeapon
                {
                    WeaponEntity = entity,
                });
                ECB.SetComponentEnabled<InitialWeapon>(Player, true);
            }
        }
    }
}