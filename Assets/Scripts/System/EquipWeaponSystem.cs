using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct EquipWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterAnimatorReference>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            #region Initialize
            foreach (var (_, entity) in SystemAPI.Query<LocalTransform>()
                .WithAll<InitialWeapon>()
                .WithAbsent<EquippingWeapon>()
                .WithEntityAccess()
                )
            {
                ecb.AddComponent(entity, new EquippingWeapon 
                {
                    Entity = Entity.Null,
                });
            }
            foreach (var (_, entity) in SystemAPI.Query<LocalTransform>()
                .WithDisabled<InitialWeapon>()
                .WithAbsent<EquippingWeapon>()
                .WithEntityAccess()
                )
            {
                ecb.AddComponent(entity, new EquippingWeapon
                {
                    Entity = Entity.Null,
                });
            }

            #endregion

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
                var weaponType = state.EntityManager.GetComponentData<WeaponType>(newWeaponEntity);
                var newWeaponObject = Object.Instantiate(newWeaponPrefab.Value);
                Transform hand;
                if(weaponType.Value == WeaponTypeEnum.Bow)
                {
                    hand = animatorRef.Value.GetBoneTransform(HumanBodyBones.LeftHand);
                }
                else
                {
                    hand = animatorRef.Value.GetBoneTransform(HumanBodyBones.RightHand);
                }

                // Add new weapon to Entity and GameObject
                equippingWeapon.Entity = newWeaponEntity;
                equippingWeapon.Object = newWeaponObject;
                ecb.AppendToBuffer<Child>(entity, new Child { Value = newWeaponEntity });
                newWeaponObject.transform.SetParent(hand);
                newWeaponObject.transform.localPosition = Vector3.zero;
                newWeaponObject.transform.localRotation = Quaternion.identity;

                // Set animator value
                animatorRef.Value.SetInteger("WeaponType_int", ((byte)weaponType.Value));
                if (weaponType.Value != WeaponTypeEnum.Handgun)
                {
                    animatorRef.Value.SetFloat("Body_Horizontal_f", 0.5f);
                    animatorRef.Value.SetFloat("Head_Horizontal_f", -0.5f);
                }
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