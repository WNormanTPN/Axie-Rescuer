using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngine;
using Unity.Logging;

namespace AxieRescuer
{
    [BurstCompile]
    [UpdateAfter(typeof(ItemDropSystem))]
    public partial struct WeaponDropInitializeSystem : ISystem
    {
        private EntityQuery _query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<WeaponNeedInitTag>()
                .WithAll<DroppedItem>()
                .Build();
            state.RequireForUpdate(_query);
        }
        public void OnUpdate(ref SystemState state) 
        {
            foreach(var (weaponPrefab,needInit,localTransform,objectRef) in SystemAPI.Query<WeaponPrefab,EnabledRefRW<WeaponNeedInitTag>,LocalTransform,WeaponObjectReference>())
            {
                needInit.ValueRW = false;
                var newWeaponGameObject = Object.Instantiate(weaponPrefab.Value);
                newWeaponGameObject.transform.position = localTransform.Position;
                objectRef.Value = newWeaponGameObject;
                newWeaponGameObject.transform.localScale = 2 * newWeaponGameObject.transform.localScale;
            }
        }
    }
}