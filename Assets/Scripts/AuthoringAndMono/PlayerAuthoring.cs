using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [Header("Movement")]
        public float MoveSpeed;
        [Range(0, 1)] public float RotationSpeed;

        [Header("Stats")]
        public float MaxHealth;

        [Header("Weapon")]
        public GameObject Weapon;

        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                #region Tags
                AddComponent<PlayerTag>(entity);

                #endregion

                #region Movement Components
                AddComponent(entity, new MoveDirection
                {
                    Value = new float3(0, 0, 0),
                });
                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.MoveSpeed,
                });
                AddComponent(entity, new TargetRotationDirection
                {
                    Value = new float3(0, 0, 0),
                });
                AddComponent(entity, new RotationSpeed
                {
                    Value = authoring.RotationSpeed,
                });

                #endregion

                #region Stats Components
                AddComponent(entity, new Health
                {
                    Max = authoring.MaxHealth,
                    Current = authoring.MaxHealth,
                });

                #endregion

                #region Weapon Components
                AddComponent(entity, new InitialWeapon
                {
                    WeaponEntity = GetEntity(authoring.Weapon, TransformUsageFlags.Dynamic),
                });
                SetComponentEnabled<InitialWeapon>(entity, false);

                #endregion

            }
        }
    }
}