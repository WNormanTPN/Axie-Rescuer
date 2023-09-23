using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
    public partial struct CharacterMovementAnimateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            #region Initial Animator
            foreach (var (CharacterGameObjectPrefab, entity) in
                     SystemAPI.Query<CharacterGameObjectPrefab>().WithNone<CharacterAnimatorReference>().WithEntityAccess())
            {
                var newCompanionGameObject = Object.Instantiate(CharacterGameObjectPrefab.Value);
                StaticGameObjectReference.Player = newCompanionGameObject;
                var newAnimatorReference = new CharacterAnimatorReference
                {
                    Value = newCompanionGameObject.GetComponent<Animator>()
                };
                ecb.AddComponent(entity, newAnimatorReference);
            }

            #endregion

            #region Character Movement
            foreach (var (transform, animatorReference, moveDirection) in
                     SystemAPI.Query<LocalTransform, CharacterAnimatorReference, MoveDirection>().WithNone<ZombieTag>())
            {
                animatorReference.Value.transform.rotation = transform.Rotation;
                animatorReference.Value.transform.position = transform.Position;
                if (moveDirection.Value.Equals(float3.zero))
                {
                    animatorReference.Value.SetFloat("Speed_f", 0);
                }
                else
                {
                    var heading = transform.Forward();
                    var headingAngle = math.atan2(heading.z, heading.x);
                    var moveAngle = math.atan2(moveDirection.Value.z, moveDirection.Value.x);
                    var offsetAngle = math.abs(headingAngle - moveAngle);
                    var animationValue = (offsetAngle < math.PI * 2 / 3.0) ? 1 : -1;
                    animatorReference.Value.SetFloat("Speed_f", animationValue);
                }
            }

            #endregion

            #region Zombie Movement
            foreach (var (transform, animatorReference, moveDirection) in
                     SystemAPI.Query<LocalTransform, CharacterAnimatorReference, MoveDirection>().WithAll<ZombieTag>())
            {
                animatorReference.Value.Play("Zombie_Walk");
                animatorReference.Value.transform.position = transform.Position;
                animatorReference.Value.transform.rotation = transform.Rotation;
            }

            #endregion

            #region Cleanup
            foreach (var (animatorReference, entity) in
                     SystemAPI.Query<CharacterAnimatorReference>().WithNone<CharacterGameObjectPrefab, LocalTransform>()
                         .WithEntityAccess())
            {
                Object.Destroy(animatorReference.Value.gameObject);
                ecb.RemoveComponent<CharacterAnimatorReference>(entity);
            }

            #endregion

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}