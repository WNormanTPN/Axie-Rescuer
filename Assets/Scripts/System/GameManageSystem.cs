using Unity.Burst;
using Unity.Entities;
using Unity.Logging;
using UnityEngine;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct GameManageSystem : ISystem, ISystemStartStop
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
        }
        public void OnStartRunning(ref SystemState state)
        {
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            state.EntityManager.SetComponentEnabled<InitialWeapon>(playerEntity, true);
            var world = World.DefaultGameObjectInjectionWorld;
            SystemHandle mySystem = world.GetExistingSystem<SpawnZombieSystem>();
            ref SystemState spawnZombieSystem = ref world.Unmanaged.ResolveSystemStateRef(mySystem);
            spawnZombieSystem.Enabled = true;
            //var spawnZombieSystemState = world.ResolveSystemStateRef(spawnZombieSystem);
            //spawnZombieSystemState.Enabled = true; 
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}