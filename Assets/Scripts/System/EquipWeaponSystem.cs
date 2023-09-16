using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct EquipWeaponSystem : ISystem
    {
        private EntityQuery _initialEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _initialEntityQuery = SystemAPI.QueryBuilder()
                .WithDisabled<InitialWeapon>()
                .WithNone<EquippingWeapon>()
                .Build();
            state.RequireForUpdate<CharacterAnimatorReference>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Initialize
            ecb.AddComponent(_initialEntityQuery, new EquippingWeapon { });

            #region Switch Weapon
            foreach (var (animatorRef,equippingWeapon, nextWeapon, entity) in 
                SystemAPI.Query<CharacterAnimatorReference, EquippingWeapon, InitialWeapon>()
                .WithEntityAccess()
            )
            {
                state.EntityManager.SetComponentEnabled<InitialWeapon>(entity, false);

                // Remove current weapon
                if(equippingWeapon.Entity != Entity.Null)
                {
                    ecb.DestroyEntity(equippingWeapon.Entity);
                    GameObject.Destroy(equippingWeapon.Object);
                }

                // Create new weapon
                var newWeaponEntity = state.EntityManager.Instantiate(nextWeapon.WeaponEntity);
                var newWeaponPrefab = state.EntityManager.GetComponentObject<WeaponPrefab>(newWeaponEntity);
                var newWeaponObject = Object.Instantiate(newWeaponPrefab.Value);
                var rightHand = animatorRef.Value.GetBoneTransform(HumanBodyBones.RightHand);
                var weaponType = state.EntityManager.GetComponentData<WeaponType>(newWeaponEntity);

                // Add new weapon to Entity and GameObject
                equippingWeapon.Entity = newWeaponEntity;
                equippingWeapon.Object = newWeaponObject;
                state.EntityManager.GetBuffer<Child>(entity).Append(new Child { Value = newWeaponEntity });
                newWeaponObject.transform.SetParent(rightHand);
                newWeaponObject.transform.localPosition = Vector3.zero;
                newWeaponObject.transform.localRotation = Quaternion.identity;

                // Set animator value
                animatorRef.Value.SetInteger("WeaponType_int", ((byte)weaponType.Value));
                animatorRef.Value.SetFloat("Body_Horizontal_f", 0.5f);
                animatorRef.Value.SetFloat("Head_Horizontal_f", -0.5f);
            }

            #endregion

            #region Cleanup
            foreach(var equippingWeapon in SystemAPI.Query<EquippingWeapon>()
                .WithAbsent<InitialWeapon>()
                 )
            {
                if (equippingWeapon.Entity != Entity.Null)
                {
                    ecb.DestroyEntity(equippingWeapon.Entity);
                    GameObject.Destroy(equippingWeapon.Object);
                }
            }

            #endregion

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}