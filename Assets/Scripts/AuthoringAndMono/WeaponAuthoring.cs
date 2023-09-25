using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public class WeaponAuthoring : MonoBehaviour
    {
        public GameObject WeaponPrefab;
        public GameObject GunFlashPrefab;
        public float3 GunFlashOffset;
        public float GunFlashScale;
        public WeaponTypeEnum WeaponType;
        public float2 DamageRange;
        public float Range;
        [Range(0, 100)] public int Accuracy;
        public float FireRate;
        public float ReloadTime;
        public int TotalMagazineValue;
        public int MaxValuePerReload;

        public class WeaponBaker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new WeaponPrefab
                {
                    Value = authoring.WeaponPrefab,
                });
                AddComponent(entity, new WeaponType
                {
                    Value = authoring.WeaponType,
                });
                AddComponent(entity, new GunFlash
                {
                    Entity = GetEntity(authoring.GunFlashPrefab, TransformUsageFlags.Dynamic),
                    Offset = authoring.GunFlashOffset,
                    Scale = authoring.GunFlashScale,
                });
                AddComponent(entity, new Damage
                {
                    MaxValue = authoring.DamageRange.x,
                    MinValue = authoring.DamageRange.y,
                });
                AddComponent(entity, new Range
                {
                    Value = authoring.Range,
                });
                AddComponent(entity, new Accuracy
                {
                    Value = authoring.Accuracy,
                });
                AddComponent(entity, new FireRate
                {
                    Value = authoring.FireRate,
                });
                AddComponent(entity, new ReloadTime
                {
                    Value = authoring.ReloadTime,
                });
                AddComponent(entity, new MagazineData
                {
                    TotalValue = authoring.TotalMagazineValue,
                    CurrentValue = authoring.MaxValuePerReload,
                    MaxValuePerReload = authoring.MaxValuePerReload,
                });
                AddComponent<DroppedItem>(entity);
                SetComponentEnabled<DroppedItem>(entity, false);
            }
        }
    }
}
