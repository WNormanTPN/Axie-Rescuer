using Unity.Burst;
using Unity.Entities;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    public partial struct ChangeToDefaultWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<WeaponsBuffer>();
            state.RequireForUpdate<EquippingWeapon>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            if (!state.EntityManager.HasComponent<EquippingWeapon>(player)) return;
            var equipingWeapon = state.EntityManager.GetComponentObject<EquippingWeapon>(player);
            if (equipingWeapon.Entity.Equals(Entity.Null)) return;
            var magazineData = state.EntityManager.GetComponentData<MagazineData>(equipingWeapon.Entity);
            if (magazineData.TotalValue > 0) return;
            var weaponDynamicBuffer = SystemAPI.GetSingletonBuffer<WeaponsBuffer>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            ecb.SetComponent(player, new InitialWeapon { WeaponEntity = weaponDynamicBuffer[0].Value });
            ecb.SetComponentEnabled<InitialWeapon>(player, true);
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}